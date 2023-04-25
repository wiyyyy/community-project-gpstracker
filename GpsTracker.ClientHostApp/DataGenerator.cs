using GpsTracker.ClientHostApp;
using Microsoft.Extensions.Hosting;

internal class DataGenerator : BackgroundService
{
    private readonly IClusterClient _orleansClient;
    public DataGenerator(IClusterClient orleansClient)
    {
        _orleansClient = orleansClient;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return LoadDriver.DriveLoad(_orleansClient, 500, stoppingToken);
    }
}