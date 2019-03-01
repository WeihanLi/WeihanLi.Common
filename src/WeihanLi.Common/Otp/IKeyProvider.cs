namespace WeihanLi.Common.Otp
{
    /// <summary>
    /// Interface used to interact with a key
    /// </summary>
    internal interface IKeyProvider
    {
        /// <summary>
        /// Uses the key to get an HMAC using the specified algorithm and data
        /// </summary>
        /// <remarks>
        /// This is a much better API than the previous API which would briefly expose the key for all derived types.
        ///
        /// Now a derived type could be bound to an HSM/smart card/etc if required and a lot of the security limitations
        /// of in app/memory exposure of the key can be eliminated.
        /// </remarks>
        /// <param name="mode">The HMAC algorithm to use</param>
        /// <param name="data">The data used to compute the HMAC</param>
        /// <returns>HMAC of the key and data</returns>
        byte[] ComputeHmac(OtpHashMode mode, byte[] data);
    }
}
