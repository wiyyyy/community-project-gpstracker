using GpsTracker.GrainInterfaces;
using GpsTracker.Models;
using System.Diagnostics;

namespace GpsTracker.ClientHostApp;

public static class LoadDriver
{
    private static Random Random => Random.Shared;

    // San Francisco: approximate boundaries.
    private const double IstanbulPendikLatMin = 35.680;
    private const double IstanbulPendikLatMax = 41.901;
    private const double IstanbulPendikLonMin = 26.286;
    private const double IstanbulPendikLonMax = 44.538;

    private static int s_counter = 0;

    public static async Task DriveLoad(IGrainFactory client, int numDevices, CancellationToken cancellationToken)
    {
        // Simulate some devices
        var deviceModels = new List<Model>();
        for (int i = 0; i < numDevices; i++)
        {
            deviceModels.Add(new Model
            {
                DeviceId = Guid.NewGuid(),
                Lat = NextDouble(IstanbulPendikLatMin, IstanbulPendikLatMax),
                Lon = NextDouble(IstanbulPendikLonMin, IstanbulPendikLonMax),
                Direction = NextDouble(-Math.PI, Math.PI),
                Speed = NextDouble(0, 0.0005)
            });
        }

        // Update each device in a loop.
        var tasks = new List<Task>();
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (Model model in deviceModels)
            {
                tasks.Add(DeviceLoop(client, model, cancellationToken));
            }

            await Task.WhenAll(tasks);
            tasks.Clear();
        }
    }

    private static async Task DeviceLoop(IGrainFactory grainFactory, Model model, CancellationToken cancellationToken, int count = -1)
    {
        await Task.Yield();

        // Send the message to the service
        IDataCollector device = grainFactory.GetGrain<IDataCollector>(model.DeviceId);
        IChannelGrain channelGrain = grainFactory.GetGrain<IChannelGrain>(0);

        while (!cancellationToken.IsCancellationRequested && (count == -1 || count-- > 0))
        {
            try
            {
                // There is nothing particular about these values, they are just simulating a random walk.
                double delta = model.TimeSinceLastUpdate.Elapsed.TotalMilliseconds;

                // Simulate the device moving
                model.Acceleration = Math.Clamp(model.Acceleration + delta / 100 * NextDouble(-0.0005, 0.0005), -10, 10);
                model.Speed = Math.Clamp(model.Speed + model.Acceleration * delta / 100, -0.0005, 0.0005);

                model.AngularVelocity = Math.Clamp(model.AngularVelocity + delta * NextDouble(-0.005, 0.005), -0.01, 0.01);
                model.Direction += model.AngularVelocity * delta;

                double lastLat = model.Lat;
                double lastLon = model.Lon;

                UpdateDevicePosition(model, delta);

                if (lastLat == model.Lat || lastLon == model.Lon)
                {
                    // The device has hit the boundary, so change direction.
                    model.Direction += NextDouble(-Math.PI, Math.PI);

                    UpdateDevicePosition(model, delta);
                }

                model.TimeSinceLastUpdate.Restart();

                await device.Collect(
                    new DeviceMessage(
                        model.Lat,
                        model.Lon,
                        ++model.MessageId,
                        model.DeviceId,
                        DateTime.UtcNow)).ConfigureAwait(false);

                Interlocked.Increment(ref s_counter);

                // Ensure at least 15ms passes between updates
                await Task.Delay(TimeSpan.FromMilliseconds(Math.Max(0, 15 - model.TimeSinceLastUpdate.ElapsedMilliseconds)));
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception sending message: {exception}");
            }
        }
    }

    private static void UpdateDevicePosition(Model model, double delta)
    {
        model.Lat += Math.Cos(model.Direction) * (model.Speed * delta / 10);
        model.Lon += Math.Sin(model.Direction) * (model.Speed * delta / 10);
        model.Lat = Math.Clamp(model.Lat, IstanbulPendikLatMin, IstanbulPendikLatMax);
        model.Lon = Math.Clamp(model.Lon, IstanbulPendikLonMin, IstanbulPendikLonMax);
    }

    public static double NextDouble(double min, double max) => Random.NextDouble() * (max - min) + min;

    private class Model
    {
        public Stopwatch TimeSinceLastUpdate { get; } = Stopwatch.StartNew();
        public Guid DeviceId { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Direction { get; set; }
        public double AngularVelocity { get; set; }
        public double Acceleration { get; set; }
        public double Speed { get; set; }
        public long MessageId { get; set; }
    }
}
