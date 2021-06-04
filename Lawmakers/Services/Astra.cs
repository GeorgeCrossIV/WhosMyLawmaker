using Lawmakers.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Lawmakers.Services
{
    public class Astra
    {
        public static string GetToken(string authUrl, string username, string password)
        {            
            var client = new RestClient(authUrl);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\n    \"username\": \"" + username + "\",\n    \"password\": \"" + password + "\"\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            return JObject.Parse(response.Content)["authToken"].ToString();
        }

        public static void AddLawmaker(IConfiguration config, Lawmaker lawmaker, int id)
        {
            var baseUrl = config.GetSection("Astra").GetSection("BaseUrl").Value;
            var collection = config.GetSection("Astra").GetSection("Collection").Value;
            var lawmakerJson = JsonConvert.SerializeObject(lawmaker);
            var db_id = config.GetSection("Astra").GetSection("ASTRA_DB_ID").Value;
            var region = config.GetSection("Astra").GetSection("ASTRA_DB_REGION").Value;
            var keyspace = config.GetSection("Astra").GetSection("ASTRA_DB_KEYSPACE").Value;
            var token = config.GetSection("Astra").GetSection("ASTRA_DB_APPLICATION_TOKEN").Value;
            var url = string.Format("https://{0}-{1}.apps.astra.datastax.com/api/rest/v2/namespaces/{2}/collections/lawmakers/{3}",
                    db_id, region, keyspace, id);

            var client = new RestClient(url); 
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            request.AddHeader("X-Cassandra-Token", token);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", lawmakerJson, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);

        }

        public static LawmakerDocument GetLawmaker(IConfiguration config, int id)
        {
            LawmakerDocument lawmakerDocument = new LawmakerDocument();

            var baseUrl = config.GetSection("Astra").GetSection("BaseUrl").Value;
            var collection = config.GetSection("Astra").GetSection("Collection").Value;

            var db_id = config.GetSection("Astra").GetSection("ASTRA_DB_ID").Value;
            var region = config.GetSection("Astra").GetSection("ASTRA_DB_REGION").Value;
            var keyspace = config.GetSection("Astra").GetSection("ASTRA_DB_KEYSPACE").Value;
            var token = config.GetSection("Astra").GetSection("ASTRA_DB_APPLICATION_TOKEN").Value;

            var url = string.Format("https://{0}-{1}.apps.astra.datastax.com/api/rest/v2/namespaces/{2}/collections/lawmakers/{3}",
                    db_id, region, keyspace, id);

            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-Cassandra-Token", token);
            IRestResponse response = client.Execute(request);
            lawmakerDocument = JsonConvert.DeserializeObject<LawmakerDocument>(response.Content);

            return lawmakerDocument;
        }
        public static List<Lawmaker> LoadLawmakers(IWebHostEnvironment host, IConfiguration config)
        {
            var lawmakersFilename = "legislators.json";
            List<Lawmaker> lawmakers = new List<Lawmaker>();

            try
            {
                var authUrl = config.GetSection("Astra").GetSection("AuthUrl").Value;
                var username = config.GetSection("Astra").GetSection("Username").Value;
                var password = config.GetSection("Astra").GetSection("Password").Value;

                // load lawmaker data from the Internet
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://theunitedstates.io/congress-legislators/legislators-current.json", lawmakersFilename);
                }

                // save the lawmaker JSON data to the local web server
                var lawmakersFilepath = host.ContentRootPath + "\\" + lawmakersFilename;
                var data = File.ReadAllText(lawmakersFilepath);
                lawmakers = JsonConvert.DeserializeObject<List<Lawmaker>>(data);

                // clear the existing lawmaker data from the collection in Astra


                // add the lawmakers to the Astra collection
                var id = 0;
                foreach (Lawmaker lawmaker in lawmakers)
                {
                    id++; // incrementer used as the document id
                    lawmaker.state = lawmaker.terms.First().state;
                    AddLawmaker(config, lawmaker, id);
                }

            } catch (Exception ex)
            {

            }

            return lawmakers;
        }

        public static List<LawmakerDocument> GetLawmakers(IConfiguration config, string state)
        {
            List<LawmakerDocument> lawmakerDocuments = new List<LawmakerDocument>();
            List<Lawmaker> lawmakers = new List<Lawmaker>();

            var db_id = config.GetSection("Astra").GetSection("ASTRA_DB_ID").Value;
            var region = config.GetSection("Astra").GetSection("ASTRA_DB_REGION").Value;
            var keyspace = config.GetSection("Astra").GetSection("ASTRA_DB_KEYSPACE").Value;
            var token = config.GetSection("Astra").GetSection("ASTRA_DB_APPLICATION_TOKEN").Value;
            var baseUrl = config.GetSection("Astra").GetSection("BaseUrl").Value;
            var collection = config.GetSection("Astra").GetSection("Collection").Value;
            var url = string.Format("https://{0}-{1}.apps.astra.datastax.com/api/rest/v2/namespaces/{2}/collections/lawmakers?where={{\"state\": {{\"$eq\": \"{3}\"}}}}&page-size=20",
                db_id, region, keyspace, state);

            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-Cassandra-Token", token);
            IRestResponse response = client.Execute(request);
            Console.WriteLine("retrieved lawmakers for: " + state);

            Lawmaker lawmaker;            

            // get search results
            JObject jObject = JObject.Parse(response.Content);
            JToken result = JToken.Parse(response.Content);
            JToken data = result.SelectToken("data");
            foreach (var x in data.Children())
            {
                var lmString = x.ToString().Split(':', 2)[1];
                lawmaker = JsonConvert.DeserializeObject<Lawmaker>(lmString);
                //lawmakers.Add(lawmaker);
                lawmakerDocuments.Add(new LawmakerDocument {
                    documentId=x.Path.Split('.',2)[1],
                    data = lawmaker
                });
            }

            return lawmakerDocuments;
        }
    }
}
