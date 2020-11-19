using System.Threading.Tasks;
using Glasswall.AspNet.O365.Filehandler.Models;

namespace Glasswall.AspNet.O365.Filehandler
{
    public class AsyncJob
    {
        public JobStatus Status { get; private set; }

        public string Id { get { return Status.Id; } }

        private IAsyncJob Job { get; set; }

        public AsyncJob(IAsyncJob job)
        {
            this.Job = job;
            Status = new JobStatus();
            Status.State = JobState.NotStarted;
            JobTracker.QueueJob(this.Status);
        }

        public async void Begin(string[] sourceUrls, string accessToken = null)
        {
            Status.State = JobState.Running;

            try
            {
                var webUrl = await Job.DoWorkAsync(sourceUrls, accessToken);

                Status.ResultWebUrl = webUrl;
                Status.State = JobState.Complete;
            }
            catch (ConverterException ex)
            {
                Status.State = JobState.Error;
                Status.Error = ex;
            }
        }
    }

    public interface IAsyncJob
    {
        Task<string> DoWorkAsync(string[] sourceUrls, string accessToken);
    }
}