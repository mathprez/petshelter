using System.Net;

namespace petshelterApi.Helpers
{
    public class CommandResult
    {
        public CommandResult(HttpStatusCode statusCode, string statusText) //object?
        {
            StatusCode = statusCode;
            StatusText = statusText;
        }

        public CommandResult(HttpStatusCode statusCode, string statusText, object resourceId) //object?
        {
            StatusCode = statusCode;
            StatusText = statusText;
            ResourceId = resourceId;
        }

        public CommandResult(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
            StatusText = string.Empty;
        }

        public object ResourceId { get; set; }

        public HttpStatusCode StatusCode { get; set; }
        public string StatusText { get; set; }

        public static implicit operator bool(CommandResult result)
        {
            return result.StatusCode == HttpStatusCode.OK;
        }
    }
}
