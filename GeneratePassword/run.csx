using System.Net;
using Microsoft.Azure.Services.AppAuthentication;
using System.Configuration;
using System.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using Diacritics.Extensions;
//using System.Web.Configuration;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");
    //int minlen = 14;
    //int maxlen = 20;


    // parse query parameter
    string lang = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "lang", true) == 0)
        .Value;
    int minlen;
    int.TryParse (req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "minlen", true) == 0)
        .Value, out minlen);
   // int maxlen;
   // int.TryParse(req.GetQueryNameValuePairs()
   //     .FirstOrDefault(q => string.Compare(q.Key, "maxlen", true) == 0)
   //     .Value, out maxlen); 
    string sasciionly = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "asciionly", true) == 0)
        .Value;


   


    IEnumerable<KeyValuePair<string, string>> values = req.GetQueryNameValuePairs();

    // Write query parameters to log
    foreach (KeyValuePair<string, string> val in values)
    {
        log.Info($"Parameter: {val.Key}\nValue: {val.Value} \n Type: {val.GetType()} \n");
    }
    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    lang = (lang ?? data?.lang) ?? "Random";
    sasciionly = (sasciionly ?? data?.asciionly) ?? "True";

    if (minlen==0)
        minlen =  data?.minlen ?? 14;
   // if (maxlen==0)    
   //     maxlen = data?.maxlen ?? 20; 
    
    bool asciionly = true;
    bool.TryParse(sasciionly, out asciionly);      


    log.Info($"Language {lang}");
    log.Info($"Minlen {minlen}");
  //  log.Info($"Maxlen {maxlen}");
    log.Info($"ASCIIonly {asciionly}");
    log.Info($"SASCIIonly {sasciionly}");
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
    int tempent = 1;
    double dtempent = 0;
    while (curlen < minlen)
    {
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.AccessToken = accessToken;
            conn.Open();

            int id = rnd.Next(selllang.dictionarysize);

            var sqlquery = "SELECT TOP 1 word FROM words_"+selllang.langcode+" WHERE id >" + id;
            log.Info($"{sqlquery}");
            SqlCommand cmd = new SqlCommand(sqlquery, conn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string sqlread = reader.GetString(0);
            
                int capitalize = rnd.Next(0,4);
                switch (capitalize)
                {
                    case 0:
                        sqlread = sqlread.ToUpper();
                        break;
                    case 1:
                        sqlread = sqlread.ToLower();
                        break;
                    default:
                        sqlread = sqlread.First().ToString().ToUpper() + sqlread.Substring(1).ToLower();
                        break;
                }
                if (!asciionly)
                    words.Add(sqlread);
                else
                    words.Add(sqlread.RemoveDiacritics());
                curlen += (reader.GetString(0)).Length;
                tempent *= selllang.dictionarysize * 3;
                dtempent += Math.Log(selllang.dictionarysize * 3,2);
            } 

            words.Add(((char)rnd.Next(33,64)).ToString());
            tempent *= 31;
            dtempent += Math.Log(31,2);
            curlen++;
            string number = (rnd.Next(0,100)).ToString();
            tempent *= 100;
            dtempent += Math.Log(100,2);
            words.Add(number);
            curlen+=number.Length;

        }   
    }
      Response res = new Response();
      res.words = words;
      res.password = string.Join("", words.ToArray());
      res.language = selllang;
      res.entropy = Math.Log(tempent,2);
      res.entropy2 = dtempent;
      return req.CreateResponse(HttpStatusCode.OK, res);
      //  return req.CreateResponse(HttpStatusCode.OK, selllang);
}


public class Language
{
    public string langcode;
    public string langname;
    public int dictionarysize;
}

public class Response
{
    public List<string> words;
    public String password;
    public Language language;
    public double entropy;
    public double entropy2;
}