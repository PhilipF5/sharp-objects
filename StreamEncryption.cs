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
		private const int BUFFER = 0x10000;
		private readonly PgpKeys _pgpKeys;

		public StreamEncryption(PgpKeys keys)
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys", "keys object is null");
			}
			_pgpKeys = keys;
		}

		private Stream ChainEncryptedOut(Stream outputStream)
		{
			var generator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256, true, new SecureRandom());
			generator.AddMethod(_pgpKeys.PublicKey);
			return generator.Open(outputStream, new byte[BUFFER]);
		}

		private static Stream ChainLiteralOut(Stream encryptedOut, string fileName)
		{
			var generator = new PgpLiteralDataGenerator();
			return generator.Open(encryptedOut, PgpLiteralData.Utf8, fileName, DateTime.Now, new byte[BUFFER]);
		}

		public void Decrypt(Stream inputStream, Stream outputStream)
		{
			var input = PgpUtilities.GetDecoderStream(inputStream);
			var encryptedFactory = new PgpObjectFactory(input);
			PgpEncryptedDataList dataList;

			PgpObject obj = encryptedFactory.NextPgpObject();
			if (obj is PgpEncryptedDataList)
			{
				dataList = (PgpEncryptedDataList)obj;
			}
			else
			{
				dataList = (PgpEncryptedDataList)encryptedFactory.NextPgpObject();
			}

			PgpPublicKeyEncryptedData encryptedData =
				dataList.GetEncryptedDataObjects().Cast<PgpPublicKeyEncryptedData>().First();
			Stream decryptedData = encryptedData.GetDataStream(_pgpKeys.PrivateKey);
			var decryptedFactory = new PgpObjectFactory(decryptedData);

			PgpObject message = decryptedFactory.NextPgpObject();
			if (message is PgpOnePassSignatureList)
			{
				message = decryptedFactory.NextPgpObject();
			}
			PgpLiteralData literalData = (PgpLiteralData)message;
			Stream decryptedLiteral = literalData.GetInputStream();
			Streams.PipeAll(decryptedLiteral, outputStream);
		}

		public void Encrypt(string fileName, Stream inputStream, Stream outputStream, bool sign = false)
		{
			PgpSignatureGenerator signatureGenerator = null;

			Stream encryptedOut = ChainEncryptedOut(outputStream);
			if (sign)
			{
				signatureGenerator = InitSignatureGenerator(encryptedOut);
			}

			Stream literalOut = ChainLiteralOut(encryptedOut, fileName);
			int length = 0;
			byte[] buffer = new byte[BUFFER];
			while ((length = inputStream.Read(buffer, 0, buffer.Length)) > 0)
			{
				literalOut.Write(buffer, 0, length);
				if (sign)
				{
					signatureGenerator.Update(buffer, 0, length);
				}
			}
			literalOut.Close();

			if (sign)
			{
				signatureGenerator.Generate().Encode(encryptedOut);
			}
			encryptedOut.Close();
		}

		private PgpSignatureGenerator InitSignatureGenerator(Stream streamToSign)
		{
			PublicKeyAlgorithmTag tag = _pgpKeys.SecretKey.PublicKey.Algorithm;
			var pgpSignatureGenerator = new PgpSignatureGenerator(tag, HashAlgorithmTag.Sha1);
			pgpSignatureGenerator.InitSign(PgpSignature.BinaryDocument, _pgpKeys.PrivateKey);

			var userIds = _pgpKeys.SecretKey.PublicKey.GetUserIds().OfType<string>();
			string userId = userIds.FirstOrDefault();
			if (userId != null)
			{
				var subPacketGenerator = new PgpSignatureSubpacketGenerator();
				subPacketGenerator.SetSignerUserId(isCritical: false, userId: userId);
				pgpSignatureGenerator.SetHashedSubpackets(subPacketGenerator.Generate());
			}

			pgpSignatureGenerator.GenerateOnePassVersion(isNested: false).Encode(streamToSign);
			return pgpSignatureGenerator;
		}
	}
}
