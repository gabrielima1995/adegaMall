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
        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = TimeSpan.FromDays(ConfigHelper.GetSetting("HANGFIRE:JOB:EXPIRATION:DAYS", 7));
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = TimeSpan.FromDays(ConfigHelper.GetSetting("HANGFIRE:JOB:EXPIRATION:DAYS", 7));
        }
    }
}