using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace testJson
{
    public class CosmosDb
    {

        public CosmosDb(string url, string key)
        {
            DocumentClient client = new DocumentClient(new Uri(url), key);
         }

       // }
    }
}
