using Microsoft.EntityFrameworkCore;
using DriveInsight.Api.Models;

namespace DriveInsight.Api.Data;

public class DriveInsightDbContext : DbContext
{
    public DriveInsightDbContext(DbContextOptions<DriveInsightDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Drive> Drives => Set<Drive>();

    public DbSet<GpsReading> GpsReadings => Set<GpsReading>();

    public DbSet<MotionReading> MotionReadings => Set<MotionReading>();

    public DbSet<DetectedEvent> DetectedEvents => Set<DetectedEvent>();

    public DbSet<DriveAnalysis> DriveAnalyses => Set<DriveAnalysis>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(user => user.Id);

            entity.Property(user => user.DisplayName)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(user => user.CreatedAt)
                .IsRequired();
        });

        modelBuilder.Entity<Drive>(entity =>
        {
            entity.ToTable("drives");

            entity.HasKey(drive => drive.Id);

            entity.Property(drive => drive.StartedAt)
                .IsRequired();

            entity.Property(drive => drive.EndedAt);

            entity.Property(drive => drive.Status)
                .HasConversion<string>()
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(drive => drive.CreatedAt)
                .IsRequired();

            entity.HasOne(drive => drive.User)
                .WithMany(user => user.Drives)
                .HasForeignKey(drive => drive.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<GpsReading>(entity =>
        {
            entity.ToTable("gps_readings");

            entity.HasKey(reading => reading.Id);

            entity.Property(reading => reading.Timestamp)
                .IsRequired();

            entity.Property(reading => reading.Latitude)
                .IsRequired();

            entity.Property(reading => reading.Longitude)
                .IsRequired();

            entity.Property(reading => reading.CreatedAt)
                .IsRequired();

            entity.HasIndex(reading => new { reading.DriveId, reading.Timestamp });

            entity.HasOne(reading => reading.Drive)
                .WithMany(drive => drive.GpsReadings)
                .HasForeignKey(reading => reading.DriveId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MotionReading>(entity =>
        {
            entity.ToTable("motion_readings");

            entity.HasKey(reading => reading.Id);

            entity.Property(reading => reading.Timestamp)
                .IsRequired();

            entity.Property(reading => reading.AccelerometerX)
                .IsRequired();

            entity.Property(reading => reading.AccelerometerY)
                .IsRequired();

            entity.Property(reading => reading.AccelerometerZ)
                .IsRequired();

            entity.Property(reading => reading.CreatedAt)
                .IsRequired();

            entity.HasIndex(reading => new { reading.DriveId, reading.Timestamp });

            entity.HasOne(reading => reading.Drive)
                .WithMany(drive => drive.MotionReadings)
                .HasForeignKey(reading => reading.DriveId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DetectedEvent>(entity =>
        {
            entity.ToTable("detected_events");

            entity.HasKey(detectedEvent => detectedEvent.Id);

            entity.Property(detectedEvent => detectedEvent.Type)
                .HasConversion<string>()
                .HasMaxLength(60)
                .IsRequired();

            entity.Property(detectedEvent => detectedEvent.Timestamp)
                .IsRequired();

            entity.Property(detectedEvent => detectedEvent.Latitude)
                .IsRequired();

            entity.Property(detectedEvent => detectedEvent.Longitude)
                .IsRequired();

            entity.Property(detectedEvent => detectedEvent.Severity)
                .HasConversion<string>()
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(detectedEvent => detectedEvent.Value)
                .IsRequired();

            entity.Property(detectedEvent => detectedEvent.CreatedAt)
                .IsRequired();

            entity.HasIndex(detectedEvent => new { detectedEvent.DriveId, detectedEvent.Timestamp });

            entity.HasOne(detectedEvent => detectedEvent.Drive)
                .WithMany(drive => drive.DetectedEvents)
                .HasForeignKey(detectedEvent => detectedEvent.DriveId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DriveAnalysis>(entity =>
        {
            entity.ToTable("drive_analyses");

            entity.HasKey(analysis => analysis.Id);

            entity.Property(analysis => analysis.DurationSeconds)
                .IsRequired();

            entity.Property(analysis => analysis.DistanceMeters)
                .IsRequired();

            entity.Property(analysis => analysis.AverageSpeedKmh)
                .IsRequired();

            entity.Property(analysis => analysis.MaxSpeedKmh)
                .IsRequired();

            entity.Property(analysis => analysis.HarshAccelerationCount)
                .IsRequired();

            entity.Property(analysis => analysis.HarshBrakingCount)
                .IsRequired();

            entity.Property(analysis => analysis.SharpTurnCount)
                .IsRequired();

            entity.Property(analysis => analysis.Score)
                .IsRequired();

            entity.Property(analysis => analysis.Summary)
                .HasMaxLength(1000);

            entity.Property(analysis => analysis.CreatedAt)
                .IsRequired();

            entity.HasIndex(analysis => analysis.DriveId)
                .IsUnique();

            entity.HasOne(analysis => analysis.Drive)
                .WithOne(drive => drive.Analysis)
                .HasForeignKey<DriveAnalysis>(analysis => analysis.DriveId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
