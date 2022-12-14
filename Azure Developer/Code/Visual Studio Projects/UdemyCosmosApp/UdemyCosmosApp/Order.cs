using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyCosmosApp
{
    public class Order
    {
        public string id { get; set; }
        public string orderId { get; set; }
        public string category { get; set; }
        public int quantity { get; set; }
        public DateTime creationTime { get; set; }

        public override string ToString()
        {
            return $"ID: {id}, Order ID: {orderId}, Category: {category}, Quantity: {quantity}, Creation Time: {creationTime}";
        }
    }
}
