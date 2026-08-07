using DriveInsight.Api.Data;
using DriveInsight.Api.Dtos.Drives;
using DriveInsight.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DriveInsight.Api.Services;

public interface IDriveService
{
    Task<CreateDriveResponse> CreateDriveAsync(CreateDriveRequest request, CancellationToken cancellationToken);

    Task<AddReadingsResponse> AddReadingsAsync(Guid driveId, AddReadingsRequest request, CancellationToken cancellationToken);

    Task<FinishDriveResponse> FinishDriveAsync(Guid driveId, FinishDriveRequest request, CancellationToken cancellationToken);

    Task<List<DriveListItemResponse>> GetDrivesAsync(CancellationToken cancellationToken);

    Task<DriveDetailResponse> GetDriveAsync(Guid driveId, CancellationToken cancellationToken);
}

public class DriveService : IDriveService
{
    private static readonly Guid DemoUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private readonly DriveInsightDbContext dbContext;

    public DriveService(DriveInsightDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<CreateDriveResponse> CreateDriveAsync(CreateDriveRequest request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var user = await EnsureDemoUserAsync(now, cancellationToken);

        var drive = new Drive
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            StartedAt = request.StartedAt ?? now,
            Status = DriveStatus.Recording,
            CreatedAt = now
        };

        dbContext.Drives.Add(drive);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateDriveResponse
        {
            DriveId = drive.Id,
            Status = ToApiStatus(drive.Status)
        };
    }

    public async Task<AddReadingsResponse> AddReadingsAsync(Guid driveId, AddReadingsRequest request, CancellationToken cancellationToken)
    {
        var requestGpsReadings = request.GpsReadings ?? new List<GpsReadingRequest>();
        var requestMotionReadings = request.MotionReadings ?? new List<MotionReadingRequest>();

        if (requestGpsReadings.Count == 0 && requestMotionReadings.Count == 0)
        {
            throw new DriveServiceException(DriveServiceError.Validation, "Batch must contain at least one GPS or motion reading.");
        }

        ValidateGpsReadings(requestGpsReadings);
        ValidateMotionReadings(requestMotionReadings);

        var drive = await dbContext.Drives.FirstOrDefaultAsync(drive => drive.Id == driveId, cancellationToken);
        if (drive is null)
        {
            throw new DriveServiceException(DriveServiceError.NotFound, "Drive was not found.");
        }

        if (drive.Status != DriveStatus.Recording)
        {
            throw new DriveServiceException(DriveServiceError.Conflict, "Readings can only be added while the drive is recording.");
        }

        var now = DateTimeOffset.UtcNow;

        var gpsReadings = requestGpsReadings.Select(reading => new GpsReading
        {
            Id = Guid.NewGuid(),
            DriveId = drive.Id,
            Timestamp = reading.Timestamp,
            Latitude = reading.Latitude,
            Longitude = reading.Longitude,
            SpeedMps = reading.SpeedMps,
            AccuracyMeters = reading.AccuracyMeters,
            CreatedAt = now
        });

        var motionReadings = requestMotionReadings.Select(reading => new MotionReading
        {
            Id = Guid.NewGuid(),
            DriveId = drive.Id,
            Timestamp = reading.Timestamp,
            AccelerometerX = reading.Accelerometer!.X,
            AccelerometerY = reading.Accelerometer.Y,
            AccelerometerZ = reading.Accelerometer.Z,
            GyroscopeX = reading.Gyroscope?.X,
            GyroscopeY = reading.Gyroscope?.Y,
            GyroscopeZ = reading.Gyroscope?.Z,
            CreatedAt = now
        });

        dbContext.GpsReadings.AddRange(gpsReadings);
        dbContext.MotionReadings.AddRange(motionReadings);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddReadingsResponse
        {
            AcceptedGpsCount = requestGpsReadings.Count,
            AcceptedMotionCount = requestMotionReadings.Count
        };
    }

    public async Task<FinishDriveResponse> FinishDriveAsync(Guid driveId, FinishDriveRequest request, CancellationToken cancellationToken)
    {
        var drive = await dbContext.Drives.FirstOrDefaultAsync(drive => drive.Id == driveId, cancellationToken);
        if (drive is null)
        {
            throw new DriveServiceException(DriveServiceError.NotFound, "Drive was not found.");
        }

        if (drive.Status != DriveStatus.Recording)
        {
            throw new DriveServiceException(DriveServiceError.Conflict, "Only a recording drive can be finished.");
        }

        var endedAt = request.EndedAt ?? DateTimeOffset.UtcNow;
        if (endedAt < drive.StartedAt)
        {
            throw new DriveServiceException(DriveServiceError.Validation, "Finish time cannot be before start time.");
        }

        drive.EndedAt = endedAt;
        drive.Status = DriveStatus.Completed;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new FinishDriveResponse
        {
            DriveId = drive.Id,
            Status = ToApiStatus(drive.Status),
            AnalysisStatus = "not_started"
        };
    }

    public async Task<List<DriveListItemResponse>> GetDrivesAsync(CancellationToken cancellationToken)
    {
        var drives = await dbContext.Drives
            .AsNoTracking()
            .Include(drive => drive.Analysis)
            .OrderByDescending(drive => drive.StartedAt)
            .ToListAsync(cancellationToken);

        return drives.Select(drive => new DriveListItemResponse
        {
            DriveId = drive.Id,
            StartedAt = drive.StartedAt,
            EndedAt = drive.EndedAt,
            Status = ToApiStatus(drive.Status),
            DurationSeconds = drive.Analysis?.DurationSeconds,
            DistanceMeters = drive.Analysis?.DistanceMeters,
            AverageSpeedKmh = drive.Analysis?.AverageSpeedKmh,
            MaxSpeedKmh = drive.Analysis?.MaxSpeedKmh,
            Score = drive.Analysis?.Score
        }).ToList();
    }

    public async Task<DriveDetailResponse> GetDriveAsync(Guid driveId, CancellationToken cancellationToken)
    {
        var drive = await dbContext.Drives
            .AsNoTracking()
            .Include(drive => drive.GpsReadings)
            .Include(drive => drive.DetectedEvents)
            .Include(drive => drive.Analysis)
            .FirstOrDefaultAsync(drive => drive.Id == driveId, cancellationToken);

        if (drive is null)
        {
            throw new DriveServiceException(DriveServiceError.NotFound, "Drive was not found.");
        }

        return new DriveDetailResponse
        {
            DriveId = drive.Id,
            StartedAt = drive.StartedAt,
            EndedAt = drive.EndedAt,
            Status = ToApiStatus(drive.Status),
            Route = drive.GpsReadings
                .OrderBy(reading => reading.Timestamp)
                .Select(reading => new RoutePointResponse
                {
                    Timestamp = reading.Timestamp,
                    Latitude = reading.Latitude,
                    Longitude = reading.Longitude,
                    SpeedKmh = reading.SpeedMps * 3.6
                })
                .ToList(),
            Analysis = drive.Analysis is null ? null : new DriveAnalysisResponse
            {
                DurationSeconds = drive.Analysis.DurationSeconds,
                DistanceMeters = drive.Analysis.DistanceMeters,
                AverageSpeedKmh = drive.Analysis.AverageSpeedKmh,
                MaxSpeedKmh = drive.Analysis.MaxSpeedKmh,
                HarshAccelerationCount = drive.Analysis.HarshAccelerationCount,
                HarshBrakingCount = drive.Analysis.HarshBrakingCount,
                SharpTurnCount = drive.Analysis.SharpTurnCount,
                Score = drive.Analysis.Score,
                Summary = drive.Analysis.Summary
            },
            Events = drive.DetectedEvents
                .OrderBy(detectedEvent => detectedEvent.Timestamp)
                .Select(detectedEvent => new DetectedEventResponse
                {
                    Type = ToApiEventType(detectedEvent.Type),
                    Timestamp = detectedEvent.Timestamp,
                    Latitude = detectedEvent.Latitude,
                    Longitude = detectedEvent.Longitude,
                    Severity = ToApiSeverity(detectedEvent.Severity),
                    Value = detectedEvent.Value
                })
                .ToList()
        };
    }

    private async Task<User> EnsureDemoUserAsync(DateTimeOffset now, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == DemoUserId, cancellationToken);
        if (user is not null)
        {
            return user;
        }

        user = new User
        {
            Id = DemoUserId,
            DisplayName = "Demo User",
            CreatedAt = now
        };

        dbContext.Users.Add(user);
        return user;
    }

    private static void ValidateGpsReadings(IEnumerable<GpsReadingRequest> readings)
    {
        foreach (var reading in readings)
        {
            if (reading.Latitude is < -90 or > 90)
            {
                throw new DriveServiceException(DriveServiceError.Validation, "GPS latitude must be between -90 and 90.");
            }

            if (reading.Longitude is < -180 or > 180)
            {
                throw new DriveServiceException(DriveServiceError.Validation, "GPS longitude must be between -180 and 180.");
            }
        }
    }

    private static void ValidateMotionReadings(IEnumerable<MotionReadingRequest> readings)
    {
        foreach (var reading in readings)
        {
            if (reading.Accelerometer is null)
            {
                throw new DriveServiceException(DriveServiceError.Validation, "Motion reading must contain accelerometer values.");
            }
        }
    }

    private static string ToApiStatus(DriveStatus status) => status switch
    {
        DriveStatus.Recording => "recording",
        DriveStatus.Completed => "completed",
        DriveStatus.Failed => "failed",
        _ => status.ToString()
    };

    private static string ToApiEventType(DetectedEventType type) => type switch
    {
        DetectedEventType.HarshAcceleration => "harsh_acceleration",
        DetectedEventType.HarshBraking => "harsh_braking",
        DetectedEventType.SharpTurn => "sharp_turn",
        _ => type.ToString()
    };

    private static string ToApiSeverity(EventSeverity severity) => severity switch
    {
        EventSeverity.Low => "low",
        EventSeverity.Medium => "medium",
        EventSeverity.High => "high",
        _ => severity.ToString()
    };
}

public class DriveServiceException : Exception
{
    public DriveServiceException(DriveServiceError error, string message)
        : base(message)
    {
        Error = error;
    }

    public DriveServiceError Error { get; }
}

public enum DriveServiceError
{
    NotFound,
    Conflict,
    Validation
}
