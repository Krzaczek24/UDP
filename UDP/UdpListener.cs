using System.Net.Sockets;
using System.Net;
using System.Text;

namespace UDP
{
    public interface IUdpListener
    {
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

        public string Receive() => receiveFunc();

        private string ReceiveMessage() => Encoding.GetString(UdpClient.Receive(ref EndPoint));
    }
}
