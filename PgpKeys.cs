using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpObjects
{
	public class PgpKeys
	{
		private bool _toEncrypt;

		public PgpPublicKey PublicKey { get; set; }
		public PgpPrivateKey PrivateKey { get; set; }
		public PgpSecretKey SecretKey { get; set; }

		public PgpKeys(Stream publicKeyStream, Stream privateKeyStream = null, string passPhrase = null, bool toEncrypt = true)
		{
			_toEncrypt = toEncrypt;
			PublicKey = ReadPublicKey(publicKeyStream);
			if (privateKeyStream != null)
			{
				SecretKey = ReadSecretKey(privateKeyStream);
				PrivateKey = ReadPrivateKey(passPhrase);
			}
		}

		private PgpPublicKey GetFirstPublicKey(PgpPublicKeyRingBundle publicKeyRingBundle)
		{
			foreach (PgpPublicKeyRing kRing in publicKeyRingBundle.GetKeyRings())
			{
				PgpPublicKey key = kRing.GetPublicKeys()
					.Cast<PgpPublicKey>()
					.FirstOrDefault(k => k.IsEncryptionKey);
				if (key != null)
				{
					return key;
				}
			}
			return null;
		}

		private PgpSecretKey GetFirstSecretKey(PgpSecretKeyRingBundle secretKeyRingBundle)
		{
			foreach (PgpSecretKeyRing kRing in secretKeyRingBundle.GetKeyRings())
			{
				PgpSecretKey key = kRing.GetSecretKeys()
					.Cast<PgpSecretKey>()
					.FirstOrDefault(k => k.IsSigningKey);
				if (key != null)
				{
					return key;
				}
			}
			return null;
		}

		private PgpSecretKey GetLastSecretKey(PgpSecretKeyRingBundle secretKeyRingBundle)
		{
			return (from PgpSecretKeyRing kRing in secretKeyRingBundle.GetKeyRings()
					select kRing.GetSecretKeys().Cast<PgpSecretKey>()
					.LastOrDefault(k => k.IsSigningKey))
					.LastOrDefault(key => key != null);
		}

		private PgpPublicKey ReadPublicKey(Stream publicKeyStream)
		{
			using (Stream inputStream = PgpUtilities.GetDecoderStream(publicKeyStream))
			{
				PgpPublicKeyRingBundle publicKeyRingBundle = new PgpPublicKeyRingBundle(inputStream);
				PgpPublicKey foundKey = GetFirstPublicKey(publicKeyRingBundle);
				if (foundKey != null)
				{
					return foundKey;
				}
			}
			throw new ArgumentException("No encryption key found in public key ring.");
		}

		private PgpPrivateKey ReadPrivateKey(string passPhrase)
		{
			PgpPrivateKey privateKey = SecretKey.ExtractPrivateKey(passPhrase.ToCharArray());
			if (privateKey != null)
			{
				return privateKey;
			}
			throw new ArgumentException("No private key found in secret key.");
		}

		private PgpSecretKey ReadSecretKey(Stream privateKeyStream)
		{
			using (Stream inputStream = PgpUtilities.GetDecoderStream(privateKeyStream))
			{
				PgpSecretKeyRingBundle secretKeyRingBundle = new PgpSecretKeyRingBundle(inputStream);
				PgpSecretKey foundKey = GetFirstSecretKey(secretKeyRingBundle);
				if (foundKey != null)
				{
					return foundKey;
				}
				throw new ArgumentException("Can't find signing key in key ring.");
			}
		}
	}
}
