namespace CnSharp.VisualStudio.SharpUpdater
{
    public class FileListItem
    {
        public string Dir { get; set; }
        public bool IsFile { get; set; }
        public bool Selected { get; set; }
        public string RelativeFileName { get; set; }
    }
}
