using System.Net.Sockets;
using System.Net;
using System.Text;


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
}
