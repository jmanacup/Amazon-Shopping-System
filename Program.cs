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

        // The containers we will create.
        //container1 = USER; container2 = MANAGER; container3 = SHOPPINGITEM
        private Container container1, container2, container3;

        // The name of the database and container
        private string databaseId = "ShoppingDatabase";
        private string containerId1 = "USER";
        private string containerId2 = "MANAGER";
        private string containerId3 = "SHOPPINGITEM";

        public static async Task Main(string[] args)
        {
           
                Program p = new Program();
                //Establish connection to the Azure service
                await p.EstablishConnection();
                await p.ShowIndex();

        }
        
        public async Task EstablishConnection()
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);

            //Create a new database called ShoppingDatabase
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);

            //Create containers for each classes
            this.container1 = await this.database.CreateContainerIfNotExistsAsync(containerId1, "/LastName");
            this.container2 = await this.database.CreateContainerIfNotExistsAsync(containerId2, "/LastName");
            this.container3 = await this.database.CreateContainerIfNotExistsAsync(containerId3, "/category");

        }
        public async Task ShowIndex()
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
                    Console.Clear();
                    await this.Register();
                    break;
                case 2:
                    Console.Clear();
                    await this.Login();
                    break;
                default:
                    System.Environment.Exit(1);
                    break;
            }
        }

        //Registration Part of the Program
        public async Task Register()
        {
            User account = new User();

            Console.WriteLine("\nUSER REGISTRATION");
            Console.WriteLine("==================================================================================\n");

            Console.Write("Username: ");
            account.UserName = Console.ReadLine();

            Console.Write("Password: ");
            account.password = Console.ReadLine();

            Console.Write("First Name: ");
            account.FirstName = Console.ReadLine();

            Console.Write("Last Name: ");
            account.LastName = Console.ReadLine();

            Console.Write("Age: ");
            account.age = Convert.ToInt32(Console.ReadLine());

            Console.Write("Email Address: ");
            account.EmailAdd = Console.ReadLine();

            Console.Write("Physical Address: ");
            account.PhyAdd = Console.ReadLine();

            account.Id = account.UserName;

            Console.WriteLine("\n\nSetting up Account... Please Wait...");

            try
            {
                //Check to see if the account already exist
                ItemResponse<User> UserResponse = await this.container1.ReadItemAsync<User>(account.UserName, new PartitionKey(account.LastName));
                Console.WriteLine("\n\nThe username is already taken. Please Try again.\nPress any key to continue.");
                Console.ReadKey();
                Console.Clear();
                await this.Register();
            }

            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {

                // Create an item in the container with account; Partition key will be Username of the account
                ItemResponse<User> UserResponse = await this.container1.CreateItemAsync<User>(account, new PartitionKey(account.LastName));

            }

            Console.Write("\n\nYou have successfully created your account.\n Press any key to continue.");
            Console.ReadKey();

            Console.Clear();
            await this.ShowIndex();
        }

        //Login part of the Program
        public async Task Login()
        {

            string username, password;

            Console.WriteLine("\nLOGIN PAGE");
            Console.WriteLine("==================================================================================\n");

            Console.Write("\t\t\tUsername: ");
            username = Console.ReadLine();

            Console.Write("\t\t\tPassword: ");
            password = Console.ReadLine();

            string checkPass = "SELECT * FROM c WHERE c.UserName = '" + username + "'";

            QueryDefinition queryDefinition = new QueryDefinition(checkPass);
            FeedIterator<User> queryResultSetIterator = this.container1.GetItemQueryIterator<User>(queryDefinition);


            FeedResponse<User> currentResult = await queryResultSetIterator.ReadNextAsync();

            User testUser = new User();

            //If the given password is not the same as that of the value in the JSON document

            //since this is only 1
            foreach (User user in currentResult)
                testUser = user;

            if (testUser.password == password)
            {
                Console.Clear();
                this.LoginUser();
            }

            else
            {

                Console.WriteLine("\n\nInvalid password for the given username. Please try again.");
                System.Threading.Thread.Sleep(4000);
                Console.Clear();
                await this.Login();
            }
        }

        public void LoginUser() {

            Console.WriteLine("\nLOGIN PAGE");
            Console.WriteLine("==================================================================================\n");


        }
        public void LoginManager() { }
    }
}
