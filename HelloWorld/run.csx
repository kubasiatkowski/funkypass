using System.Net;
using Microsoft.Azure.Services.AppAuthentication;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

     var tokenProvider = new AzureServiceTokenProvider();
     string accessToken = await tokenProvider.GetAccessTokenAsync("https://database.windows.net/");
     log.Info($"accessToken: {accessToken}");


    
    // parse query parameter
    string name = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
        .Value;

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    name = name ?? data?.name;

    if (name == null)
        return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body");
    else
    {
        Test t = new Test();
        t.name = name;
        return req.CreateResponse(HttpStatusCode.OK, t); //"Hello " + name)
    };
}


public class Test
{
    public string name;
}
