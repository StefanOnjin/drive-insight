namespace DriveInsight.Api.Models;

public class User
{
    public Guid Id { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public List<Drive> Drives { get; set; } = new();
}
