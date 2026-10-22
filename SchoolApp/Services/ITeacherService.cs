using SchoolApp.DTO;

namespace SchoolApp.Services
{
    public interface ITeacherService
    {
        Task SignUpUserAsync(TeacherSignupDTO request);
    }
}
