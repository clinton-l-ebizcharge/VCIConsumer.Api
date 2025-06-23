using Microsoft.AspNetCore.Mvc;
using VCIConsumer.Api.Models.Enums;

namespace VCIConsumer.Api.Models.Query
{
    public class PaymentListQuery
    {
        /// <summary>
        /// The field by which to sort results.
        /// Valid values: created_at, status, amount, name.
        /// Prefix with '-' for descending order.
        /// </summary>
        public string? Sort { get; set; }

        /// <summary>
        /// Maximum number of results per page. Default is 100.
        /// </summary>
        [FromQuery(Name = "limit_per_page")]
        public int? LimitPerPage { get; set; } = 100;

        /// <summary>
        /// Page number to retrieve. Default is 1.
        /// </summary>
        [FromQuery(Name = "page_number")]
        public int? PageNumber { get; set; } = 1;

        /// <summary>
        /// Payment status filter.
        /// Valid values: ACCEPTED, ERROR, ORIGINATED, SETTLED, PARTIAL SETTLED, VERIFYING, VOID, RETURN, NSF DECLINED.
        /// </summary>
        public PaymentStatus? Status { get; set; }

        /// <summary>
        /// Filter by creation date that is greater than or equal to (YYYY-MM-DD HH:MM:SS).
        /// </summary>
        [FromQuery(Name = "created_at[gte]")]
        public DateTime? CreatedAtGte { get; set; }

        /// <summary>
        /// Filter by creation date that is less than or equal to (YYYY-MM-DD HH:MM:SS).
        /// </summary>
        [FromQuery(Name = "created_at[lte]")]
        public DateTime? CreatedAtLte { get; set; }

        /// <summary>
        /// Filter by originated date that is greater than or equal to (YYYY-MM-DD HH:MM:SS).
        /// </summary>
        [FromQuery(Name = "originated_at[gte]")]
        public DateTime? OriginatedAtGte { get; set; }

        /// <summary>
        /// Filter by originated date that is less than or equal to (YYYY-MM-DD HH:MM:SS).
        /// </summary>
        [FromQuery(Name = "originated_at[lte]")]
        public DateTime? OriginatedAtLte { get; set; }

        /// <summary>
        /// Filter by settled date that is greater than or equal to (YYYY-MM-DD HH:MM:SS).
        /// </summary>
        [FromQuery(Name = "settled_at[gte]")]
        public DateTime? SettledAtGte { get; set; }

        /// <summary>
        /// Filter by settled date that is less than or equal to (YYYY-MM-DD HH:MM:SS).
        /// </summary>
        [FromQuery(Name = "settled_at[lte]")]
        public DateTime? SettledAtLte { get; set; }

        /// <summary>
        /// Filter by returned date that is greater than or equal to (YYYY-MM-DD HH:MM:SS).
        /// </summary>
        [FromQuery(Name = "returned_at[gte]")]
        public DateTime? ReturnedAtGte { get; set; }

        /// <summary>
        /// Filter by returned date that is less than or equal to (YYYY-MM-DD HH:MM:SS).
        /// </summary>
        [FromQuery(Name = "returned_at[lte]")]
        public DateTime? ReturnedAtLte { get; set; }
    }
}
