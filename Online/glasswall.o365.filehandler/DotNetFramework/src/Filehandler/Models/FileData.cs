using System.IO;

namespace Glasswall.AspNet.O365.Filehandler.Models
{

    public class FileData
    {
        public string Content { get; set; }

        public Stream FileStream { get; set; }

        public string Filename { get; set; }
    }
}