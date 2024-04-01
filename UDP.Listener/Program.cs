namespace UDP.Listener
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, I'm LISTENER!");
            Console.WriteLine("Let's go!");

            IUdpListener listener = new UdpListener(12345);
            listener.OnMessageReceived += (msg) => Console.WriteLine($"Received: '{msg}'");

            IUdpListener<MyObj> listener2 = new UdpListener<MyObj>(10000);
            listener2.OnMessageReceived += (obj) => Console.WriteLine($"Received: [{obj?.Id}] '{obj?.Message}'");

            var end = DateTimeOffset.Now + TimeSpan.FromSeconds(10);
            while (DateTimeOffset.Now < end)
            {
                listener.Receive();                
                listener2.Receive();
                Thread.Sleep(10);
            }

            Console.WriteLine("I finished listenting!");
            Console.Write("Press any key to continue ...");
            Console.ReadKey();
        }

        public class MyObj
        {
            public int Id { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }
}
