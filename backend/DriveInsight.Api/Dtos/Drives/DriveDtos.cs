namespace DriveInsight.Api.Dtos.Drives;

public class CreateDriveRequest
{
    public DateTimeOffset? StartedAt { get; set; }
}

public class CreateDriveResponse
{
    public Guid DriveId { get; set; }

    public string Status { get; set; } = string.Empty;
}

public class AddReadingsRequest
{
    public List<GpsReadingRequest> GpsReadings { get; set; } = new();

    public List<MotionReadingRequest> MotionReadings { get; set; } = new();
}

public class GpsReadingRequest
{
    public DateTimeOffset Timestamp { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public double? SpeedMps { get; set; }

    public double? AccuracyMeters { get; set; }
}

public class MotionReadingRequest
{
    public DateTimeOffset Timestamp { get; set; }

    public Vector3Request? Accelerometer { get; set; }

    public Vector3Request? Gyroscope { get; set; }
}

public class Vector3Request
{
    public double X { get; set; }

    public double Y { get; set; }

    public double Z { get; set; }
}

public class AddReadingsResponse
{
    public int AcceptedGpsCount { get; set; }

    public int AcceptedMotionCount { get; set; }
}

public class FinishDriveRequest
{
    public DateTimeOffset? EndedAt { get; set; }
}

public class FinishDriveResponse
{
    public Guid DriveId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string AnalysisStatus { get; set; } = string.Empty;
}

public class DriveListItemResponse
{
    public Guid DriveId { get; set; }

    public DateTimeOffset StartedAt { get; set; }

    public DateTimeOffset? EndedAt { get; set; }

    public string Status { get; set; } = string.Empty;

    public int? DurationSeconds { get; set; }

    public double? DistanceMeters { get; set; }

    public double? AverageSpeedKmh { get; set; }

    public double? MaxSpeedKmh { get; set; }

    public int? Score { get; set; }

    public int EventCount { get; set; }
}

public class DriveDetailResponse
{
    public Guid DriveId { get; set; }

    public DateTimeOffset StartedAt { get; set; }

    public DateTimeOffset? EndedAt { get; set; }

    public string Status { get; set; } = string.Empty;

    public List<RoutePointResponse> Route { get; set; } = new();

    public DriveAnalysisResponse? Analysis { get; set; }

    public List<DetectedEventResponse> Events { get; set; } = new();
}

public class RoutePointResponse
{
    public DateTimeOffset Timestamp { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public double? SpeedKmh { get; set; }
}

public class DriveAnalysisResponse
{
    public int DurationSeconds { get; set; }

    public double DistanceMeters { get; set; }

    public double AverageSpeedKmh { get; set; }

    public double MaxSpeedKmh { get; set; }

    public int HarshAccelerationCount { get; set; }

    public int HarshBrakingCount { get; set; }

    public int SharpTurnCount { get; set; }

    public int Score { get; set; }

    public string? Summary { get; set; }
}

public class DetectedEventResponse
{
    public string Type { get; set; } = string.Empty;

    public DateTimeOffset Timestamp { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string Severity { get; set; } = string.Empty;

    public double Value { get; set; }
}
