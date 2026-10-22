using SchoolApp.Models;

namespace SchoolApp.Repositories
{
    public interface IStudentRepository : IBaseRepository<Student>
    {
        Task<List<Course>> GetStudentCoursesAsync(int studentId);
        Task<Student?> GetByAm(string? am);
    }
}
