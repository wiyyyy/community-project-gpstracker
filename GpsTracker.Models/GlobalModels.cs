namespace GpsTracker.Models
{
    public class GlobalModels
    {

    }

    [Immutable, GenerateSerializer]
    public record class DeviceMessage(
    double Latitude,
    double Longitude,
    long MessageId,
    Guid DeviceId,
    DateTime Timestamp);
}