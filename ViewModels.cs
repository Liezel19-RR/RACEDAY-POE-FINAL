using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models;

public class LoginVm
{
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required] public string Password { get; set; } = "";
}

public class RegisterVm
{
    [Required, MaxLength(100)] public string FullName { get; set; } = "";
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required, MinLength(8)] public string Password { get; set; } = "";
    [Required] public string Role { get; set; } = "Participant";
    public string? Phone { get; set; }
}

public class EventFilterVm
{
    public string? Search { get; set; }
    public string? Type { get; set; }
    public string? Location { get; set; }
    [DataType(DataType.Date)] public DateTime? From { get; set; }
    [DataType(DataType.Date)] public DateTime? To { get; set; }
    public bool UpcomingOnly { get; set; } = true;
}

public class EventVm
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Date { get; set; }
    public string Location { get; set; } = "";
    public decimal DistanceKm { get; set; }
    public string EventType { get; set; } = "Run";
    public string? BannerImageUrl { get; set; }
    public List<CategoryVm>? Categories { get; set; }
}

public class CategoryVm
{
    public int Id { get; set; }
    public int EventId { get; set; }
    [Required, MaxLength(80)] public string Name { get; set; } = "";
    public string? Description { get; set; }
    [Range(.1, 1000)] public decimal DistanceKm { get; set; } = 5;
    [Range(0, 100000)] public decimal Price { get; set; }
    [Range(1, int.MaxValue)] public int? Capacity { get; set; }
    public int EnrolledCount { get; set; }
}

public class CreateEventVm
{
    [Required, MaxLength(150)] public string Name { get; set; } = "";
    [Required] public string Description { get; set; } = "";
    [DataType(DataType.Date)] public DateTime Date { get; set; } = DateTime.Today.AddDays(30);
    [Required] public string Location { get; set; } = "";
    [Range(.1, 1000)] public decimal DistanceKm { get; set; } = 5;
    [Required] public string EventType { get; set; } = "Run";
}

public class ProfileVm
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string FullName { get; set; } = "";
    [Required, EmailAddress] public string Email { get; set; } = "";
    public string Role { get; set; } = "";
    public string? Phone { get; set; }
    public string? ProfilePictureUrl { get; set; }
}

public class EnrolmentVm
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string Event { get; set; } = "";
    public DateTime EventDate { get; set; }
    public int ParticipantId { get; set; }
    public string Participant { get; set; } = "";
    public string Email { get; set; } = "";
    public int CategoryId { get; set; }
    public string Category { get; set; } = "";
    public string Status { get; set; } = "";
    public ResultSummaryVm? Result { get; set; }
    public string? ResultStatus => Result?.Status;
}

public class ResultSummaryVm
{
    public string Status { get; set; } = "";
    public TimeSpan FinishTime { get; set; }
    public int FinishingPosition { get; set; }
}

public class ResultInputVm
{
    [Range(1, int.MaxValue)] public int EnrolmentId { get; set; }
    [Required] public TimeSpan FinishTime { get; set; }
    [Range(1, int.MaxValue)] public int FinishingPosition { get; set; } = 1;
    [Required] public string Status { get; set; } = "Finished";
}

public class ResultVm
{
    public int Id { get; set; }
    public string Event { get; set; } = "";
    public DateTime Date { get; set; }
    public string Category { get; set; } = "";
    public TimeSpan FinishTime { get; set; }
    public int FinishingPosition { get; set; }
    public int FinisherCount { get; set; }
    public string PositionDisplay { get; set; } = "";
    public string Status { get; set; } = "";
}

public class LeaderboardVm
{
    public string Participant { get; set; } = "";
    public string Category { get; set; } = "";
    public TimeSpan FinishTime { get; set; }
    public int FinishingPosition { get; set; }
    public string Status { get; set; } = "";
}
