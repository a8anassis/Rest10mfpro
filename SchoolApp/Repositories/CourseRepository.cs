using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;
using SchoolApp.Models;

namespace SchoolApp.Repositories;

public class CourseRepository : BaseRepository<Course>, ICourseRepository
{
    public CourseRepository(SchoolMvcDbfirstContext context) : base(context)
    {
    }

    public async Task<List<Student>> GetCourseStudentsAsync(int courseId)
    {
        return await _context.Courses
            .Where(c => c.Id == courseId)
            .SelectMany(c => c.Students)
            .ToListAsync();
    }

    public async Task<Teacher?> GetCourseTeacherAsync(int courseId)
    {
        var course = await _context.Courses
            .Include(c => c.Teacher)
            .SingleOrDefaultAsync(c => c.Id == courseId);

        return course?.Teacher;
    }
}
