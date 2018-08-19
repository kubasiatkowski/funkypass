
using System.Net;
using System.Configuration;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info (Environment.GetEnvironmentVariable("WEBROOT_PATH"));
    log.Info (Environment.GetEnvironmentVariable("HOME"));


    var filepath = $".azurefunctions\\swagger\\swagger.json";
    log.Info($"{filepath}");
    if (!File.Exists(filepath))
    {
      log.Info($"File not found");
      return req.CreateResponse(HttpStatusCode.NotFound);
    }

    var reader = File.OpenText(filepath);
    var stream = await reader.ReadToEndAsync().ConfigureAwait(false);
    return req.CreateResponse(HttpStatusCode.OK, stream);

}

