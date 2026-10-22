namespace SchoolApp.Services
{
    public class ApplicationService : IApplicationService
    {

        public IUserService UserService { get; }
        public ITeacherService TeacherService { get; }

        // Other services can be added
        
        public ApplicationService(IUserService userService, ITeacherService teacherService)
        {
            UserService = userService;
            TeacherService = teacherService;
        }
    }
}
