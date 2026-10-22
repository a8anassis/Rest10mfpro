using SchoolApp.Data;

namespace SchoolApp.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SchoolMvcDbfirstContext _context;

    public IUserRepository UserRepository { get; }
    public ITeacherRepository TeacherRepository { get; }
    public IStudentRepository StudentRepository { get; }
    public ICourseRepository CourseRepository { get; }


    public UnitOfWork(SchoolMvcDbfirstContext context)
    {
        _context = context;
        UserRepository = new UserRepository(context);
        StudentRepository = new StudentRepository(context);
        TeacherRepository = new TeacherRepository(context);
        CourseRepository = new CourseRepository(context);
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;       // κάνει commit και αυτόματα rollback αν αποτύχει
    }

}
