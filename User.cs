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
        public List<ShoppingItem> ShoppingCart { get; set; } = null;
    }
}
