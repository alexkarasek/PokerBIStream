using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.EventHubs;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace testJson
{
    class StreamData
    {
        public static EventHubClient eventHubClient;
        public const string EventHubConnectionString = "Endpoint=sb://akpshub.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=FlAcsDZnZOJo9ZRRvBqkHHsRSkmdkdxjhjjoiZqcq/M=";
        public const string EventHubName = "pshub1";
        public static bool SetRandomPartitionKey = false;

        public  async Task MainAsync(ga_payload gap)
        {
            // Creates an EventHubsConnectionStringBuilder object from a the connection string, and sets the EntityPath.
            // Typically the connection string should have the Entity Path in it, but for the sake of this simple scenario
            // we are using the connection string from the namespace.
            

            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
            {
                EntityPath = EventHubName
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            //await SendMessagesToEventHub(100);
            await SendMessagesToEventHub(gap);  //UPDATED

            await eventHubClient.CloseAsync();

            //Console.WriteLine("Press any key to exit.");
            //Console.ReadLine();
        }

        // Creates an Event Hub client and sends 100 messages to the event hub.  //UPDATED
        public static async Task SendMessagesToEventHub(ga_payload ga_Payload)
        {
            //var rnd = new Random();

            //for (var i = 0; i < numMessagesToSend; i++)
            //{
                try
                {
                    //Games g = new Games();
                    //g.gameid = "201500064889";
                    //g.sitename = "PS";
                    string tstmessage = JsonConvert.SerializeObject(ga_Payload); ;
                    //var message = $"Message {i}";
                    ////string tstmessage = {"id":"201500064889","timestamp":"2019/06/18 2:20:35 ET","sitename":"PokerStars","tablename":"Achird","limits":"$0.05/$0.10 USD","seats":[{"playername":"vassartem","seatno":1},{"playername":"Bebabcsinya","seatno":2},{"playername":"bartux363","seatno":3},{"playername":"Y2K-17","seatno":4},{"playername":"GabrielCH88","seatno":5},{"playername":"plumiy","seatno":6}],"actions":[{"GameActionId":1,"streetname":"Pre-flop","action":"blind","actor":"GabrielCH88","amount":-0.05},{"GameActionId":2,"streetname":"Pre-flop","action":"blind","actor":"plumiy","amount":-0.10},{"GameActionId":3,"streetname":"Pre-flop","action":"fold","actor":"vassartem","amount":0.0},{"GameActionId":4,"streetname":"Pre-flop","action":"raise","actor":"Bebabcsinya","amount":-0.25},{"GameActionId":5,"streetname":"Pre-flop","action":"fold","actor":"bartux363","amount":0.0},{"GameActionId":6,"streetname":"Pre-flop","action":"fold","actor":"Y2K-17","amount":0.0},{"GameActionId":7,"streetname":"Pre-flop","action":"fold","actor":"GabrielCH88","amount":0.0},{"GameActionId":8,"streetname":"Pre-flop","action":"call","actor":"plumiy","amount":-0.15},{"GameActionId":9,"streetname":"FLOP","action":"check","actor":"plumiy","amount":0.0},{"GameActionId":10,"streetname":"FLOP","action":"bet","actor":"Bebabcsinya","amount":-0.30},{"GameActionId":11,"streetname":"FLOP","action":"call","actor":"plumiy","amount":-0.30},{"GameActionId":12,"streetname":"TURN","action":"check","actor":"plumiy","amount":0.0},{"GameActionId":13,"streetname":"TURN","action":"check","actor":"Bebabcsinya","amount":0.0},{"GameActionId":14,"streetname":"RIVER","action":"check","actor":"plumiy","amount":0.0},{"GameActionId":15,"streetname":"RIVER","action":"check","actor":"Bebabcsinya","amount":0.0},{"GameActionId":16,"streetname":"RIVER","action":"win","actor":"Bebabcsinya","amount":1.10},{"GameActionId":17,"streetname":"RIVER","action":"rake","actor":"house","amount":0.05}]};
                    //// Set random partition key?
                    //if (SetRandomPartitionKey)
                    //{
                    //    var pKey = Guid.NewGuid().ToString();
                    //    //await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)), pKey);
                    //    //Console.WriteLine($"Sent message: '{message}' Partition Key: '{pKey}'");

                    //    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(tstmessage)), pKey);
                    //    Console.WriteLine($"Sent message: '{tstmessage}' Partition Key: '{pKey}'");
                    //}
                    //else
                    {
                        //await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                        //Console.WriteLine($"Sent message: '{message}'");

                        await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(tstmessage)));
                        //Console.WriteLine($"Sent message: '{tstmessage}' ");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }

                await Task.Delay(0);
            //}
        }
    }
}
