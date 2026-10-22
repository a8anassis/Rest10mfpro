namespace SchoolApp.Models;

public class Student : BaseEntity
{
    public int Id { get; set; }
    public string Am { get; set; } = null!;
    public string Institution { get; set; } = null!;
    public string? Department { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<Course> Courses { get; set; } = new HashSet<Course>();
}
