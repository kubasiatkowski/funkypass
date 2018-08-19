using System.Net;
using System.Net.Http.Headers;
using System.IO;

public static HttpResponseMessage Run(HttpRequestMessage req, TraceWriter log)
{

    var wwwroot = Environment.GetEnvironmentVariable("WEBROOT_PATH");
    var filepath = $"{wwwroot}\\.azurefunctions\\swagger\\swagger.json";
    log.Info($"{filepath}");
    if (!File.Exists(filepath))
    {
        return req.CreateResponse(HttpStatusCode.NotFound);
    }

    using (var reader = File.OpenText(filepath))
    {
        var stream = await reader.ReadToEndAsync().ConfigureAwait(false);
        response.Content = new StreamContent(stream);
        response.Content.Headers.ContentType = 
            new MediaTypeHeaderValue("application/json");
        return response;
    }
}