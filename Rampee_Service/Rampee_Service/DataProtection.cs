using System;
using System.Security.Cryptography;

namespace Rampee_Service
{
    internal class DataProtection
    {
        // https://msdn.microsoft.com/en-us/library/system.security.cryptography.dataprotectionscope(v=vs.110).aspx

        internal static byte[] Unprotect(byte[] password, byte[] salt)
        {
            try
            {
                //Decrypt the data using DataProtectionScope.LocalMachine.
                return ProtectedData.Unprotect(password, salt, DataProtectionScope.LocalMachine);
            }
            catch (CryptographicException e)
            {
                LogClient.Record(0000, string.Format("Unprotect: An error occurred: {0} :: {1}", e.Message, e.InnerException));
                throw;
            }
        }

        internal static byte[] Protect(byte[] data, byte[] salt)
        {
            try
            {
                // Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted
                //  only by the same current user.
                return ProtectedData.Protect(data, salt, DataProtectionScope.LocalMachine);
            }
            catch (Exception e)
            {
                LogClient.Record(0000, string.Format("Protect: An error occurred: {0}", e.Message));
                throw;
            }
        }

        internal static byte[] GetEntropy()
        {
            Random rnd = new Random();
            Byte[] b = new Byte[20];
            rnd.NextBytes(b);
            return b;
        }
    }
}