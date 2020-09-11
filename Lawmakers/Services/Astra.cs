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

        public static void AddLawmaker(IConfiguration config, string token, Lawmaker lawmaker, int id)
        {
            var baseUrl = config.GetSection("Astra").GetSection("BaseUrl").Value;
            var keyspace = config.GetSection("Astra").GetSection("Keyspace").Value;
            var collection = config.GetSection("Astra").GetSection("Collection").Value;
            var lawmakerJson = JsonConvert.SerializeObject(lawmaker);
            var url = string.Format("{0}namespaces/{1}/collections/{2}/{3}", baseUrl, keyspace, collection, id);

            var client = new RestClient(url); 
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            request.AddHeader("X-Cassandra-Token", token);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", lawmakerJson, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        public static LawmakerDocument GetLawmaker(IConfiguration config, string token, int id)
        {
            LawmakerDocument lawmakerDocument = new LawmakerDocument();

            var baseUrl = config.GetSection("Astra").GetSection("BaseUrl").Value;
            var keyspace = config.GetSection("Astra").GetSection("Keyspace").Value;
            var collection = config.GetSection("Astra").GetSection("Collection").Value;
            var url = string.Format("{0}namespaces/{1}/collections/{2}/{3}", baseUrl, keyspace, collection, id);

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
                var token = Astra.GetToken(authUrl, username, password);

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
                    AddLawmaker(config, token, lawmaker, id);
                }

            } catch (Exception ex)
            {

            }

            return lawmakers;
        }
    }
}
