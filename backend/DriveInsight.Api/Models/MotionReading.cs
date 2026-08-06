namespace DriveInsight.Api.Models;

public class MotionReading
{
    public Guid Id { get; set; }

    public Guid DriveId { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public double AccelerometerX { get; set; }

    public double AccelerometerY { get; set; }

    public double AccelerometerZ { get; set; }

    public double? GyroscopeX { get; set; }

    public double? GyroscopeY { get; set; }

    public double? GyroscopeZ { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Drive? Drive { get; set; }
}
