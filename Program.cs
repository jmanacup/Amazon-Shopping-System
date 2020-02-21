using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//Azure references
using System.Configuration;
using System.Net;
using Microsoft.Azure.Cosmos;


namespace FirstProjectinCSHARP
{
    class Program
    {
        // The Azure Cosmos DB endpoint
        private static readonly string EndpointUri = "https://azure-db-main.documents.azure.com:443/";
        // Primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = "GDi3tH4Y5qTxq9t8zH0oAJpXtHygwvMX3Erx7kzhtL1TMrqr8FgTvp9ZUrz5WkBzSJa08YRSWqp8A3TUjptO9g==";

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;


        public static async Task Main(string[] args)
        {


            /*===============THIS IS FOR CONNECTING IN AZURE================
             try
             {
                 Console.WriteLine("Beginning operations...\n");
                 Program p = new Program();
                 await p.EstablishConnection();

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
          .   finally
             {
                 Console.WriteLine("End of demo, press any key to exit.");
                 Console.ReadKey();
             }                                                             
             ==============================================================
            */

            ShowIndex();

        }

        public async Task EstablishConnection()
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);

        }

        public static void ShowIndex()
        {
            Console.WriteLine("\t\t\t\t\tAMAZONE");
            Console.WriteLine("\t\t\t\t\"Like Amazon but Crappier\"");
            Console.WriteLine("==================================================================================\n");
            Console.WriteLine("\t\t\t\tWhat do you want to do?\n");
            Console.WriteLine("\t\t[1] Register an Account");
            Console.WriteLine("\t\t[2] Login");
            Console.WriteLine("\t\t[3] Exit\n");

            Console.Write("Type your response here: ");
            int ans = Convert.ToInt32(Console.ReadLine());

            //choices designated to their specific method call/action
            switch (ans) 
           
            {
                case 1:
                    Register();
                    Console.Clear();
                    break;
                case 2:
                    //Login();
                    Console.Clear();
                    break;
                default:
                    System.Environment.Exit(1);
                    break;
            }
        }

        public static void Register()
        {

        }
    }
}
