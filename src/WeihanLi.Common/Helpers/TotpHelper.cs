using System;
using WeihanLi.Common.Otp;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers
{
    public class TotpHelper
    {
        private static readonly Lazy<Totp> Totp = new Lazy<Totp>(() => new Totp(DefaultOptions.Algorithm, DefaultOptions.Size));

        private static readonly TotpOptions DefaultOptions = new TotpOptions();

        public static TotpOptions ConfigureTotpOptions(Action<TotpOptions> configAction)
        {
            configAction?.Invoke(DefaultOptions);
            return DefaultOptions;
        }

        /// <summary>
        /// Generates code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token to generate code.</param>
        /// <returns>The generated code.</returns>
        public static string GenerateCode(byte[] securityToken)
        {
            if (securityToken == null)
            {
                throw new ArgumentNullException(nameof(securityToken));
            }

            if (DefaultOptions.Salt.IsNullOrEmpty())
            {
                return Totp.Value.Compute(securityToken);
            }

            var saltBytes = DefaultOptions.Salt.GetBytes();
            var bytes = new byte[securityToken.Length + saltBytes.Length];
            Array.Copy(securityToken, bytes, securityToken.Length);
            Array.Copy(saltBytes, 0, bytes, securityToken.Length, saltBytes.Length);

            return Totp.Value.Compute(bytes);
        }

        /// <summary>
        /// ttl of the code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token to generate code.</param>
        /// <returns>the code remaining seconds expires in</returns>
        public static int TTL(byte[] securityToken)
        {
            if (securityToken == null)
            {
                throw new ArgumentNullException(nameof(securityToken));
            }

            return Totp.Value.RemainingSeconds();
        }

        /// <summary>
        /// Validates the code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token for verifying.</param>
        /// <param name="code">The code to validate.</param>
        /// <param name="expiresIn">expiresIn, in seconds</param>
        /// <returns><c>True</c> if validate succeed, otherwise, <c>false</c>.</returns>
        public static bool VerifyCode(byte[] securityToken, string code, int expiresIn = 30)
        {
            if (securityToken == null)
            {
                throw new ArgumentNullException(nameof(securityToken));
            }
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            var saltBytes = DefaultOptions.Salt.GetBytes();
            var bytes = new byte[securityToken.Length + saltBytes.Length];
            Array.Copy(securityToken, bytes, securityToken.Length);
            Array.Copy(saltBytes, 0, bytes, securityToken.Length, saltBytes.Length);

            var validateResult = Totp.Value.Verify(bytes, code, TimeSpan.FromSeconds(expiresIn));
            return validateResult;
        }

        /// <summary>
        /// Generates code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token to generate code.</param>
        /// <returns>The generated code.</returns>
        public static string GenerateCode(string securityToken) => GenerateCode(System.Text.Encoding.UTF8.GetBytes(securityToken));

        /// <summary>
        /// ttl of the code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token to generate code.</param>
        public static int TTL(string securityToken) => TTL(System.Text.Encoding.UTF8.GetBytes(securityToken));

        /// <summary>
        /// Validates the code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token for verifying.</param>
        /// <param name="code">The code to validate.</param>
        /// <param name="expiresIn">expiresIn, in seconds</param>
        /// <returns><c>True</c> if validate succeed, otherwise, <c>false</c>.</returns>
        public static bool VerifyCode(string securityToken, string code, int expiresIn = 30) => VerifyCode(System.Text.Encoding.UTF8.GetBytes(securityToken), code, expiresIn);
    }
}
