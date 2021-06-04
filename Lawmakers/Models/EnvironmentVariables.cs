using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lawmakers.Models
{
    public class EnvironmentVariables
    {
        public EnvironmentVariables(IConfiguration config)
        {
            this.ASTRA_DB_ID = config.GetSection("Astra").GetSection("ASTRA_DB_ID").Value;
            this.ASTRA_DB_REGION = config.GetSection("Astra").GetSection("ASTRA_DB_REGION").Value;
            this.ASTRA_DB_KEYSPACE = config.GetSection("Astra").GetSection("ASTRA_DB_KEYSPACE").Value;
            this.ASTRA_DB_APPLICATION_TOKEN = config.GetSection("Astra").GetSection("ASTRA_DB_APPLICATION_TOKEN").Value;
        }
        public string ASTRA_DB_ID { get; set; }
        public string ASTRA_DB_REGION { get; set; }
        public string ASTRA_DB_KEYSPACE { get; set; }
        public string ASTRA_DB_APPLICATION_TOKEN { get; set; }
    }
}
