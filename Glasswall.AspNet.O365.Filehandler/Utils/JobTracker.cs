using System;
using System.Collections.Generic;

namespace Glasswall.AspNet.O365.Filehandler
{
    public static class JobTracker
    {
        public static Dictionary<string, JobStatus> TrackedJobs = new Dictionary<string, JobStatus>();

        public static string QueueJob(JobStatus status)
        {
            string id = Guid.NewGuid().ToString("b");
            TrackedJobs[id] = status;
            status.Id = id;

            return id;
        }

        public static JobStatus GetJob(string id)
        {
            JobStatus value;
            if (TrackedJobs.TryGetValue(id, out value))
            {
                return value;
            }

            return null;
        }

        public static void Remove(string id)
        {
            TrackedJobs.Remove(id);
        }
    }


    public class JobStatus
    {
        public JobState State { get; set; }
        public Exception Error { get; set; }
        public string Id { get; internal set; }
        public string ResultWebUrl { get; internal set; }
        public Dictionary<string, string> OriginalParameters { get; set; }
    }

    public enum JobState
    {
        NotStarted,
        Running,
        Complete,
        Error
    }
}