using System;
using System.Security.Cryptography;

namespace WeihanLi.Common.Otp
{
    /// <summary>
    /// Represents a key in memory
    /// </summary>
    /// <remarks>
    /// This will attempt to use the Windows data protection api to encrypt the key in memory.
    /// However, this type favors working over memory protection. This is an attempt to minimize
    /// exposure in memory, nothing more. This protection is flawed in many ways and is limited
    /// to Windows.
    ///
    /// In order to use the key to compute an hmac it must be temporarily decrypted, used,
    /// then re-encrypted. This does expose the key in memory for a time. If a memory dump occurs in this time
    /// the plaintext key will be part of it. Furthermore, there are potentially
    /// artifacts from the hmac computation, GC compaction, or any number of other leaks even after
    /// the key is re-encrypted.
    ///
    /// This type favors working over memory protection. If the particular platform isn't supported then,
    /// unless forced by modifying the IsPlatformSupported method, it will just store the key in a standard
    /// byte array.
    /// </remarks>
    internal class InMemoryKey : IKeyProvider
    {
        private static readonly object platformSupportSync = new object();
        private readonly object stateSync = new object();
        private readonly byte[] KeyData;
        private readonly int keyLength;

        /// <summary>
        /// Creates an instance of a key.
        /// </summary>
        /// <param name="key">Plaintext key data</param>
        public InMemoryKey(byte[] key)
        {
            if (!(key != null))
                throw new ArgumentNullException("key");
            if (!(key.Length > 0))
                throw new ArgumentException("The key must not be empty");

            this.keyLength = key.Length;
            int paddedKeyLength = (int)Math.Ceiling((decimal)key.Length / (decimal)16) * 16;
            this.KeyData = new byte[paddedKeyLength];
            Array.Copy(key, this.KeyData, key.Length);
        }

        /// <summary>
        /// Gets a copy of the plaintext key
        /// </summary>
        /// <remarks>
        /// This is internal rather than protected so that the tests can use this method
        /// </remarks>
        /// <returns>Plaintext Key</returns>
        internal byte[] GetCopyOfKey()
        {
            var plainKey = new byte[this.keyLength];
            lock (this.stateSync)
            {
                Array.Copy(this.KeyData, plainKey, this.keyLength);
            }
            return plainKey;
        }

        /// <summary>
        /// Uses the key to get an HMAC using the specified algorithm and data
        /// </summary>
        /// <param name="mode">The HMAC algorithm to use</param>
        /// <param name="data">The data used to compute the HMAC</param>
        /// <returns>HMAC of the key and data</returns>
        public byte[] ComputeHmac(OtpHashMode mode, byte[] data)
        {
            byte[] hashedValue = null;
            using (HMAC hmac = CreateHmacHash(mode))
            {
                byte[] key = this.GetCopyOfKey();
                try
                {
                    hmac.Key = key;
                    hashedValue = hmac.ComputeHash(data);
                }
                finally
                {
                    KeyUtilities.Destroy(key);
                }
            }

            return hashedValue;
        }

        /// <summary>
        /// Create an HMAC object for the specified algorithm
        /// </summary>
        private static HMAC CreateHmacHash(OtpHashMode otpHashMode)
        {
            HMAC hmacAlgorithm = null;
            switch (otpHashMode)
            {
                case OtpHashMode.Sha256:
                    hmacAlgorithm = new HMACSHA256();
                    break;

                case OtpHashMode.Sha512:
                    hmacAlgorithm = new HMACSHA512();
                    break;

                default: //case OtpHashMode.Sha1:
                    hmacAlgorithm = new HMACSHA1();
                    break;
            }
            return hmacAlgorithm;
        }
    }
}
