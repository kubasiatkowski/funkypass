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

    string specialchars ="!@#$%^&*()-_=+[{]}\\|;:\"',<.>/?";

    // parse query parameteru
    string lang = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "lang", true) == 0)
        .Value;
    int minlen;
    int.TryParse (req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "minlen", true) == 0)
        .Value, out minlen);
    int maxlen;
    int.TryParse(req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "maxlen", true) == 0)
        .Value, out maxlen); 
    string asciionly = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "asciionly", true) == 0)
        .Value;
    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    lang = (lang ?? data?.lang) ?? "Random";


    if (minlen==0)
        minlen =  data?.minlen ?? 14;
    if (maxlen==0)    
        maxlen = data?.maxlen ?? 32; 
 
    asciionly = (asciionly ?? data?.asciionly) ?? "True";

    bool ascii;
    bool.TryParse(asciionly,out ascii);


    log.Info($"Language {lang}");
    log.Info($"Minlen {minlen}");
    log.Info($"Maxlen {maxlen}");
    log.Info($"ASCIIonly {ascii}");


    int curlen = 0;
    Random rnd = new Random();

     var tokenProvider = new AzureServiceTokenProvider();
     string accessToken = await tokenProvider.GetAccessTokenAsync("https://database.windows.net/");
     //log.Info($"accessToken: {accessToken}");


    var connStr  = ConfigurationManager.ConnectionStrings["connstring"].ConnectionString;
    //log.Info($"connectionString: {connStr}");
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
        int r = rnd.Next(languages.Count);
        selllang = languages[r];
    }
    //log.Info($"{selectedlanguage.langname}");
    var words = new List<string>();
    words.Add(specialchars[rnd.Next(0,specialchars.Length)].ToString());
    curlen ++;
   
    string word = rnd.Next(0,1000).ToString();
    words.Add(word);
    curlen += word.Length;

    while (curlen < minlen)
    {
    using (SqlConnection conn = new SqlConnection(connStr))
    {
        conn.AccessToken = accessToken;
        conn.Open();

        int id = rnd.Next(selllang.dictionarysize);

        var sqlquery = "SELECT TOP 1 word FROM words_"+selllang.langcode+" WHERE id >" + id + " ORDER BY id";
         log.Info($"{sqlquery}");
        SqlCommand cmd = new SqlCommand(sqlquery, conn);
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
           // string sqlread = reader.GetString(0);
           // log.Info($"{sqlread}");
            //languages.Add(sqlread);
            word =reader.GetString(0);
            if(curlen + word.Length > maxlen)
            {
                word = word.Remove(maxlen-curlen);
            }
            words.Add(word);
            curlen += (word).Length;
        } 

        if (curlen>=minlen)
        {
            break;
        }

        if(rnd.NextDouble() > 0.5)
        {
            words.Add(specialchars[rnd.Next(0,specialchars.Length)].ToString());
            curlen ++;
        }
        else{
            num = rnd.Next(0,100).ToString();
            words.Add(num);
            curlen += num.Length;
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