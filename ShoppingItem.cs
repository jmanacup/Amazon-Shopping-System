using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace FirstProjectinCSHARP
{
   public class ShoppingItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string ItemName { get; set; }
        public string category { get; set; }
        public double price { get; set; }
        public int quantity { get; set; }
        public override string ToString()
        {
            string presentItem = String.Format("{0,10}\t\t\t{1,10}\t\t\t{2,7}\t\t\t{3,7}", ItemName, category, price, quantity);
            return presentItem;
        }
    }
}
