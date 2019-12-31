using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace WeihanLi.Common.Helpers
{
    /// <summary>
    /// IPNetwork
    /// https://source.dot.net/#Microsoft.AspNetCore.HttpOverrides/IPNetwork.cs,ab4d458482303384
    /// </summary>
    public class IPNetwork
    {
        public IPNetwork(string cidr)
        {
            if (string.IsNullOrWhiteSpace(cidr))
            {
                throw new ArgumentNullException(nameof(cidr));
            }
            var arr = cidr.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length == 0 || arr.Length > 2)
            {
                throw new ArgumentException("invalid cidr format", nameof(cidr));
            }
            if (!IPAddress.TryParse(arr[0], out var ipAddress))
            {
                throw new ArgumentException("invalid ip format", nameof(cidr));
            }

            PrefixLength = 0;
            if (arr.Length == 2)
            {
                if (!int.TryParse(arr[1], out var prefixLength))
                {
                    throw new ArgumentException("invalid cidr format", nameof(cidr));
                }
                PrefixLength = prefixLength;
            }

            Prefix = ipAddress;
            PrefixBytes = Prefix.GetAddressBytes();
            Mask = CreateMask();
        }

        public IPNetwork(string ipPrefix, int prefixLength) : this(IPAddress.Parse(ipPrefix), prefixLength)
        {
        }

        public IPNetwork(IPAddress prefix, int prefixLength)
        {
            Prefix = prefix;
            PrefixLength = prefixLength;
            PrefixBytes = Prefix.GetAddressBytes();
            Mask = CreateMask();
        }

        public IPAddress Prefix { get; }

        private byte[] PrefixBytes { get; }

        /// <summary>
        /// The CIDR notation of the subnet mask
        /// </summary>
        public int PrefixLength { get; }

        private byte[] Mask { get; }

        public bool Contains(IPAddress address)
        {
            if (Prefix.AddressFamily != address.AddressFamily)
            {
                return false;
            }

            var addressBytes = address.GetAddressBytes();
            for (var i = 0; i < PrefixBytes.Length && Mask[i] != 0; i++)
            {
                if (PrefixBytes[i] != (addressBytes[i] & Mask[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private byte[] CreateMask()
        {
            var mask = new byte[PrefixBytes.Length];
            var remainingBits = PrefixLength;
            var i = 0;
            while (remainingBits >= 8)
            {
                mask[i] = 0xFF;
                i++;
                remainingBits -= 8;
            }
            if (remainingBits > 0)
            {
                mask[i] = (byte)(0xFF << (8 - remainingBits));
            }

            return mask;
        }
    }

    public static class NetHelper
    {
        /// <summary>
        /// get a random port not used
        /// </summary>
        /// <param name="min">min port, 10000 by default</param>
        /// <param name="max">max, 65535</param>
        /// <returns></returns>
        public static int GetRandomPort(int min = 10240, int max = 65535)
        {
            if (min < 1024 || min >= 65535)
            {
                min = 10240;
            }
            if (max < min || max > 65535)
            {
                max = 65535;
            }

            int randomPort;
            do
            {
                randomPort = SecurityHelper.Random.Next(min, max);
            } while (IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Any(p => p.Port == randomPort));
            return randomPort;
        }

        private static readonly Lazy<IPNetwork> PrivateAddressBlockANetwork = new Lazy<IPNetwork>(() => new IPNetwork("10.0.0.0/8"));
        private static readonly Lazy<IPNetwork> PrivateAddressBlockBNetwork = new Lazy<IPNetwork>(() => new IPNetwork("172.16.0.0/12"));
        private static readonly Lazy<IPNetwork> PrivateAddressBlockCNetwork = new Lazy<IPNetwork>(() => new IPNetwork("192.168.0.0/16"));

        /// <summary>
        /// whether the ip is a private ip
        /// </summary>
        /// <param name="ip">ipAddress</param>
        /// <returns></returns>
        public static bool IsPrivateIP(string ip) => IsPrivateIP(IPAddress.Parse(ip));

        /// <summary>
        /// whether the ip is a private ip
        /// </summary>
        /// <param name="ipAddress">ipAddress</param>
        /// <returns></returns>
        public static bool IsPrivateIP(IPAddress ipAddress)
        {
            return PrivateAddressBlockANetwork.Value.Contains(ipAddress)
                   || PrivateAddressBlockBNetwork.Value.Contains(ipAddress)
                   || PrivateAddressBlockCNetwork.Value.Contains(ipAddress)
                ;
        }
    }
}
