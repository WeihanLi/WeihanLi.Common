using System.Linq;
using System.Net.NetworkInformation;

namespace WeihanLi.Common.Helpers
{
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
    }
}
