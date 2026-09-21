using OpenTelemetry;
using System.Diagnostics;

namespace BuildingBlocks.OpenTelemetry.OpenTelemetryProcessors
{
    public class FilterHangfireQueriesProcessor : BaseProcessor<Activity>
    {
        public override void OnEnd(Activity activity)
        {
            var dbQuery = activity.GetTagItem("db.query.text") as string;

            if (dbQuery != null && dbQuery.Contains("hangfire", StringComparison.OrdinalIgnoreCase))
            {
                activity.IsAllDataRequested = false;
                activity.ActivityTraceFlags = ActivityTraceFlags.None;
                return;
            }
            base.OnEnd(activity);
        }
    }
}
