using DriveInsight.Api.Models;

namespace DriveInsight.Api.Services;

public interface IDriveAnalysisService
{
    DriveAnalysisResult Analyze(
        Drive drive,
        IReadOnlyCollection<GpsReading> gpsReadings,
        IReadOnlyCollection<MotionReading> motionReadings,
        DateTimeOffset createdAt);
}

public class DriveAnalysisService : IDriveAnalysisService{

    private const double MaxGpsAccuracyMeters = 50;
    private const double HarshAccelerationThreshold = 3.0;
    private const double HarshBrakingThreshold = -3.0;
    private const double SharpTurnThreshold = 1.5;
    private static readonly TimeSpan EventCooldown = TimeSpan.FromSeconds(2);

    public DriveAnalysisResult Analyze(
        Drive drive,
        IReadOnlyCollection<GpsReading> gpsReadings,
        IReadOnlyCollection<MotionReading> motionReadings,
        DateTimeOffset createdAt)
    {
        var usableGpsReadings = gpsReadings
            .Where(IsUsableGpsReading)
            .OrderBy(reading => reading.Timestamp)
            .ToList();

        var events = DetectEvents(drive.Id, motionReadings, usableGpsReadings, createdAt);
        var harshAccelerationCount = events.Count(detectedEvent => detectedEvent.Type == DetectedEventType.HarshAcceleration);
        var harshBrakingCount = events.Count(detectedEvent => detectedEvent.Type == DetectedEventType.HarshBraking);
        var sharpTurnCount = events.Count(detectedEvent => detectedEvent.Type == DetectedEventType.SharpTurn);

        var durationSeconds = CalculateDurationSeconds(drive);
        var distanceMeters = CalculateDistanceMeters(usableGpsReadings);
        var averageSpeedKmh = durationSeconds == 0 ? 0 : distanceMeters / durationSeconds * 3.6;
        var maxSpeedKmh = usableGpsReadings
            .Where(reading => reading.SpeedMps.HasValue)
            .Select(reading => reading.SpeedMps!.Value * 3.6)
            .DefaultIfEmpty(0)
            .Max();
        var score = CalculateScore(harshAccelerationCount, harshBrakingCount, sharpTurnCount);

        var analysis = new DriveAnalysis
        {
            Id = Guid.NewGuid(),
            DriveId = drive.Id,
            DurationSeconds = durationSeconds,
            DistanceMeters = Math.Round(distanceMeters, 2),
            AverageSpeedKmh = Math.Round(averageSpeedKmh, 2),
            MaxSpeedKmh = Math.Round(maxSpeedKmh, 2),
            HarshAccelerationCount = harshAccelerationCount,
            HarshBrakingCount = harshBrakingCount,
            SharpTurnCount = sharpTurnCount,
            Score = score,
            Summary = CreateSummary(score),
            CreatedAt = createdAt
        };

        return new DriveAnalysisResult(analysis, events);
    }

    private static List<DetectedEvent> DetectEvents(
        Guid driveId,
        IEnumerable<MotionReading> motionReadings,
        IReadOnlyList<GpsReading> usableGpsReadings,
        DateTimeOffset createdAt)
    {
        var events = new List<DetectedEvent>();
        if (usableGpsReadings.Count == 0)
        {
            return events;
        }

        var lastEventByType = new Dictionary<DetectedEventType, DateTimeOffset>();
        foreach (var reading in motionReadings.OrderBy(reading => reading.Timestamp))
        {
            if (reading.AccelerometerX > HarshAccelerationThreshold)
            {
                AddEventIfAllowed(
                    events,
                    lastEventByType,
                    driveId,
                    DetectedEventType.HarshAcceleration,
                    reading.Timestamp,
                    reading.AccelerometerX,
                    GetAccelerationSeverity(reading.AccelerometerX),
                    usableGpsReadings,
                    createdAt);
            }

            if (reading.AccelerometerX < HarshBrakingThreshold)
            {
                AddEventIfAllowed(
                    events,
                    lastEventByType,
                    driveId,
                    DetectedEventType.HarshBraking,
                    reading.Timestamp,
                    reading.AccelerometerX,
                    GetAccelerationSeverity(Math.Abs(reading.AccelerometerX)),
                    usableGpsReadings,
                    createdAt);
            }

            if (reading.GyroscopeZ.HasValue && Math.Abs(reading.GyroscopeZ.Value) > SharpTurnThreshold)
            {
                AddEventIfAllowed(
                    events,
                    lastEventByType,
                    driveId,
                    DetectedEventType.SharpTurn,
                    reading.Timestamp,
                    reading.GyroscopeZ.Value,
                    GetSharpTurnSeverity(Math.Abs(reading.GyroscopeZ.Value)),
                    usableGpsReadings,
                    createdAt);
            }
        }

        return events;
    }

    private static void AddEventIfAllowed(
        List<DetectedEvent> events,
        Dictionary<DetectedEventType, DateTimeOffset> lastEventByType,
        Guid driveId,
        DetectedEventType type,
        DateTimeOffset timestamp,
        double value,
        EventSeverity severity,
        IReadOnlyList<GpsReading> usableGpsReadings,
        DateTimeOffset createdAt)
    {
        if (lastEventByType.TryGetValue(type, out var lastEventAt) && timestamp - lastEventAt < EventCooldown)
        {
            return;
        }

        var nearestGps = FindNearestGpsReading(usableGpsReadings, timestamp);
        events.Add(new DetectedEvent
        {
            Id = Guid.NewGuid(),
            DriveId = driveId,
            Type = type,
            Timestamp = timestamp,
            Latitude = nearestGps.Latitude,
            Longitude = nearestGps.Longitude,
            Severity = severity,
            Value = Math.Round(value, 3),
            CreatedAt = createdAt
        });

        lastEventByType[type] = timestamp;
    }

    private static GpsReading FindNearestGpsReading(IReadOnlyList<GpsReading> gpsReadings, DateTimeOffset timestamp)
    {
        return gpsReadings
            .MinBy(reading => Math.Abs((reading.Timestamp - timestamp).TotalMilliseconds))!;
    }

    private static int CalculateDurationSeconds(Drive drive)
    {
        if (drive.EndedAt is null)
        {
            return 0;
        }

        return (int)Math.Max(0, Math.Round((drive.EndedAt.Value - drive.StartedAt).TotalSeconds));
    }

    private static double CalculateDistanceMeters(IReadOnlyList<GpsReading> gpsReadings)
    {
        var distanceMeters = 0.0;
        for (var i = 1; i < gpsReadings.Count; i++)
        {
            distanceMeters += CalculateDistanceMeters(gpsReadings[i - 1], gpsReadings[i]);
        }

        return distanceMeters;
    }

    private static double CalculateDistanceMeters(GpsReading from, GpsReading to)
    {
        const double earthRadiusMeters = 6371000;

        var fromLatitudeRadians = DegreesToRadians(from.Latitude);
        var toLatitudeRadians = DegreesToRadians(to.Latitude);
        var latitudeDeltaRadians = DegreesToRadians(to.Latitude - from.Latitude);
        var longitudeDeltaRadians = DegreesToRadians(to.Longitude - from.Longitude);

        var haversine =
            Math.Sin(latitudeDeltaRadians / 2) * Math.Sin(latitudeDeltaRadians / 2) +
            Math.Cos(fromLatitudeRadians) * Math.Cos(toLatitudeRadians) *
            Math.Sin(longitudeDeltaRadians / 2) * Math.Sin(longitudeDeltaRadians / 2);

        var centralAngle = 2 * Math.Atan2(Math.Sqrt(haversine), Math.Sqrt(1 - haversine));
        return earthRadiusMeters * centralAngle;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    private static int CalculateScore(int harshAccelerationCount, int harshBrakingCount, int sharpTurnCount)
    {
        var score = 100;
        score -= harshAccelerationCount * 5;
        score -= harshBrakingCount * 7;
        score -= sharpTurnCount * 4;

        return Math.Clamp(score, 0, 100);
    }

    private static EventSeverity GetAccelerationSeverity(double value)
    {
        return value switch
        {
            > 5.0 => EventSeverity.High,
            > 4.0 => EventSeverity.Medium,
            _ => EventSeverity.Low
        };
    }

    private static EventSeverity GetSharpTurnSeverity(double value)
    {
        return value switch
        {
            > 2.8 => EventSeverity.High,
            > 2.0 => EventSeverity.Medium,
            _ => EventSeverity.Low
        };
    }

    private static string CreateSummary(int score)
    {
        return score switch
        {
            >= 85 => "The drive was mostly stable, with few risky driving events.",
            >= 65 => "The drive was acceptable, but some sudden maneuvers were detected.",
            _ => "The drive contains multiple risky events. Smoother acceleration, braking, and turning are recommended."
        };
    }

    private static bool IsUsableGpsReading(GpsReading reading)
    {
        return reading.AccuracyMeters is null || reading.AccuracyMeters <= MaxGpsAccuracyMeters;
    }
}

public record DriveAnalysisResult(DriveAnalysis Analysis, List<DetectedEvent> Events);
