using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UDP
{
    public delegate void OnMessageSentEventHandler(string message);

    public interface IUdpBroadcaster
    {
        event OnMessageSentEventHandler OnMessageSent;
        Task SendAsync(string message);
    }

    public class UdpBroadcaster : UdpBase, IUdpBroadcaster
    {
        public event OnMessageSentEventHandler OnMessageSent = delegate { };

        protected internal UdpBroadcaster(int port, Encoding? encoding = null) : base(port, IPAddress.Broadcast, encoding) { }

        public async Task SendAsync(string message)
        {
            await UdpClient.SendAsync(Encoding.GetBytes(message), EndPoint);
            OnMessageSent(message);
        }

        public static IUdpBroadcaster Create(int port, Encoding? encoding = null) => new UdpBroadcaster(port, encoding);
        public static IUdpBroadcaster<T> Create<T>(int port, Func<T, string>? serializator = null, Encoding? encoding = null) where T : class => new UdpBroadcaster<T>(port, serializator, encoding);
    }
}
