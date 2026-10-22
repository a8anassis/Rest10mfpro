using SchoolApp.Repositories;

namespace SchoolApp.Services
{
    public interface IApplicationService
    {
        IUserService UserService { get;  }
        ITeacherService TeacherService { get; }


        //IStudentService StudentService { get;  }
        // Other services can be added
    }
}
