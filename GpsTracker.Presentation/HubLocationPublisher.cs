using GpsTracker.GrainInterfaces;
using GpsTracker.Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;

public class HubLocationPublisher : BackgroundService
{
    private readonly IClusterClient _orleansClusterClient;
    private readonly IHubContext<LocationHub> _hubContext;
    public HubLocationPublisher(IClusterClient orleansClusterClient, IHubContext<LocationHub> hubContext)
    {
        _orleansClusterClient = orleansClusterClient;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        var channelGrain = _orleansClusterClient.GetGrain<IChannelGrain>(0);

        while (!stoppingToken.IsCancellationRequested)
        {
            var data = await channelGrain.ReadAsync();

            if (data.MessageId == -1) continue;

            await _hubContext.Clients.All.SendAsync("locationUpdates", data, cancellationToken: stoppingToken);
        }
    }
}