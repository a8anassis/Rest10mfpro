using SchoolApp.Models;

namespace SchoolApp.Repositories
{
    public interface ICourseRepository : IBaseRepository<Course>
    {
        Task<List<Student>> GetCourseStudentsAsync(int courseId);
        Task<Teacher?> GetCourseTeacherAsync(int courseId);
    }
}
