using System;

namespace WeihanLi.Common.Otp
{
    /// <summary>
    /// Class to apply a correction factor to the system time
    /// </summary>
    /// <remarks>
    /// In cases where the local system time is incorrect it is preferable to simply correct the system time.
    /// This class is provided to handle cases where it isn't possible for the client, the server, or both, to be on the correct time.
    ///
    /// This library provides limited facilities to to ping NIST for a correct network time.  This class can be used manually however in cases where a server's time is off
    /// and the consumer of this library can't control it.  In that case create an instance of this class and provide the current server time as the correct time parameter
    ///
    /// This class is immutable and therefore threadsafe
    /// </remarks>
    internal class TimeCorrection
    {
        /// <summary>
        /// An instance that provides no correction factor
        /// </summary>
        public static readonly TimeCorrection UncorrectedInstance = new TimeCorrection();

        private readonly TimeSpan timeCorrectionFactor;

        /// <summary>
        /// Constructor used solely for the UncorrectedInstance static field to provide an instance without a correction factor.
        /// </summary>
        private TimeCorrection()
        {
            this.timeCorrectionFactor = TimeSpan.Zero;
        }

        /// <summary>
        /// Creates a corrected time object by providing the known correct current UTC time.  The current system UTC time will be used as the reference
        /// </summary>
        /// <remarks>
        /// This overload assumes UTC.  If a base and reference time other than UTC are required then use the other overlaod.
        /// </remarks>
        /// <param name="correctUtc">The current correct UTC time</param>
        public TimeCorrection(DateTime correctUtc)
        {
            this.timeCorrectionFactor = DateTime.UtcNow - correctUtc;
        }

        /// <summary>
        /// Creates a corrected time object by providing the known correct current time and the current reference time that needs correction
        /// </summary>
        /// <param name="correctTime">The current correct time</param>
        /// <param name="referenceTime">The current reference time (time that will have the correction factor applied in subsequent calls)</param>
        public TimeCorrection(DateTime correctTime, DateTime referenceTime)
        {
            this.timeCorrectionFactor = referenceTime - correctTime;
        }

        /// <summary>
        /// Applies the correction factor to the reference time and returns a corrected time
        /// </summary>
        /// <param name="referenceTime">The reference time</param>
        /// <returns>The reference time with the correction factor applied</returns>
        public DateTime GetCorrectedTime(DateTime referenceTime)
        {
            return referenceTime - timeCorrectionFactor;
        }

        /// <summary>
        /// Applies the correction factor to the current system UTC time and returns a corrected time
        /// </summary>
        public DateTime CorrectedUtcNow
        {
            get { return GetCorrectedTime(DateTime.UtcNow); }
        }

        /// <summary>
        /// The timespan that is used to calculate a corrected time
        /// </summary>
        public TimeSpan CorrectionFactor
        {
            get { return this.timeCorrectionFactor; }
        }
    }
}
