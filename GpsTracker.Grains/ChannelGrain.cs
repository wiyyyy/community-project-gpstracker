using GpsTracker.GrainInterfaces;
using GpsTracker.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace GpsTracker.Grains
{
    public class ChannelGrain : Grain, IChannelGrain
    {
        private readonly Channel<DeviceMessage> _channel;

        public ChannelGrain()
        {
            _channel ??= Channel.CreateUnbounded<DeviceMessage>();
        }

        public ValueTask<DeviceMessage> ReadAsync()
        {
            if (_channel.Reader.TryRead(out var data))
            {
                return ValueTask.FromResult(data);
            }
            return ValueTask.FromResult(new DeviceMessage(-1 , -1, -1, Guid.Empty,DateTime.MinValue));
        }

        public async ValueTask WriteAsync(DeviceMessage json)
        {
            await _channel.Writer.WriteAsync(json);
        }
    }
}
