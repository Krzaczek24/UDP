using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;

namespace UDP
{
    public delegate void OnMessageReceivedEventHandler(string message);

    public interface IUdpListener
    {
        event OnMessageReceivedEventHandler OnMessageReceived;
        public string Receive();
    }

    public class UdpListener : UdpBase, IUdpListener
    {
        private new IPEndPoint EndPoint = new(IPAddress.Any, 0);
        private Func<string> receiveFunc;

        public UdpListener(int port, Encoding? encoding = null)
            : base(port, IPAddress.Any, encoding)
        {
            UdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            receiveFunc = () =>
            {
                UdpClient.Client.Bind(base.EndPoint);
                return (receiveFunc = ReceiveMessage)();
            };
        }

        public event OnMessageReceivedEventHandler OnMessageReceived = delegate { };

        public string Receive() => receiveFunc();

        private string ReceiveMessage()
        {
            string message = Encoding.GetString(UdpClient.Receive(ref EndPoint));
            OnMessageReceived(message);
            return message;
        }
    }

    public delegate void OnObjectReceivedEventHandler<T>(T? obj);

    public interface IUdpListener<T> where T : class
    {
        event OnObjectReceivedEventHandler<T> OnMessageReceived;
        public T? Receive();
    }

    public class UdpListener<T> : UdpBase, IUdpListener<T> where T : class
    {
        private new IPEndPoint EndPoint = new(IPAddress.Any, 0);
        private Func<T?> receiveFunc;

        public UdpListener(int port, Encoding? encoding = null)
            : base(port, IPAddress.Any, encoding)
        {
            UdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            receiveFunc = () =>
            {
                UdpClient.Client.Bind(base.EndPoint);
                return (receiveFunc = ReceiveMessage)();
            };
        }

        public event OnObjectReceivedEventHandler<T> OnMessageReceived = delegate { };

        public T? Receive() => receiveFunc();

        private T? ReceiveMessage()
        {
            string message = Encoding.GetString(UdpClient.Receive(ref EndPoint));
            T? obj = JsonSerializer.Deserialize<T?>(message);
            OnMessageReceived(obj);
            return obj;
        }
    }
}
