using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;
using SchoolApp.Models;

namespace SchoolApp.Repositories;

public class StudentRepository : BaseRepository<Student>, IStudentRepository
{
    public StudentRepository(SchoolMvcDbfirstContext context) : base(context)
    {
    }

    public async Task<Student?> GetByAm(string? am)
    {
        return await _context.Students
            .Where(s => s.Am == am)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Course>> GetStudentCoursesAsync(int studentId)
    {
        List<Course> courses;

        courses = await _context.Students
            .Where(s => s.Id == studentId)
            .SelectMany(s => s.Courses)
            .ToListAsync();

        return courses;
    }
}
