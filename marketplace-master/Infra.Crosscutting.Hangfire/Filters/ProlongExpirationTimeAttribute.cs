using Hangfire.States;
using Hangfire.Storage;
using System;

namespace Hangfire.Common
{
    /// <summary>
    /// Filter responsible for determining the expiration period of a completed job.
    /// </summary>
    public class ProlongExpirationTimeAttribute : JobFilterAttribute, IApplyStateFilter
    {
        private readonly TimeSpan _deleteAfter = TimeSpan.FromDays(ConfigHelper.GetSetting("HANGFIRE:JOB:EXPIRATION:DAYS", 7));

        public ProlongExpirationTimeAttribute()
        {

        }

        public ProlongExpirationTimeAttribute(int days, int hours, int minutes, int seconds = 0)
        {
            _deleteAfter = new TimeSpan(days, hours, minutes, seconds);
        }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = _deleteAfter;
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = _deleteAfter;
        }
    }
}