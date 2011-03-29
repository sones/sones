using System;
using System.IO;
namespace sones.GraphFS.Definitions
{
    /// <summary>
    /// This struct represents the filesystem definition for a stream
    /// </summary>
    public sealed class StreamAddDefinition
    {
        #region data

        /// <summary>
        /// The stream that should be added to the filesystem
        /// </summary>
        public readonly Stream Stream;

        /// <summary>
        /// The id of the stream
        /// </summary>
        public readonly Int64 PropertyID;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new stream definition
        /// </summary>
        /// <param name="myPropertyID">The id of the stream</param>
        /// <param name="myStream">The stream that should be added to the filesystem</param>
        public StreamAddDefinition(
            Int64 myPropertyID,
            Stream myStream)
        {
            Stream = myStream;
            PropertyID = myPropertyID;
        }

        #endregion
    }
}