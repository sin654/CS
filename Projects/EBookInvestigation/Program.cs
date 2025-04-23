using System.Text;
using System.Net;

namespace EBookInvestigation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BookReader bookReader = new BookReader();
            bookReader.GetBook();
            Console.ReadLine();
        }

        void GetBook()
        {

        }
    }
}
