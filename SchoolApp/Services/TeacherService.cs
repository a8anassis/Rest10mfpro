using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SchoolApp.DTO;
using SchoolApp.Exceptions;
using SchoolApp.Models;
using SchoolApp.Repositories;
using SchoolApp.Security;

namespace SchoolApp.Services;

public class TeacherService : ITeacherService
{

    private readonly IEncryptionUtil _encryptionUtil;
    private readonly IUnitOfWork _unitOfWork;
    private IMapper _mapper;
    private readonly ILogger<TeacherService> _logger;

    public TeacherService(IEncryptionUtil encryptionUtil, IUnitOfWork unitOfWork, IMapper mapper, ILogger<TeacherService> logger)
    {
        _encryptionUtil = encryptionUtil;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task SignUpUserAsync(TeacherSignupDTO teacherSignupDTO)
    {
        Teacher teacher = _mapper.Map<Teacher>(teacherSignupDTO);
        User user = _mapper.Map<User>(teacherSignupDTO);

        try
        {
            User? existingUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(user.Username);

            if (existingUser is not null)
            {
                throw new EntityAlreadyExistsException("User " + existingUser.Username + " already exists");
            }

            user.Teacher = teacher;
            user.Password = _encryptionUtil.Encrypt(user.Password);

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.TeacherRepository.AddAsync(teacher);

            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Teacher with id={Id} signed up successfully", teacher.Id);
        }
        catch (EntityAlreadyExistsException)
        {
            _logger.LogWarning("Error signing up for teacher with username={Username}", teacher.User.Username);
            throw;
        }
    }
}
