using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace FirstProjectinCSHARP
{
    public class User
    {
        //sets the unique identifier as the username
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAdd { get; set; }
        public string PhyAdd { get; set; }
        public int age { get; set; }
        public List<ShoppingItem> ShoppingCart { get; set; } = new List<ShoppingItem>();
        public bool isManager { get; set; }
        public void showProfile()
        {
            Console.WriteLine("\t\t [1]       Username: " + UserName);
            Console.WriteLine("\t\t [2]        Password: " + password);
            Console.WriteLine("\t\t [3]      First Name: " + FirstName);
            Console.WriteLine("\t\t [4]       Last Name: " + LastName);
            Console.WriteLine("\t\t [5]   Email Address: " + EmailAdd);
            Console.WriteLine("\t\t [6]Physical Address: " + PhyAdd);
            Console.WriteLine("\t\t [7]             Age: " + age);

        }

        public override string ToString()
        {
           return JsonConvert.SerializeObject(this);
        }
    }
}
