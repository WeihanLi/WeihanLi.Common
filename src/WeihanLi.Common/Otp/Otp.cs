using System;

namespace WeihanLi.Common.Otp
{
    /// <summary>
    /// An abstract class that contains common OTP calculations
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc4226
    /// </remarks>
    internal abstract class Otp
    {
        /// <summary>
        /// Secret key
        /// </summary>
        protected readonly IKeyProvider secretKey;

        /// <summary>
        /// The hash mode to use
        /// </summary>
        protected readonly OtpHashMode hashMode;

        /// <summary>
        /// Constructor for the abstract class.  This is to guarantee that all implementations have a secret key
        /// </summary>
        /// <param name="secretKey"></param>
        /// <param name="mode">The hash mode to use</param>
        protected Otp(byte[] secretKey, OtpHashMode mode)
        {
            if (secretKey == null)
                throw new ArgumentNullException(nameof(secretKey));
            if (secretKey.Length == 0)
                throw new ArgumentException("secretKey empty", nameof(secretKey));

            // when passing a key into the constructor the caller may depend on the reference to the key remaining intact.
            this.secretKey = new InMemoryKey(secretKey);

            hashMode = mode;
        }

        /// <summary>
        /// An abstract definition of a compute method.  Takes a counter and runs it through the derived algorithm.
        /// </summary>
        /// <param name="counter">Counter or step</param>
        /// <param name="mode">The hash mode to use</param>
        /// <returns>OTP calculated code</returns>
        protected abstract string Compute(long counter, OtpHashMode mode);

        /// <summary>
        /// Helper method that calculates OTPs
        /// </summary>
        protected internal long CalculateOtp(byte[] data, OtpHashMode mode)
        {
            var hmacComputedHash = secretKey.ComputeHmac(mode, data);

            // The RFC has a hard coded index 19 in this value.
            // This is the same thing but also accomodates SHA256 and SHA512
            // hmacComputedHash[19] => hmacComputedHash[hmacComputedHash.Length - 1]

            var offset = hmacComputedHash[hmacComputedHash.Length - 1] & 0x0F;
            return (hmacComputedHash[offset] & 0x7f) << 24
                | (hmacComputedHash[offset + 1] & 0xff) << 16
                | (hmacComputedHash[offset + 2] & 0xff) << 8
                | (hmacComputedHash[offset + 3] & 0xff) % 1000000;
        }

        /// <summary>
        /// truncates a number down to the specified number of digits
        /// </summary>
        protected internal static string Digits(long input, int digitCount)
        {
            var truncatedValue = ((int)input % (int)Math.Pow(10, digitCount));
            return truncatedValue.ToString().PadLeft(digitCount, '0');
        }

        /// <summary>
        /// Verify an OTP value
        /// </summary>
        /// <param name="initialStep">The initial step to try</param>
        /// <param name="valueToVerify">The value to verify</param>
        /// <param name="matchedStep">Output parameter that provides the step where the match was found.  If no match was found it will be 0</param>
        /// <param name="window">The window to verify</param>
        /// <returns>True if a match is found</returns>
        protected bool Verify(long initialStep, string valueToVerify, out long matchedStep, VerificationWindow window)
        {
            if (window == null)
                window = new VerificationWindow();
            foreach (var frame in window.ValidationCandidates(initialStep))
            {
                var comparisonValue = Compute(frame, hashMode);
                if (ValuesEqual(comparisonValue, valueToVerify))
                {
                    matchedStep = frame;
                    return true;
                }
            }

            matchedStep = 0;
            return false;
        }

        // Constant time comparison of two values
        private bool ValuesEqual(string a, string b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            var result = 0;
            for (var i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }

            return result == 0;
        }
    }
}
