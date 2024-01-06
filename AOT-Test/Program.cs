using System.Text.Json.Nodes;

namespace AOT_Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            JsonObject k = new JsonObject();
            k["a"] = 1;

            Console.WriteLine("Result:" + SharedMemory.WriteToSharedMemory("Hi!", "lalala"));

            Console.WriteLine(k);
 
            Console.ReadLine();
        }
    }
}
