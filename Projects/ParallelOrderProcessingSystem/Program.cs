using ParallelOrderProcessingSystem.OrderManagement;

namespace ParallelOrderProcessingSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Create orders
            OrderProcessor orderProcessor = new OrderProcessor(); // Create an instance of OrderProcessor
            for (int i = 1; i <= 50; i++)
            {
                Order order = new Order
                {
                    Id = i,
                    CustomerName = $"Customer {i}",
                    Amount = i * 100.0m // Example: Amount increases by 100 for each order
                };

                orderProcessor.AddOrder(order); // Add order to the processor
            }

            // Process orders
            orderProcessor.Process();

            // Print log
            orderProcessor.PrintLog();
        }
    }
}
