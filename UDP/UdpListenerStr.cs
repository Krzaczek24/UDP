using System.Net.Sockets;
using System.Net;
using System.Text;

namespace UDP
{
    public delegate void OnMessageReceivedEventHandler(string message);

    public interface IUdpListener
    {
        event OnMessageReceivedEventHandler OnMessageReceived;
        Task ReceiveAsync();
    }

    public class UdpListener : UdpBase, IUdpListener
    {
        public event OnMessageReceivedEventHandler OnMessageReceived = delegate { };

        private Func<Task> receiveFunc;

        protected internal UdpListener(int port, Encoding? encoding = null) : base(port, IPAddress.Any, encoding)
        {
            UdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            receiveFunc = async () =>
            {
                UdpClient.Client.Bind(EndPoint);
                await (receiveFunc = Receive)();
            };
        }

        public async Task ReceiveAsync() => await receiveFunc();

        private async Task Receive()
        {
            var msg = await UdpClient.ReceiveAsync();
            string message = Encoding.UTF8.GetString(msg.Buffer, 0, msg.Buffer.Length);
            OnMessageReceived(message);
        }

        public static IUdpListener Create(int port, Encoding? encoding = null) => new UdpListener(port, encoding);
        public static IUdpListener<T> Create<T>(int port, Func<string, T>? deserializator = null, Encoding? encoding = null) where T : class => new UdpListener<T>(port, deserializator, encoding);
    }
}
