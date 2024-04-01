using System.Net.Sockets;
using System.Net;
using System.Text;

namespace UDP
{
    public abstract class UdpBase(int port, IPAddress address, Encoding? encoding = null) : IDisposable
    {
        protected Encoding Encoding { get; } = encoding ?? Encoding.UTF8;
        protected IPEndPoint EndPoint { get; } = new IPEndPoint(address, port);
        protected UdpClient UdpClient { get; } = new UdpClient();

        public void Dispose()
        {
            UdpClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
