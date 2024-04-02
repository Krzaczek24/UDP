using System.Net;
using System.Text;
using System.Text.Json;

namespace UDP
{
    public delegate void OnObjectSentEventHandler<T>(T obj);

    public interface IUdpBroadcaster<T> where T : class
    {
        event OnObjectSentEventHandler<T> OnMessageSent;
        Task SendAsync(T obj);
    }

    public class UdpBroadcaster<T> : UdpBase, IUdpBroadcaster<T> where T : class
    {
        public event OnObjectSentEventHandler<T> OnMessageSent = delegate { };

        private readonly Func<T, Task<byte[]>> serializeFunc;

        protected internal UdpBroadcaster(int port, Func<T, string>? serializator = null, Encoding? encoding = null) : base(port, IPAddress.Broadcast, encoding)
        {
            serializeFunc = serializator == null ? Serialize : (T obj) => Serialize(obj, serializator);
        }

        public async Task SendAsync(T obj)
        {
            byte[] bytes = await serializeFunc(obj);
            _ = await UdpClient.SendAsync(bytes, EndPoint);
            OnMessageSent(obj);
        }

        private async Task<byte[]> Serialize(T obj, Func<T, string> serializator)
        {
            string message = serializator(obj);
            byte[] bytes = Encoding.GetBytes(message);
            return await Task.FromResult(bytes);
        }

        private static async Task<byte[]> Serialize(T obj)
        {
            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, obj);
            return stream.ToArray();
        }
    }
}
