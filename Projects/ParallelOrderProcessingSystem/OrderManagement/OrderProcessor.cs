using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelOrderProcessingSystem.OrderManagement
{
    class OrderProcessor
    {
        private Stack<Order> orderStack = new Stack<Order>(); // Stack to hold orders
        private OrderLog _log;
        private List<Thread> threads = new List<Thread>(); // List to hold threads

        public OrderProcessor() => _log = new OrderLog(); // Initialize the order log;
        public OrderProcessor(OrderLog log)
        {
            _log = log; // Initialize the order log;
        }

        public void AddOrder(Order order)
        {
            orderStack.Push(order); // Add order to the stack
        }

        public void Process()
        {
            while(orderStack.Count > 0)
            {
                Order order = orderStack.Pop(); // Get the order from the stack
                ProcessOrder(order); // Process the order
            }
        }

        private void ProcessOrder(Order order)
        {
            // Simulate order processing
            Console.WriteLine($"Processing order {order.Id} for customer {order.CustomerName}");
            // Add logic to process the order
            Random random = new Random();

            Thread thread = new Thread(() =>
            {
                Thread.Sleep(random.Next(200, 500)); // Simulate processing time
                Console.WriteLine($"Order {order.Id} processed successfully.");
                _log.AddProcessedOrder(order); // Log the order
            });
            threads.Add(thread);
            thread.Start();
        }

        public void PrintLog()
        {
            foreach (Thread thread in threads)
            {
                thread.Join(); // Wait for all threads to finish
            }
            threads.Clear(); // Clear the thread list

            _log.Print(); // Print the log
        }
    }
}
