namespace UDP.Broadcaster
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, I'm BROADCASTER!");

            IUdpBroadcaster broadcaster = new UdpBroadcaster(12345);
            broadcaster.OnMessageSent += (msg) => Console.WriteLine($"Sent: '{msg}'");

            IUdpBroadcaster<MyObj> broadcaster2 = new UdpBroadcaster<MyObj>(10000);
            broadcaster2.OnMessageSent += (obj) => Console.WriteLine($"Sent: [{obj.Id}] '{obj.Message}'");

            int i = 0;
            var end = DateTimeOffset.Now + TimeSpan.FromSeconds(30);
            while (DateTimeOffset.Now < end)
            {
                broadcaster.Send($"> {++i}. part");
                Thread.Sleep(100);
                broadcaster.Send("Tarama sęktuma");
                Thread.Sleep(100);
                broadcaster.Send("Tulis manorę");
                Thread.Sleep(100);
                broadcaster.Send("Aja nostrę");
                Thread.Sleep(100);
                broadcaster2.Send(new MyObj() { Id = i, Message = "FORTIS FORTUNA ADIUVAT" });
                Thread.Sleep(1600);
            }

            Console.WriteLine("All messages were sent!");
            Console.Write("Press any key to continue ...");
            Console.ReadKey();
        }
    }

    public class MyObj
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
