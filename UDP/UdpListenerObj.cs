using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;

namespace UDP
{
    public delegate void OnObjectReceivedEventHandler<T>(T? obj);

    public interface IUdpListener<T> where T : class
    {
        event OnObjectReceivedEventHandler<T> OnMessageReceived;        
        Task ReceiveAsync();
    }

    public class UdpListener<T> : UdpBase, IUdpListener<T> where T : class
    {
        public event OnObjectReceivedEventHandler<T> OnMessageReceived = delegate { };

        private Func<Task> receiveFunc;
        private readonly Func<byte[], Task<T?>> deserializeFunc;

        protected internal UdpListener(int port, Func<string, T>? deserializator = null, Encoding? encoding = null) : base(port, IPAddress.Any, encoding)
        {
            UdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            receiveFunc = async () =>
            {
                UdpClient.Client.Bind(EndPoint);
                await (receiveFunc = Receive)();
            };
            deserializeFunc = deserializator == null ? Deserialize : (byte[] bytes) => Deserialize(bytes, deserializator);
        }

        public async Task ReceiveAsync() => await receiveFunc();

        private async Task Receive()
        {
            var msg = await UdpClient.ReceiveAsync();
            T? obj = await deserializeFunc(msg.Buffer);
            OnMessageReceived(obj);
        }

        private async Task<T?> Deserialize(byte[] bytes, Func<string, T> deserializator)
        {
            string message = Encoding.GetString(bytes, 0, bytes.Length);
            return await Task.FromResult(deserializator(message));
        }

        private static async Task<T?> Deserialize(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            return await JsonSerializer.DeserializeAsync<T?>(stream);
        }
    }
}
