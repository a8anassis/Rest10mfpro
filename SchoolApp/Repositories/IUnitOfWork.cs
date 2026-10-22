namespace SchoolApp.Repositories;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IStudentRepository StudentRepository { get; }

    ITeacherRepository TeacherRepository { get; }

    ICourseRepository CourseRepository { get; }

    Task<bool> SaveAsync();
}
