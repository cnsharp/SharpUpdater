namespace CnSharp.Updater
{
    public class ReleaseFile
    {
        public string FileName { get; set; }

        //public string ReleaseDate { get; set; }

        /// <summary>
        /// File size in Byte
        /// </summary>
        public long FileSize { get; set; }

        public string Version { get; set; }
    }
}