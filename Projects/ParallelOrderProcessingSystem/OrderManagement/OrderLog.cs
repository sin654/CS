using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelOrderProcessingSystem.OrderManagement
{
    class OrderLog
    {
        private object ThreadLock  = new object(); // Thread lock object
        private List<Order> processedOrders = new List<Order>(); // List to store processed orders

        public void AddProcessedOrder(Order order)
        {
            lock (ThreadLock) // Locking to ensure thread safety
            {
                processedOrders.Add(order);
                //Console.WriteLine($"Order {order.Id} processed and added to log.");
            }
        }

        public void Print()
        {
            foreach (Order order in processedOrders)
            {
                Console.WriteLine($"{order} processed.");
            }
        }
    }
}
