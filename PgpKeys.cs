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
		// Constructor; if you only want to decrypt, you'd only need publicKeyStream

		// Get the first public key from a key ring bundle

		// Pre-built methods for getting the first or last secret key from a key ring bundle.
		// Depending on your exact configuration, you may need to play with LINQ a bit
		// to get the correct secret key.
		// If you get a PgpException: exception decrypting secret key
		// with an inner InvalidCipherTextException: unknown block type
		// that's probably what's causing it. For me, it was trial and error.

		// Read the public key into memory for encrypting data

		// Private key is derived from secret key using passphrase

		// Secret key (private key outside Bouncy Castle) is used for decrypting and signing
	}
}
