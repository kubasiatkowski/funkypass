
using System.Net;
using System.Configuration;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info (Environment.GetEnvironmentVariable("WEBROOT_PATH"));
   
    var home = Environment.GetEnvironmentVariable("HOME");
    log.Info (home);

    var filepath = $"{home}\\site\\wwwroot\\.azurefunctions\\swagger\\swagger.json";
    log.Info($"{filepath}");
    if (!File.Exists(filepath))
    {
      log.Info($"File not found");
      return req.CreateResponse(HttpStatusCode.NotFound);
    }

    var reader = File.OpenText(filepath);
    var stream = await reader.ReadToEndAsync().ConfigureAwait(false);
    var deserialiser = new DeserializerBuilder().Build();
    var deserialised = deserialiser.Deserialize<dynamic>(stream);
    return req.CreateResponse(HttpStatusCode.OK, (object)deserialised,"application/json");

}

