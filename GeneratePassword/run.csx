using System.Net;
using Microsoft.Azure.Services.AppAuthentication;
using System.Configuration;
using System.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
//using System.Web.Configuration;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");
    int minlen = 10;
    int maxlen = 20;
    int curlen = 0;
    // parse query parameter
    string lang = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "lang", true) == 0)
        .Value;

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    lang = lang ?? data?.lang;

     log.Info($"Selected language {lang}");

     var tokenProvider = new AzureServiceTokenProvider();
     string accessToken = await tokenProvider.GetAccessTokenAsync("https://database.windows.net/");
     log.Info($"accessToken: {accessToken}");


    var connStr  = ConfigurationManager.ConnectionStrings["connstring"].ConnectionString;
    log.Info($"connectionString: {connStr}");
    var languages = new List<Language>();

    using (SqlConnection conn = new SqlConnection(connStr))
    {
        conn.AccessToken = accessToken;
        conn.Open();
        var sqlquery = @"SELECT [langcode]
                        ,[langname]
                        ,[maxid]
                        FROM [dbo].[dictionaries]";
    
        SqlCommand cmd = new SqlCommand(sqlquery, conn);
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
           // string sqlread = reader.GetString(0);
           // log.Info($"{sqlread}");
            //languages.Add(sqlread);
            Language l = new Language();
            l.langcode = reader.GetString(0);
            l.langname = reader.GetString(1);
            l.dictionarysize = reader.GetInt32(2);
            languages.Add(l);
            log.Info($"{l}");
        }         
    }

    Language selllang = new Language();
    //var selectedlanguage = 

    var result = from lan in languages
            where lan.langcode == lang
            select lan;
    selllang = result.FirstOrDefault();


    if (selllang == null || lang == "Random")
    {
        Random rnd = new Random();
        int r = rnd.Next(languages.Count);
        selllang = languages[r];
    }
    //log.Info($"{selectedlanguage.langname}");
    var words = new List<string>();
    while (curlen < minlen)
    {
    using (SqlConnection conn = new SqlConnection(connStr))
    {
        conn.AccessToken = accessToken;
        conn.Open();
        Random r = new Random();
        r.Next(selllang.dictionarysize);

        var sqlquery = "SELECT TOP 1 word FROM words_"+selllang.langcode+" WHERE id >" + r;
    
        SqlCommand cmd = new SqlCommand(sqlquery, conn);
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
           // string sqlread = reader.GetString(0);
           // log.Info($"{sqlread}");
            //languages.Add(sqlread);
            words.Add(reader.GetString(0));
            curlen += (reader.GetString(0)).Length;
        }         
    }

    }
      return req.CreateResponse(HttpStatusCode.OK, words);
      //  return req.CreateResponse(HttpStatusCode.OK, selllang);
}


public class Language
{
    public string langcode;
    public string langname;
    public int dictionarysize;
}