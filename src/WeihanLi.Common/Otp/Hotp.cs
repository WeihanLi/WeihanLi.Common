using System;

namespace WeihanLi.Common.Otp
{
    /// <summary>
    /// Calculate HMAC-Based One-Time-Passwords (HOTP) from a secret key
    /// </summary>
    /// <remarks>
    /// The specifications for this are found in RFC 4226
    /// http://tools.ietf.org/html/rfc4226
    /// </remarks>
    internal class Hotp : Otp
    {
        private readonly int hotpSize;

        /// <summary>
        /// Create a HOTP instance
        /// </summary>
        /// <param name="secretKey">The secret key to use in HOTP calculations</param>
        /// <param name="mode">The hash mode to use</param>
        /// <param name="hotpSize">The number of digits that the returning HOTP should have.  The default is 6. 6~8 is valid</param>
        public Hotp(byte[] secretKey, OtpHashMode mode = OtpHashMode.Sha1, int hotpSize = 6)
            : base(secretKey, mode)
        {
            VerifyParameters(hotpSize);

            this.hotpSize = hotpSize;
        }

        private static void VerifyParameters(int hotpSize)
        {
            if (hotpSize < 6 || hotpSize > 8)
                throw new ArgumentOutOfRangeException(nameof(hotpSize));
        }

        /// <summary>
        /// Takes a counter and then computes a HOTP value
        /// </summary>
        /// <param name="counter"></param>
        /// <returns>a HOTP value</returns>
        public string ComputeHOTP(long counter)
        {
            return Compute(counter, hashMode);
        }

        /// <summary>
        /// Verify a value that has been provided with the calculated value
        /// </summary>
        /// <param name="hotp">the trial HOTP value</param>
        /// <param name="counter">The counter value to verify</param>
        /// <returns>True if there is a match.</returns>
        public bool VerifyHotp(string hotp, long counter)
        {
            if (hotp == ComputeHOTP(counter))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Takes a time step and computes a HOTP code
        /// </summary>
        /// <param name="counter">counter</param>
        /// <param name="mode">The hash mode to use</param>
        /// <returns>HOTP calculated code</returns>
        protected override string Compute(long counter, OtpHashMode mode)
        {
            var data = KeyUtilities.GetBigEndianBytes(counter);
            var otp = this.CalculateOtp(data, mode);
            return Digits(otp, this.hotpSize);
        }
    }
}
