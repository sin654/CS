using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelOrderProcessingSystem.OrderManagement
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = String.Empty;
        public decimal Amount { get; set; }

        public override string ToString()
        {
            return $"Order ID: {Id}, Customer: {CustomerName}, Amount: {Amount}";
        }
    }
}
