using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyCosmosApp
{
    public class Customer
    {
        [JsonProperty("id")]
        public string customerId { get; set; }
        public string customerName { get; set; }
        public string customerCity { get; set; }
        public List<Order> orders { get; set; }

        public override string ToString()
        {
            return $"Customer ID: {customerId}, Customer Name: {customerName}, Customer City: {customerCity}, Orders: {String.Join(", ", orders)}";
        }
    }
}
