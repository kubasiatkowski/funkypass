
using System.Net;
using System.Configuration;

public static class GetSwaggerHttpTrigger
{
  public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, string version, TraceWriter log)
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
}