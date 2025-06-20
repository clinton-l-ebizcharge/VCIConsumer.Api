namespace VCIConsumer.Api.Models.Query;

public class CustomerListQuery
{
    /// <summary>
    /// Sort by `created_at` or `name`. Prefix with "-" for descending.
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// Number of records to return per page.
    /// </summary>
    public string? Limit_Per_Page { get; set; }

    /// <summary>
    /// Which page of results to return (starting from 1).
    /// </summary>
    public string? Page_Number { get; set; }
}

