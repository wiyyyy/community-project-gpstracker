using GpsTracker.GrainInterfaces;
using GpsTracker.Models;
using Orleans.Runtime;
using System.Collections.Concurrent;

namespace GpsTracker.Grains
{
    internal class DataCollectorGrain : Grain, IDataCollector
    {
        private readonly IPersistentState<ConcurrentDictionary<string, string>> _data;

        public DataCollectorGrain([PersistentState("gpsData", "gpsDataStore")] IPersistentState<ConcurrentDictionary<string, string>> data)
        {
            _data = data;
        }

        public ValueTask Collect(DeviceMessage json)
        {
            //_data.State.AddOrUpdate(this.GetPrimaryKey().ToString(), json, (_, __) => json);
            //await _data.WriteStateAsync();
            var channelGrain = GrainFactory.GetGrain<IChannelGrain>(0);
            return channelGrain.WriteAsync(json);
        }
    }
}
