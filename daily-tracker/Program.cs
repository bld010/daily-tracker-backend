using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;
using Program;

namespace daily_tracker
{
    class Program
    {

        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = "https://daily-tracker.documents.azure.com:443/";
        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = "";

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private string databaseId = "daily-notes";
        private string containerId = "daily-notes";
        private object DiaryItem;

        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Beginning operations...\n");
                Program p = new Program();
                await p.GetStartedDemoAsync();

            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }

        /*
            Entry point to call methods that operate on Azure Cosmos DB resources in this sample
        */
        public async Task GetStartedDemoAsync()
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions()
            {
                ConnectionMode = ConnectionMode.Gateway
            });

            await this.CreateDatabaseAsync();
            await this.CreateContainerAsync();

            await this.AddItemsToContainerAsync();


        }

        /// <summary>
        /// Create the database if it does not exist
        /// </summary>
        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }


        private async Task CreateContainerAsync()
        {
            // Create a new container
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/UserName");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }

        private async Task AddItemsToContainerAsync()
        {
            DiaryItem diaryItem = new DiaryItem
            {
                UserName = "bld010",
                Note = "This is a note from the program",
                TimeStamp = DateTime.UtcNow.ToString(),
                Rating = "3",
                Id = Guid.NewGuid().ToString()
            };
            

            try
            {
                // Read the item to see if it exists.  
                ItemResponse<DiaryItem> diaryItemResponse = await this.container.ReadItemAsync<DiaryItem>(diaryItem.Id, new PartitionKey(diaryItem.UserName));
                Console.WriteLine("Item in database with id: {0} already exists\n", diaryItemResponse.Resource.Id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                
                ItemResponse<DiaryItem> diaryItemResponse = await this.container.CreateItemAsync<DiaryItem>(diaryItem, new PartitionKey(diaryItem.UserName));

                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", diaryItemResponse.Resource.Id, diaryItemResponse.RequestCharge);
            }
        }
    }
}
