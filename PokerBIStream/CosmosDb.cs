using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;


namespace testJson
{
    public class CosmosDb
    {

        public DocumentClient client;

        public CosmosDb(string url, string key, Games games)
        {
           
        }

       

        public void WriteToConsoleAndPromptToContinue(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

        public async Task GetStartedDemo(string url, string key, Games g)
        {
            this.client = new DocumentClient(new Uri(url), key);

            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("Poker_Logs"), new DocumentCollection { Id = "GameActions" });
            await this.CreateFamilyDocumentIfNotExists( g);

        }

        private async Task CreateFamilyDocumentIfNotExists(Games games)
        {
            try
            {
                await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri("Poker_Logs", "GameActions", games.gameid));
                this.WriteToConsoleAndPromptToContinue("Found {0}", games.gameid);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("Poker_Logs", "GameActions"), games);
                    //this.WriteToConsoleAndPromptToContinue("Created Family {0}", games.gameid);
                }
                else
                {
                    throw;
                }
            }
        }



        // }
    }
}
