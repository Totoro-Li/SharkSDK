using System;
namespace SharkSDK
{
    /// <summary>
    /// SharkError class to notify about errors
    /// </summary>
    public class SharkError : Exception
    {
        /// <summary>
        /// exit code returned from low level API
        /// </summary>
        public int exit_code;
        public SharkError (int code) : base (String.Format ("{0}:{1}", Enum.GetName (typeof (SharkExitCodes), code), code))
        {
            exit_code = code;
        }
    }
}
