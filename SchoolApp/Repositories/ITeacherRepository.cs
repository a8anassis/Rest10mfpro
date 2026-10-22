using SchoolApp.Core;
using SchoolApp.Models;
using System.Linq.Expressions;

namespace SchoolApp.Repositories;

public interface ITeacherRepository : IBaseRepository<Teacher>
{
    Task<Teacher?> GetByPhoneNumberAsync(string phoneNumber);
    Task<List<Course>> GetTeacherCoursesAsync(int teacherId);
    Task<PaginatedResult<User>> GetPaginatedUsersTeachersAsync(int pageNumber, int pageSize, 
        List<Expression<Func<User, bool>>> predicates);
}
