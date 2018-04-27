using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpObjects
{
	public class StreamEncryption
	{
		private const int BUFFER = 0x10000; // 65536, an arbitrary power of 2
		private readonly PgpKeys _pgpKeys;

		public StreamEncryption(PgpKeys keys)
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys", "keys object is null");
			}
			_pgpKeys = keys;
		}

		// Chain streams from input to PgpLiteralData to PgpEncryptedData
		private Stream ChainEncryptedOut(Stream outputStream)
		{
			var generator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256, true, new SecureRandom());
			generator.AddMethod(_pgpKeys.PublicKey);
			return generator.Open(outputStream, new byte[BUFFER]);
		}

		private static Stream ChainLiteralOut(Stream encryptedOut, string fileName)
		{
			var generator = new PgpLiteralDataGenerator();
			// fileName is just a data point, but some implementations check it for validation
			return generator.Open(encryptedOut, PgpLiteralData.Utf8, fileName, DateTime.Now, new byte[BUFFER]);
		}

		// Decrypt method just needs input and output
		public void Decrypt(Stream inputStream, Stream outputStream)
		{
			// Load input stream into PgpEncryptedDataList
			var input = PgpUtilities.GetDecoderStream(inputStream);
			var encryptedFactory = new PgpObjectFactory(input);
			PgpEncryptedDataList dataList;
			// Iterate until we hit the first PgpEncryptedDataList
			PgpObject obj;
			do {
				obj = encryptedFactory.NextPgpObject();
			} while (!(obj is PgpEncryptedDataList));
			dataList = (PgpEncryptedDataList)obj;

			// Use our private key to decrypt
			PgpPublicKeyEncryptedData encryptedData =
				dataList.GetEncryptedDataObjects().Cast<PgpPublicKeyEncryptedData>().First();
			Stream decryptedData = encryptedData.GetDataStream(_pgpKeys.PrivateKey);
			var decryptedFactory = new PgpObjectFactory(decryptedData);

			PgpObject message = decryptedFactory.NextPgpObject();
			// If the message starts with a signature, jump to the next object
			if (message is PgpOnePassSignatureList)
			{
				message = decryptedFactory.NextPgpObject();
			}
			PgpLiteralData literalData = (PgpLiteralData)message;
			// Pipe the literal data to our output stream
			Stream decryptedLiteral = literalData.GetInputStream();
			Streams.PipeAll(decryptedLiteral, outputStream);
		}

		// Encrypt method also uses fileName and a signing setting
		public void Encrypt(string fileName, Stream inputStream, Stream outputStream, bool sign = false)
		{
			// Init this to null so the IDE doesn't give us warnings
			PgpSignatureGenerator signatureGenerator = null;

			// Encrypted stream is the outer layer, gets fed to the output stream
			Stream encryptedOut = ChainEncryptedOut(outputStream);
			if (sign)
			{
				signatureGenerator = InitSignatureGenerator(encryptedOut);
			}

			// Literal stream is the inner layer, gets fed to the encrypted stream
			Stream literalOut = ChainLiteralOut(encryptedOut, fileName);
			int length = 0;
			byte[] buffer = new byte[BUFFER];
			// Standard .NET stream reading/writing stuff
			while ((length = inputStream.Read(buffer, 0, buffer.Length)) > 0)
			{
				literalOut.Write(buffer, 0, length);
				if (sign)
				{
					// Keep the signature generator in sync
					signatureGenerator.Update(buffer, 0, length);
				}
			}
			literalOut.Close();

			if (sign)
			{
				// Generate and apply the signature to the outer stream at the end
				signatureGenerator.Generate().Encode(encryptedOut);
			}
			encryptedOut.Close();
		}

		// Signature generator builder is only needed if we're signing
		private PgpSignatureGenerator InitSignatureGenerator(Stream streamToSign)
		{
			// Get our generator set up to match our keys
			PublicKeyAlgorithmTag tag = _pgpKeys.SecretKey.PublicKey.Algorithm;
			var pgpSignatureGenerator = new PgpSignatureGenerator(tag, HashAlgorithmTag.Sha1);
			pgpSignatureGenerator.InitSign(PgpSignature.BinaryDocument, _pgpKeys.PrivateKey);

			// Get the user identity from our secret key and use it for signing
			var userIds = _pgpKeys.SecretKey.PublicKey.GetUserIds().OfType<string>();
			string userId = userIds.FirstOrDefault();
			if (userId != null)
			{
				var subPacketGenerator = new PgpSignatureSubpacketGenerator();
				subPacketGenerator.SetSignerUserId(isCritical: false, userId: userId);
				pgpSignatureGenerator.SetHashedSubpackets(subPacketGenerator.Generate());
			}

			// Kick things off and send the generator back to caller for the rest
			pgpSignatureGenerator.GenerateOnePassVersion(isNested: false).Encode(streamToSign);
			return pgpSignatureGenerator;
		}
	}
}
