namespace DriveInsight.Api.Models;

public class DriveAnalysis
{
    public Guid Id { get; set; }

    public Guid DriveId { get; set; }

    public int DurationSeconds { get; set; }

    public double DistanceMeters { get; set; }

    public double AverageSpeedKmh { get; set; }

    public double MaxSpeedKmh { get; set; }

    public int HarshAccelerationCount { get; set; }

    public int HarshBrakingCount { get; set; }

    public int SharpTurnCount { get; set; }

    public int Score { get; set; }

    public string? Summary { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Drive? Drive { get; set; }
}
