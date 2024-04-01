using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;


namespace UDP
{
    public delegate void OnMessageSentEventHandler(string message);

    public interface IUdpBroadcaster
    {
        event OnMessageSentEventHandler OnMessageSent;
        void Send(string message);
    }

    public class UdpBroadcaster(int port)
        : UdpBase(port, IPAddress.Broadcast, Encoding.UTF8), IUdpBroadcaster
    {
        public event OnMessageSentEventHandler OnMessageSent = delegate { };

        public void Send(string message)
        {
            UdpClient.Send(Encoding.GetBytes(message), EndPoint);
            OnMessageSent(message);
        }
    }

    public delegate void OnObjectSentEventHandler<T>(T obj);

    public interface IUdpBroadcaster<T> where T : class
    {
        event OnObjectSentEventHandler<T> OnMessageSent;
        void Send(T obj);
    }

    public class UdpBroadcaster<T>(int port)
        : UdpBase(port, IPAddress.Broadcast, Encoding.UTF8), IUdpBroadcaster<T>
        where T : class
    {
        public event OnObjectSentEventHandler<T> OnMessageSent = delegate { };

        public void Send(T obj)
        {
            string message = JsonSerializer.Serialize(obj);
            UdpClient.Send(Encoding.GetBytes(message), EndPoint);
            OnMessageSent(obj);
        }
    }
}
