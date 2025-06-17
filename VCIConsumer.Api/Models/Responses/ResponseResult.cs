namespace VCIConsumer.Api.Models.Response;

public class ResponseResult<T>
{
    public required T Data { get; set; }
    public required string JSON { get; set; }
    public required ErrorMessage[] Errors { get; set; }
    public bool HasError { get { return Errors != null && Errors.Any(); } }
}
