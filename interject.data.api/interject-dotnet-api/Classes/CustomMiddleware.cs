namespace Interject.DataApi;

public class CustomMiddleware
{
    private readonly RequestDelegate _next;

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    public CustomMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            // Create a log object to hold the request details
            var log = new
            {
                Protocol = context.Request.Protocol,
                PathBase = context.Request.PathBase.ToString(),
                Cookies = context.Request.Cookies,
                ContentType = context.Request.ContentType,
                ContentLength = context.Request.ContentLength,
                Method = context.Request.Method,
                Scheme = context.Request.Scheme,
                Host = context.Request.Host.ToString(),
                Path = context.Request.Path.ToString(),
                QueryString = context.Request.QueryString.ToString(),
                Headers = context.Request.Headers,
                Body = await ReadRequestBodyAsync(context.Request)
            };

            await SaveFileAsync($"Log_{DateTime.Now:yyyyMMdd_HHmmss}.json", log);

            // Call the next middleware in the pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the exception
            await LogExceptionAsync(ex);
        }
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering(); // Enable buffering to allow the request body to be read multiple times

        using (var reader = new StreamReader(request.Body, leaveOpen: true))
        {
            string body = await reader.ReadToEndAsync();
            request.Body.Position = 0; // Reset the request body stream position for the next middleware
            return body;
        }
    }

    private async Task LogExceptionAsync(Exception ex)
    {
        // Create an exception log object
        var log = new
        {
            Message = ex.Message,
            StackTrace = ex.StackTrace,
            InnerException = ex.InnerException
        };

        await SaveFileAsync($"Exception_{DateTime.Now:yyyyMMdd_HHmmss}.json", log);
    }

    private async Task SaveFileAsync(string fileName, object obj)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Logs", fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        string json = JsonSerializer.Serialize(obj, _options);
        await File.WriteAllTextAsync(path, json);
    }
}