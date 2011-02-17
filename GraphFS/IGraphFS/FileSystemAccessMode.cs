namespace sones.GraphFS
{
    /// <summary>
    /// The different access modes (read/write, read-only, ...)
    /// </summary>
    public enum FileSystemAccessMode
    {
        /// <summary>
        /// The filesystem should be opened for reading and writing 
        /// </summary>
        ReadWrite,

        /// <summary>
        /// The filesystem will be opened only for appending data
        /// </summary>
        AppendOnly,

        /// <summary>
        /// The filesystem will be opened only for reading
        /// </summary>
        ReadOnly
    }
}