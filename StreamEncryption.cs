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
		// Chain streams from input to PgpLiteralData to PgpEncryptedData

		// Decrypt method just needs input and output

		// Encrypt method also uses fileName and a signing setting

		// Signature generator builder is only needed if we're signing
	}
}
