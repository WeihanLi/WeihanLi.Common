// Copyright (c) Arch team. All rights reserved.

using System;
using System.Text;
using WeihanLi.Common.Otp;

namespace WeihanLi.Common.Helpers
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc6238
    /// </summary>
    public static class TotpHelper
    {
        /// <summary>
        /// Generates code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token to generate code.</param>
        /// <param name="expiresIn">expiresIn, in seconds</param>
        /// <param name="size">return  code size</param>
        /// <returns>The generated code.</returns>
        public static string GenerateCode(byte[] securityToken, int expiresIn = 30, int size = 6)
        {
            if (securityToken == null)
            {
                throw new ArgumentNullException(nameof(securityToken));
            }

            return new Totp(securityToken, expiresIn, totpSize: size).ComputeTotp();
        }

        /// <summary>
        /// ttl of the code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token to generate code.</param>
        /// <param name="expiresIn">expiresIn, in seconds</param>
        /// <param name="size">return  code size</param>
        /// <returns>the code remaining seconds expires in</returns>
        public static int TTL(byte[] securityToken, int expiresIn = 30, int size = 6)
        {
            if (securityToken == null)
            {
                throw new ArgumentNullException(nameof(securityToken));
            }

            return new Totp(securityToken, expiresIn, totpSize: size).RemainingSeconds();
        }

        /// <summary>
        /// Validates the code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token for verifying.</param>
        /// <param name="code">The code to validate.</param>
        /// <param name="expiresIn">expiresIn, in seconds</param>
        /// <param name="size">return  code size</param>
        /// <returns><c>True</c> if validate succeed, otherwise, <c>false</c>.</returns>
        public static bool ValidateCode(byte[] securityToken, string code, int expiresIn = 30, int size = 6)
        {
            if (securityToken == null)
            {
                throw new ArgumentNullException(nameof(securityToken));
            }
            if (string.IsNullOrWhiteSpace(code) || code.Trim().Length != size)
            {
                return false;
            }

            var validateResult =
                new Totp(securityToken, expiresIn, totpSize: size).VerifyTotp(code, out var timeStepMatched);
            return validateResult && timeStepMatched > 0;
        }

        /// <summary>
        /// Generates code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token to generate code.</param>
        /// <param name="expiresIn">expiresIn, in seconds</param>
        /// <param name="size">return  code size</param>
        /// <returns>The generated code.</returns>
        public static string GenerateCode(string securityToken, int expiresIn = 30, int size = 6) => GenerateCode(Encoding.Unicode.GetBytes(securityToken), expiresIn, size);

        /// <summary>
        /// ttl of the code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token to generate code.</param>
        /// <param name="expiresIn">expiresIn, in seconds</param>
        /// <param name="size">return  code size</param>
        /// <returns>the code remaining seconds expires in</returns>
        public static int TTL(string securityToken, int expiresIn = 30, int size = 6) => TTL(Encoding.UTF8.GetBytes(securityToken), expiresIn, size);

        /// <summary>
        /// Validates the code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token for verifying.</param>
        /// <param name="code">The code to validate.</param>
        /// <param name="expiresIn">expiresIn, in seconds</param>
        /// <param name="size">return  code size</param>
        /// <returns><c>True</c> if validate succeed, otherwise, <c>false</c>.</returns>
        public static bool ValidateCode(string securityToken, string code, int expiresIn = 30, int size = 6) => ValidateCode(Encoding.Unicode.GetBytes(securityToken), code, expiresIn, size);
    }
}
