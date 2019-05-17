using System;
using System.Security.Cryptography;
using System.Text;

namespace WeihanLi.Common.Otp
{
    public class Totp
    {
        private readonly OtpHashAlgorithm _hashAlgorithm;
        private readonly int _codeSize;

        public Totp() : this(OtpHashAlgorithm.SHA1, 6)
        {
        }

        public Totp(int codeSize) : this(OtpHashAlgorithm.SHA1, codeSize)
        {
        }

        public Totp(OtpHashAlgorithm hashAlgorithm) : this(hashAlgorithm, 6)
        {
        }

        public Totp(OtpHashAlgorithm otpHashAlgorithm, int codeSize)
        {
            _hashAlgorithm = otpHashAlgorithm;

            // valid input parameter
            if (codeSize <= 0 || codeSize > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(codeSize), codeSize, "length must between 1 and 9");
            }
            _codeSize = codeSize;
        }

        private static readonly Encoding Encoding = new UTF8Encoding(false, true);

        public virtual string Compute(string securityToken) => Compute(Encoding.GetBytes(securityToken));

        public virtual string Compute(byte[] securityToken) => Compute(securityToken, GetCurrentTimeStepNumber());

        private string Compute(byte[] securityToken, long counter)
        {
            HMAC hmac;
            switch (_hashAlgorithm)
            {
                case OtpHashAlgorithm.SHA1:
                    hmac = new HMACSHA1(securityToken);
                    break;

                case OtpHashAlgorithm.SHA256:
                    hmac = new HMACSHA256(securityToken);
                    break;

                case OtpHashAlgorithm.SHA512:
                    hmac = new HMACSHA512(securityToken);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_hashAlgorithm), _hashAlgorithm, null);
            }

            using (hmac)
            {
                var stepBytes = BitConverter.GetBytes(counter);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(stepBytes); // need BigEndian
                }
                // See https://tools.ietf.org/html/rfc4226
                var hashResult = hmac.ComputeHash(stepBytes);

                var offset = hashResult[hashResult.Length - 1] & 0xf;
                var p = "";
                for (var i = 0; i < 4; i++)
                {
                    p += hashResult[offset + i].ToString("X2");
                }
                var num = Convert.ToInt64(p, 16) & 0x7FFFFFFF;

                //var binaryCode = (hashResult[offset] & 0x7f) << 24
                //                 | (hashResult[offset + 1] & 0xff) << 16
                //                 | (hashResult[offset + 2] & 0xff) << 8
                //                 | (hashResult[offset + 3] & 0xff);

                return (num % (int)Math.Pow(10, _codeSize)).ToString();
            }
        }

        public virtual bool Verify(string securityToken, string code) => Verify(Encoding.GetBytes(securityToken), code);

        public virtual bool Verify(string securityToken, string code, TimeSpan timeToleration) => Verify(Encoding.GetBytes(securityToken), code, timeToleration);

        public virtual bool Verify(byte[] securityToken, string code) => Verify(securityToken, code, TimeSpan.Zero);

        public virtual bool Verify(byte[] securityToken, string code, TimeSpan timeToleration)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;
            if (code.Length != _codeSize)
                return false;

            var futureStep = (int)(timeToleration.TotalSeconds / 30);
            var step = GetCurrentTimeStepNumber();
            for (int i = -futureStep; i <= futureStep; i++)
            {
                if (step + i < 0)
                {
                    continue;
                }
                var totp = Compute(securityToken, step + i);
                if (totp == code)
                {
                    return true;
                }
            }
            return false;
        }

        public int RemainingSeconds()
        {
            return (int)(_timeStepTicks - ((DateTime.UtcNow - _unixEpoch).Ticks / TimeSpan.TicksPerSecond) % _timeStepTicks);
        }

        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// time step
        /// 30s(Recommend)
        /// </summary>
        private const long _timeStepTicks = TimeSpan.TicksPerSecond * 30;

        // More info: https://tools.ietf.org/html/rfc6238#section-4
        private static long GetCurrentTimeStepNumber() => (DateTime.UtcNow - _unixEpoch).Ticks / _timeStepTicks;
    }
}
