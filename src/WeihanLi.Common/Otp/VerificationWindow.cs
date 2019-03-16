using System.Collections.Generic;

namespace WeihanLi.Common.Otp
{
    /// <summary>
    /// A verification window
    /// </summary>
    internal class VerificationWindow
    {
        private readonly int previous;
        private readonly int future;

        /// <summary>
        /// Create an instance of a verification window
        /// </summary>
        /// <param name="previous">The number of previous frames to accept</param>
        /// <param name="future">The number of future frames to accept</param>
        public VerificationWindow(int previous = 0, int future = 0)
        {
            this.previous = previous;
            this.future = future;
        }

        /// <summary>
        /// Gets an enumberable of all the possible validation candidates
        /// </summary>
        /// <param name="initialFrame">The initial frame to validate</param>
        /// <returns>Enumberable of all possible frames that need to be validated</returns>
        public IEnumerable<long> ValidationCandidates(long initialFrame)
        {
            yield return initialFrame;
            for (int i = 1; i <= previous; i++)
            {
                var val = initialFrame - i;
                if (val < 0)
                    break;
                yield return val;
            }

            for (int i = 1; i <= future; i++)
                yield return initialFrame + i;
        }

        /// <summary>
        /// The verification window that accomodates network delay that is recommended in the RFC
        /// </summary>
        public static readonly VerificationWindow RfcSpecifiedNetworkDelay = new VerificationWindow(previous: 1, future: 1);
    }
}
