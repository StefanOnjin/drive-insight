namespace DriveInsight.Api.Models;

public class GpsReading
{
    public Guid Id { get; set; }

    public Guid DriveId { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public double? SpeedMps { get; set; }

    public double? AccuracyMeters { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Drive? Drive { get; set; }
}
