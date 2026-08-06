namespace DriveInsight.Api.Models;

public class Drive
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTimeOffset StartedAt { get; set; }

    public DateTimeOffset? EndedAt { get; set; }

    public DriveStatus Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public User? User { get; set; }

    public List<GpsReading> GpsReadings { get; set; } = new();

    public List<MotionReading> MotionReadings { get; set; } = new();

    public List<DetectedEvent> DetectedEvents { get; set; } = new();

    public DriveAnalysis? Analysis { get; set; }
}
