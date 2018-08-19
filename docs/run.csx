
using System.Net;
using System.Configuration;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    var wwwroot = Environment.GetEnvironmentVariable("WEBROOT_PATH");

    var filepath = $"{wwwroot}\\.azurefunctions\\swagger\\swagger.json";
    log.Info($"{filepath}");
    if (!File.Exists(filepath))
    {
      return req.CreateResponse(HttpStatusCode.NotFound);
    }

    var reader = File.OpenText(filepath);
    var stream = await reader.ReadToEndAsync().ConfigureAwait(false);
    return req.CreateResponse(HttpStatusCode.OK, stream);

}

