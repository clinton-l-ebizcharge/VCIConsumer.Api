namespace VCIConsumer.Api.Endpoints;

public class ServiceEndpoints1
{
    public static void Map(WebApplication app)
    {
        //------------------------------------------------------------------------------------------
        app.MapGet("/", () => "EBizCharge!");

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        //app.MapPost("/transactions/search", ([FromHeader(Name = "SecurityId")] string securityId, [FromBody()] SearchRequest request, IEBizService eBizService) =>
        //    {
        //        try
        //        {
        //            SecurityToken securityToken = new SecurityToken { SecurityId = securityId };

        //            var response = eBizService.SearchTransactions(securityToken, request?.Filters?.ToArray() ?? [], true, request.CountOnly, request.Start.ToString(), request.Limit.ToString(), request.Sort);

        //            return response is null ? Results.NotFound("Not Found") : TypedResults.Ok(response);
        //        }
        //        catch (Exception ex) { return TypedResults.BadRequest("Error: " + ex.Message); }

        //    })
        //    .Produces<SearchTransactionListResponse>()
        //    .Produces(StatusCodes.Status404NotFound)
        //    .Produces(StatusCodes.Status400BadRequest)
        //    .WithName("Search Transactions")
        //    .WithOpenApi();
    }
}
