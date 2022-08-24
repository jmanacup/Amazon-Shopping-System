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
        private Container container1, container2;

        // The name of the database and container
        private string databaseId = "ShoppingDatabase";
        private string containerId1 = "USER";
        private string containerId2 = "SHOPPINGITEM";

        public static async Task Main(string[] args)
        {

                Program p = new Program();
                //Establish connection to the Azure service
                await p.EstablishConnection();
                await p.ShowIndex();

        }
        
        public async Task EstablishConnection()
        {
            Console.WriteLine("Please wait... Establishing connection.....");
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);

            //Create a new database called ShoppingDatabase
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);

            //Create containers for each classes
            this.container1 = await this.database.CreateContainerIfNotExistsAsync(containerId1, "/LastName");
            this.container2 = await this.database.CreateContainerIfNotExistsAsync(containerId2, "/category");

            Console.Clear();
        }
        public async Task ShowIndex()
        {
            Console.WriteLine("\t\t\t\t\tAmazing Shopping System");
            Console.WriteLine("\t\t\t\t\"We got stuff!\"");
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
            account.isManager = false;

            Console.WriteLine("\n\nSetting up Account... Please Wait...");

            try
            {
                //Check to see if the account already exist
                await this.container1.ReadItemAsync<User>(account.UserName, new PartitionKey(account.LastName));
                Console.WriteLine("\n\nThe username is already taken. Please Try again.\nPress any key to continue.");
                Console.ReadKey();
                Console.Clear();
                await this.Register();
            }

            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {

                // Create an item in the container with account; Partition key will be Username of the account
                await this.container1.CreateItemAsync<User>(account, new PartitionKey(account.LastName));

            }

            Console.Write("\n\nYou have successfully created your account.\nPress any key to continue.");
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

            Console.Write("\n\nLogging in... Please wait....");
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

                if (testUser.isManager)
                    await this.LoginManager(testUser); //manager login
                else
                    await this.LoginUser(testUser); //user login
            }

            else
            {

                Console.WriteLine("\n\nInvalid password for the given username. Please try again.");
                System.Threading.Thread.Sleep(4000);
                Console.Clear();
                await this.Login();
            }
        }

        //Login part of the User
        public async Task LoginUser(User account) {

            Console.WriteLine("\nHOME PAGE");
            Console.WriteLine("==================================================================================\n");

            Console.WriteLine("Welcome " + account.UserName);

            Console.WriteLine("\nWhat do you want to do today?");
            Console.WriteLine("\n\t\t\t[1] Edit Profile");
            Console.WriteLine("\t\t\t[2] Browse Products");
            Console.WriteLine("\t\t\t[3] See Shopping Cart");
            Console.WriteLine("\t\t\t[4] Logout\n");

            Console.Write("Type your response here: ");
            int ans = Convert.ToInt32(Console.ReadLine());

            switch (ans)
            {
                case 1:
                    Console.Clear();
                    await EditProfile(account);
                    break;
                case 2:
                    Console.Clear();
                    await BrowseProduct(account);
                    break;
                case 3:
                    Console.Clear();
                    await ShoppingCart(account);
                    break;
                default:
                    Console.WriteLine("Logging out....");
                    System.Threading.Thread.Sleep(4000);
                    Console.Clear();
                    await ShowIndex();
                    break;
            }
        }

        //Login part of the Manager
        public async Task LoginManager(User account) {

            Console.WriteLine("\nHOME PAGE (MANAGER)");
            Console.WriteLine("==================================================================================\n");

            Console.WriteLine("Welcome " + account.UserName);

            Console.WriteLine("\nWhat do you want to do today?");
            Console.WriteLine("\n\t\t\t[1] Edit Profile");
            Console.WriteLine("\t\t\t[2] Manage Item in the Store");
            Console.WriteLine("\t\t\t[3] Browse Products");
            Console.WriteLine("\t\t\t[4] See Shopping Cart");
            Console.WriteLine("\t\t\t[5] Logout\n");

            Console.Write("Type your response here: ");
            int ans = Convert.ToInt32(Console.ReadLine());

            switch (ans)
            {
                case 1:
                    Console.Clear();
                    await EditProfile(account);
                    break;
                case 2:
                    Console.Clear();
                    await ManageItem(account);
                    break;
                case 3:
                    Console.Clear();
                    await BrowseProduct(account);
                    break;
                case 4:
                    Console.Clear();
                    await ShoppingCart(account);
                    break;
                default:
                    Console.WriteLine("Logging out....");
                    System.Threading.Thread.Sleep(4000);
                    Console.Clear();
                    await ShowIndex();
                    break;
            }

        }
        public async Task EditProfile(User account) {

            Console.WriteLine("\nEDIT PROFILE");
            Console.WriteLine("==================================================================================\n");

            account.showProfile();
            Console.WriteLine("\t\t [8] Back to Home");
            Console.WriteLine("\nWhich one do you want to change?");
            Console.Write("\n\nType your response here: ");
            int ans = Convert.ToInt32(Console.ReadLine());

            //Reads the data in the JSON document and extract it
            ItemResponse<User> dataResponse = await this.container1.ReadItemAsync<User>(account.UserName, new PartitionKey(account.LastName));
            User gotAccount = dataResponse.Resource;

            switch (ans)
            {
                case 1:
                    Console.Write("\nType in your new Username here:  ");
                    gotAccount.UserName = Console.ReadLine();
                    break;
                case 2:
                    Console.Write("\nType in your new Password here:  ");
                    gotAccount.password = Console.ReadLine();
                    break;
                case 3:
                    Console.Write("\nType in your new First Name here:  ");
                    gotAccount.FirstName = Console.ReadLine();
                    break;
                case 4:
                    Console.Write("\nType in your new Last Name here:  ");
                    gotAccount.LastName = Console.ReadLine();
                    break;
                case 5:
                    Console.Write("\nType in your new Email Address here:  ");
                    gotAccount.EmailAdd = Console.ReadLine();
                    break;
                case 6:
                    Console.Write("\nType in your new Physical Address here:  ");
                    gotAccount.PhyAdd = Console.ReadLine();
                    break;
                case 7:
                    Console.Write("\nType in your new age here:  ");
                    gotAccount.age = Convert.ToInt32(Console.ReadLine());
                    break;
                default:
                    Console.WriteLine("\nRedirecting to main page....");
                    System.Threading.Thread.Sleep(4000);
                    Console.Clear();
                    if (account.isManager)
                        await LoginManager(account);
                    else
                        await LoginUser(account);
                    break;
            }

            dataResponse = await this.container1.ReplaceItemAsync<User>(gotAccount, gotAccount.Id, new PartitionKey(account.LastName));
            Console.WriteLine("\nChanges are successfully made.");
            System.Threading.Thread.Sleep(4000);
            Console.Clear();
            if (account.isManager)
                await LoginManager(account);
            else
                await LoginUser(account);

        }
        public async Task BrowseProduct(User account) {

            Console.WriteLine("\nBROWSE PRODUCT");
            Console.WriteLine("=================================================================================================\n");

            string header = String.Format("{0,10}\t\t\t{1,15}\t\t\t{2,7}\t\t\t{3,7}", "Item Name", "Category", "Price", "Quantity");
            Console.WriteLine(header + "\n");
            string sqlQueryText = "SELECT * FROM c";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<ShoppingItem> queryResultSetIterator = this.container2.GetItemQueryIterator<ShoppingItem>(queryDefinition);

            List<ShoppingItem> itemsAvailable = new List<ShoppingItem>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<ShoppingItem> currentResultSet = await queryResultSetIterator.ReadNextAsync();

                foreach (ShoppingItem item in currentResultSet) {
                    itemsAvailable.Add(item);
                    Console.WriteLine(item);
                }
            }

            string ans = "";
            do {
                if (!(itemsAvailable.Exists(x => x.ItemName == ans)) && ans != "")
                    Console.WriteLine("The item is not found in the store. Please Try again.");

                Console.Write("\nType in the item name that you want to purchase (0 to go back to main page): ");
                ans = Console.ReadLine();

            } while (!(itemsAvailable.Exists(x => x.ItemName == ans)) && ans !="0");//goes through the loop everytime the input item is not in the store

            if (ans == "0")
            {
                Console.WriteLine("Going back to main page... Please wait....");
                System.Threading.Thread.Sleep(4000);
                Console.Clear();
                if (account.isManager)
                    await this.LoginManager(account);
                else
                    await this.LoginUser(account);
            }

            else
            {
                //get the specific item in the list
                ShoppingItem itemFound = itemsAvailable.Find(x => x.ItemName == ans);

                ItemResponse<ShoppingItem> gotData = await this.container2.ReadItemAsync<ShoppingItem>(ans, new PartitionKey(itemFound.category));
                ShoppingItem itemBody = gotData.Resource;

                //This will store the item that the user has bought
                ShoppingItem itemForShopper = new ShoppingItem();

                Console.WriteLine("You have chosen " + ans);

                itemForShopper.ItemName = ans;

                //constants for the shopping item
                itemForShopper.Id = itemForShopper.ItemName;
                itemForShopper.price = itemBody.price;
                itemForShopper.category = itemBody.category;

                Console.Write("\nInsert the quantity you want to purchase: ");
                itemForShopper.quantity = Convert.ToInt32(Console.ReadLine());

                Console.Write("Adding to cart... Please wait...");
                

                //subtracting the quantity of the item
                itemBody.quantity -= itemForShopper.quantity;

                //add the item in the shopping cart for the account
                ItemResponse<User> gotUser = await this.container1.ReadItemAsync<User>(account.Id, new PartitionKey(account.LastName));
                User userFromDb = gotUser.Resource;

                userFromDb.ShoppingCart.Add(itemForShopper);

                //update the shopping cart of the user
                await this.container1.ReplaceItemAsync<User>(userFromDb, userFromDb.Id, new PartitionKey(userFromDb.LastName));
                //update the quantity of the product
                await this.container2.ReplaceItemAsync<ShoppingItem>(itemBody, itemBody.Id, new PartitionKey(itemBody.category));

                Console.WriteLine("\nYou have successfully purchased " + itemForShopper.quantity + " " + itemForShopper.ItemName);

                Console.Write("\n Refreshing page... please wait.....");
                System.Threading.Thread.Sleep(6000);
                Console.Clear();
                await this.BrowseProduct(account);
            }

        }
        public async Task ShoppingCart(User account)
        {

            Console.WriteLine("\nMY SHOPPING CART");
            Console.WriteLine("=================================================================================================\n");

            string header = String.Format("{0,10}\t\t\t{1,15}\t\t\t{2,7}\t\t\t{3,7}", "Item Name", "Category", "Price", "Quantity");
            Console.WriteLine(header + "\n");

            ItemResponse<User> gotUser = await this.container1.ReadItemAsync<User>(account.Id, new PartitionKey(account.LastName));
            User userFromDb = gotUser.Resource;

            if (userFromDb.ShoppingCart.Count == 0)
            {
                Console.WriteLine("\nYou have not purchased anything yet in the store. Redirecting to the main page... Please wait....");
                System.Threading.Thread.Sleep(6000);
                Console.Clear();
                if (account.isManager)
                    await this.LoginManager(account);
                else
                    await this.LoginUser(account);

            }
            else
            {
                double total = 0;
                foreach (ShoppingItem item in userFromDb.ShoppingCart) { 
                    Console.WriteLine(item);
                    total += (item.price * item.quantity);
                }

                Console.WriteLine("\n\n\nYour total is: " + total + " dollars.");
            }

            Console.Write("\n\nPress any key to go back to the main page...");
            Console.ReadKey();
            Console.Clear();
            if (account.isManager)
                await LoginManager(account);
            else
                await LoginUser(account);

        }
        public async Task ManageItem(User account) {

            Console.WriteLine("\nMANAGE ITEMS");
            Console.WriteLine("==================================================================================\n");

            Console.WriteLine("\nWhat do you want to do today?");
            Console.WriteLine("\n\t\t\t[1] Add Item");
            Console.WriteLine("\t\t\t[2] Delete Item");
            Console.WriteLine("\t\t\t[3] Change Item Price");
            Console.WriteLine("\t\t\t[4] Back to Home Page");

            Console.Write("\nType your response here: ");
            int ans = Convert.ToInt32(Console.ReadLine());

            switch (ans)
            {
                case 1:

                    ShoppingItem item = new ShoppingItem();

                    Console.Write("\nInsert name of the item: ");
                    item.ItemName = Console.ReadLine();

                    Console.Write("Insert category of the item: ");
                    item.category = Console.ReadLine();

                    Console.Write("Insert price of the item: ");
                    item.price = Convert.ToDouble(Console.ReadLine());

                    Console.Write("Insert quantity of the item: ");
                    item.quantity = Convert.ToInt32(Console.ReadLine());

                    item.Id = item.ItemName;
                    Console.WriteLine("\nAdding Item in the Store... Please wait....");
                    try
                    {
                        //Check to see if the account already exist
                        await this.container2.ReadItemAsync<ShoppingItem>(item.ItemName, new PartitionKey(item.category));
                        Console.WriteLine("\n\nThe item already exist. Please Try again.\nPress any key to continue.");
                        Console.ReadKey();
                        Console.Clear();
                        await this.ManageItem(account);
                    }

                    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                    {

                        // Create an item in the container with account; Partition key will be Username of the account
                        await this.container2.CreateItemAsync<ShoppingItem>(item, new PartitionKey(item.category));
                        Console.WriteLine("\nSuccessfully added " + item.ItemName + ". Redirecting to Manage Items...");
                    }
                    break;
                case 2:

                    Console.Write("\nInsert name of the item you want to be deleted: ");
                    string itemName = Console.ReadLine();

                    
                    string checkItemName = "SELECT * FROM c WHERE c.ItemName = '" + itemName + "'";
                    QueryDefinition queryDefinition = new QueryDefinition(checkItemName);
                    FeedIterator<ShoppingItem> queryResultSetIterator = this.container2.GetItemQueryIterator<ShoppingItem>(queryDefinition);
                    ShoppingItem gotItem = new ShoppingItem();
                    if (queryResultSetIterator.HasMoreResults)
                    {
                        FeedResponse<ShoppingItem> currentResult = await queryResultSetIterator.ReadNextAsync();

                        foreach (ShoppingItem itemRetrieved in currentResult)
                            gotItem = itemRetrieved;
                            
                        await this.container2.DeleteItemAsync<ShoppingItem>(itemName, new PartitionKey(gotItem.category));
                        Console.WriteLine("\n The item " + itemName + " has been successfully deleted. Redirecting to Manage Items...");
                    }
                    else
                        Console.WriteLine("The item " + itemName + "does not exist. Redirecting to Manage Items...");
                    break;
                case 3:
                    Console.Write("\nInsert the item name you want to change the price of: ");
                    string name2 = Console.ReadLine();

                    string checkItemName2 = "SELECT * FROM c WHERE c.ItemName = '" + name2 + "'";
                    QueryDefinition queryDefinition2 = new QueryDefinition(checkItemName2);
                    FeedIterator<ShoppingItem> queryResultSetIterator2 = this.container2.GetItemQueryIterator<ShoppingItem>(queryDefinition2);
                    ShoppingItem gotItem2 = new ShoppingItem();
                    if (queryResultSetIterator2.HasMoreResults)
                    {
                        FeedResponse<ShoppingItem> currentResult = await queryResultSetIterator2.ReadNextAsync();

                        foreach (ShoppingItem itemRetrieved in currentResult)
                            gotItem2 = itemRetrieved;

                        ItemResponse<ShoppingItem> itemOnDb = await this.container2.ReadItemAsync<ShoppingItem>(gotItem2.Id, new PartitionKey(gotItem2.category));
                        ShoppingItem itemBody = itemOnDb.Resource;
                        Console.Write("\nInsert the new price that would be adjusted to " + gotItem2.ItemName + ": ");
                        itemBody.price = Convert.ToDouble(Console.ReadLine());
                        await this.container2.ReplaceItemAsync<ShoppingItem>(itemBody, itemBody.Id, new PartitionKey(itemBody.category));
                        Console.WriteLine("\n The item " + name2 + " price has been successfully changed to " + "$" + itemBody.price + ". Redirecting to Manage Items...");

                    }
                    else
                        Console.WriteLine("The item " + name2 + "does not exist. Redirecting to Manage Items...");
                    break;
                default:
                    Console.WriteLine("\nRedirecting to main page....");
                    System.Threading.Thread.Sleep(4000);
                    Console.Clear();
                    await LoginManager(account);
                    break;
            }

            System.Threading.Thread.Sleep(4000);
            Console.Clear();
            await this.ManageItem(account);
        }
    }
}
