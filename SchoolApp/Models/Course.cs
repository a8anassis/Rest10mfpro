namespace SchoolApp.Models;

public class Course : BaseEntity
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public int? TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
    public ICollection<Student> Students { get; set; } = new HashSet<Student>();
}
