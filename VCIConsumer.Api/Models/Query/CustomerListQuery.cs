using Microsoft.AspNetCore.Mvc;

namespace VCIConsumer.Api.Models.Query
{
    public class CustomerListQuery
    {
        [FromQuery(Name = "sort")]
        public string? Sort { get; set; }

        [FromQuery(Name = "limit_per_page")]
        public int? LimitPerPage { get; set; }

        [FromQuery(Name = "page_number")]
        public int? PageNumber { get; set; }
    }
}
