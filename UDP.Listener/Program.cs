namespace UDP.Listener
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, I'm LISTENER!");
            Console.WriteLine("Let's go!");
            var listener = new UdpListener(12345);

            var end = DateTimeOffset.Now + TimeSpan.FromSeconds(10);
            while (DateTimeOffset.Now < end)
            {
                Console.WriteLine($"Received: '{listener.Receive()}'");
                Thread.Sleep(10);
            }

            Console.WriteLine("I finished listenting!");
            Console.Write("Press any key to continue ...");
            Console.ReadKey();
        }
    }
}
