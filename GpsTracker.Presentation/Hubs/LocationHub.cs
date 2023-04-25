using Microsoft.AspNetCore.SignalR;

namespace GpsTracker.Presentation.Hubs
{
    public sealed class LocationHub : Hub
    {
        public LocationHub()
        {

        }
    }

    public record LocationResponse
    (
        double Latitude,
        double Longitude,
        long MessageId,
        int DeviceId,
        DateTime Timestamp
    );
}
