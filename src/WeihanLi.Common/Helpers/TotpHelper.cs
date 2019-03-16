// Copyright (c) Arch team. All rights reserved.

using System;
using WeihanLi.Common.Otp;

namespace WeihanLi.Common.Helpers
{
    public class TotpHelper
    {
        /// <summary>
        /// Generates code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token to generate code.</param>
        /// <param name="size">return  code size</param>
        /// <returns>The generated code.</returns>
        public static string GenerateCode(byte[] securityToken, int size = 6)
        {
            if (securityToken == null)
            {
                throw new ArgumentNullException(nameof(securityToken));
            }

            return new Totp(securityToken, totpSize: size).ComputeTotp();
        }

        /// <summary>
        /// ttl of the code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token to generate code.</param>
        /// <param name="expiresIn">expiresIn, in seconds</param>
        /// <param name="size">return  code size</param>
        /// <returns>the code remaining seconds expires in</returns>
        public static int TTL(byte[] securityToken, int size = 6)
        {
            if (securityToken == null)
            {
                throw new ArgumentNullException(nameof(securityToken));
            }

            return new Totp(securityToken, totpSize: size).RemainingSeconds();
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
            var factor = 1;
            if (expiresIn > 30)
            {
                factor = expiresIn / 30;
            }
            factor -= 1;
            var validateResult =
                new Totp(securityToken, totpSize: size)
                    .VerifyTotp(code, out var timeStepMatched, new VerificationWindow(factor, factor));
            return validateResult;
        }

        /// <summary>
        /// Generates code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token to generate code.</param>
        /// <param name="size">return  code size</param>
        /// <returns>The generated code.</returns>
        public static string GenerateCode(string securityToken, int size = 6) => GenerateCode(System.Text.Encoding.UTF8.GetBytes(securityToken), size);

        /// <summary>
        /// ttl of the code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token to generate code.</param>
        /// <param name="expiresIn">expiresIn, in seconds</param>
        /// <param name="size">return  code size</param>
        /// <returns>the code remaining seconds expires in</returns>
        public static int TTL(string securityToken, int size = 6) => TTL(System.Text.Encoding.UTF8.GetBytes(securityToken), size);

        /// <summary>
        /// Validates the code for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token for verifying.</param>
        /// <param name="code">The code to validate.</param>
        /// <param name="expiresIn">expiresIn, in seconds</param>
        /// <param name="size">return  code size</param>
        /// <returns><c>True</c> if validate succeed, otherwise, <c>false</c>.</returns>
        public static bool ValidateCode(string securityToken, string code, int expiresIn = 30, int size = 6) => ValidateCode(System.Text.Encoding.UTF8.GetBytes(securityToken), code, expiresIn, size);
    }
}
