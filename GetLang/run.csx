using System.Net;
using Microsoft.Azure.Services.AppAuthentication;
using System.Data.SqlClient;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

     var tokenProvider = new AzureServiceTokenProvider();
     string accessToken = await tokenProvider.GetAccessTokenAsync("https://database.windows.net/");
     log.Info($"accessToken: {accessToken}");

    
   /* var str  = WebConfigurationManager.ConnectionStrings["connstring"].ConnectionString;
    
    using (SqlConnection conn = new SqlConnection(str))
    {
        conn.AccessToken = accessToken;
        conn.Open();
        var sqlquery = "SELECT DISTINCT [lang] FROM [dbo].[words]";
    
        SqlCommand cmd = new SqlCommand(sqlquery, conn))
        SqlDataReader reader = cmd.ExecuteReader())
        while (reader.Read())
        {
            log.Info($"{reader.GetString(0)}");
        }         
    }*/
            return req.CreateResponse(HttpStatusCode.OK);

}
