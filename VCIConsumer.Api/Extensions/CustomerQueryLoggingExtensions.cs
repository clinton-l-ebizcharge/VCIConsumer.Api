namespace VCIConsumer.Api.Extensions;

using Microsoft.Extensions.Logging;
using VCIConsumer.Api.Models.Query;

public static class CustomerQueryLoggingExtensions
{
    public static void LogSummary(this ILogger logger, CustomerListQuery query)
    {
        logger.LogInformation("Fetching customer list with Sort='{Sort}', Limit={Limit}, Page={Page}",
            query.Sort, query.LimitPerPage, query.PageNumber);
    }
}

