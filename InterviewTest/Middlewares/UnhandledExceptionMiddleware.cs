namespace InterviewTest.API.Middlewares
{
    public class UnhandledExceptionMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public UnhandledExceptionMiddleware(ILogger<UnhandledExceptionMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    $"Request {context.Request?.Method}: {context.Request?.Path.Value} failed");
            }
        }
    }
}
