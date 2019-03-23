using System.Net;
using Microsoft.Azure.Services.AppAuthentication;
using System.Configuration;
using System.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using Diacritics.Extensions;


public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    // parse query parameter
    string lang = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "lang", true) == 0)
        .Value;
    int minlen;
    int.TryParse(req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "minlen", true) == 0)
        .Value, out minlen);
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

    // Get variables from query string or body data
    lang = (lang ?? data?.lang) ?? "Random";
    sasciionly = (sasciionly ?? data?.asciionly) ?? "True";

    if (minlen == 0)
        minlen = data?.minlen ?? 10;
    if (minlen > 15)
        minlen = 16;

    bool asciionly = true;
    bool.TryParse(sasciionly, out asciionly);

    //log parsed parameters
    log.Info($"Language {lang}");
    log.Info($"Minlen {minlen}");
    log.Info($"ASCIIonly {asciionly}");
    log.Info($"SASCIIonly {sasciionly}");

    //set initial password length
    int curlen = 0;
    Random rnd = new Random();

    //get access token and connect to SQL
    var tokenProvider = new AzureServiceTokenProvider();
    string accessToken = await tokenProvider.GetAccessTokenAsync("https://database.windows.net/");
    var connStr = ConfigurationManager.ConnectionStrings["connstring"].ConnectionString;
    SqlConnection conn = new SqlConnection(connStr);
    conn.AccessToken = accessToken;
    conn.Open();

    //get list of languages from the database
    var languages = new List<Language>();
    var sqlquery = @"SELECT [langcode]
                        ,[langname]
                        ,[maxid]
                        FROM [dbo].[dictionaries]";

    SqlCommand cmd = new SqlCommand(sqlquery, conn);
    SqlDataReader reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        Language l = new Language();
        l.language_code = reader.GetString(0);
        l.language_name = reader.GetString(1);
        l.dictionary_size = reader.GetInt32(2);
        languages.Add(l);
        log.Info($"{l}");
    }
    reader.Close();

    //select language
    Language selllang = new Language();

    var result = from lan in languages
                 where lan.language_code == lang
                 select lan;
    selllang = result.FirstOrDefault();

    //if language doesn't exist select random
    if (selllang == null || lang == "Random")
    {
        int r = rnd.Next(languages.Count);
        selllang = languages[r];
    }
    log.Info($"{selllang.language_code}");

    //individual password components
    var words = new List<string>();

    //entropy counter
    double dtempent = 0;

    //start generating password
    while (curlen < minlen)
    {
        int id = rnd.Next(selllang.dictionary_size);
        sqlquery = "SELECT TOP 1 word FROM words_" + selllang.language_code + " WHERE id >" + id;
        log.Info($"{sqlquery}");
        cmd = new SqlCommand(sqlquery, conn);
        reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            string sqlread = reader.GetString(0);
            int capitalize = rnd.Next(0, 4);
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
            dtempent += Math.Log(selllang.dictionary_size * 3, 2);
        }
        reader.Close();
        words.Add(((char)rnd.Next(33, 64)).ToString());
        dtempent += Math.Log(31, 2);
        curlen++;
        string number = (rnd.Next(0, 100)).ToString();
        dtempent += Math.Log(100, 2);
        words.Add(number);
        curlen += number.Length;

    }

    //build and return response
    Response res = new Response();
    res.words = words;
    res.password = string.Join("", words.ToArray());
    res.language = selllang;
    res.password_entropy = dtempent;
    res.password_length = curlen;
    conn.Dispose();
    return req.CreateResponse(HttpStatusCode.OK, res);
}


public class Language
{
    public string language_code;
    public string language_name;
    public int dictionary_size;
}

public class Response
{
    public List<string> words;
    public String password;
    public Language language;
    public int password_length;
    public double password_entropy;
}
