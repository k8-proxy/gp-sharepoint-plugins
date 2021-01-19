using Glasswall.AspNet.O365.Filehandler;

namespace Glasswall.AspNet.O365.Filehandler.Models
{
    public class AsyncActionModel
    {
        public string JobIdentifier { get; set; }

        public JobStatus Status { get; set; }

        public string Title { get; set; }

    }
}