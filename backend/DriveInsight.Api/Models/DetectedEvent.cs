namespace DriveInsight.Api.Models;

public class DetectedEvent
{
    public Guid Id { get; set; }

    public Guid DriveId { get; set; }

    public DetectedEventType Type { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public EventSeverity Severity { get; set; }

    public double Value { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Drive? Drive { get; set; }
}
