using System;
using WeihanLi.Common.Otp;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers
{
    public static class TotpHelper
    {
        private static readonly Lazy<Totp> Totp = new(() => new Totp(DefaultOptions.Algorithm, DefaultOptions.Size));

        private static readonly TotpOptions DefaultOptions = new();

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

            if (DefaultOptions.Salt == null)
            {
                return Totp.Value.Compute(securityToken);
            }

            var bytes = new byte[securityToken.Length + DefaultOptions.SaltBytes.Length];
            Array.Copy(securityToken, bytes, securityToken.Length);
            Array.Copy(DefaultOptions.SaltBytes, 0, bytes, securityToken.Length, DefaultOptions.SaltBytes.Length);

            return Totp.Value.Compute(bytes);
        }

        /// <summary>
        /// Generates code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token to generate code.</param>
        /// <returns>The generated code.</returns>
        public static string GenerateCode(string securityToken) => GenerateCode(securityToken?.GetBytes());

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
        public static bool VerifyCode(byte[] securityToken, string code, int expiresIn = -1)
        {
            if (securityToken == null)
            {
                throw new ArgumentNullException(nameof(securityToken));
            }
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            if (null == DefaultOptions.SaltBytes)
            {
                return Totp.Value.Verify(securityToken, code, TimeSpan.FromSeconds(expiresIn >= 0 ? expiresIn : DefaultOptions.ExpiresIn));
            }

            var saltBytes = DefaultOptions.SaltBytes;
            var bytes = new byte[securityToken.Length + saltBytes.Length];
            Array.Copy(securityToken, bytes, securityToken.Length);
            Array.Copy(saltBytes, 0, bytes, securityToken.Length, saltBytes.Length);

            return Totp.Value.Verify(bytes, code, TimeSpan.FromSeconds(expiresIn >= 0 ? expiresIn : DefaultOptions.ExpiresIn));
        }

        /// <summary>
        /// Validates the code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token for verifying.</param>
        /// <param name="code">The code to validate.</param>
        /// <param name="expiresIn">expiresIn, in seconds</param>
        /// <returns><c>True</c> if validate succeed, otherwise, <c>false</c>.</returns>
        public static bool VerifyCode(string securityToken, string code, int expiresIn = -1) => VerifyCode(securityToken?.GetBytes(), code, expiresIn);
    }
}
