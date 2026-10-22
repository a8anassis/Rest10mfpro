namespace SchoolApp.Models;

public class Teacher : BaseEntity
{
    public int Id { get; set; }
    public string Institution { get; set; } = null!;
    public string? PhoneNumber { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<Course> Courses { get; set; } = new HashSet<Course>();

}
