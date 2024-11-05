namespace TodoApp.Api.Exceptions
{
    public class UserFriendlyException : Exception
    {
        public int StatusCode { get; }

        public UserFriendlyException(string message, int statusCode = StatusCodes.Status400BadRequest)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
