using AutoMapper;
using Microsoft.Identity.Client;
using SchoolApp.Core;
using SchoolApp.Core.Filters;
using SchoolApp.DTO;
using SchoolApp.Exceptions;
using SchoolApp.Models;
using SchoolApp.Repositories;
using SchoolApp.Security;
using System.Linq.Expressions;

namespace SchoolApp.Services
{
    public class UserService : IUserService
    {
        private readonly IEncryptionUtil _encryptionUtil;
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IEncryptionUtil encryptionUtil, IUnitOfWork unitOfWork, 
            IMapper mapper, ILogger<UserService> logger)
        {
            _encryptionUtil = encryptionUtil;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber, int pageSize, 
            UserFiltersDTO userFiltersDTO)
        {
            List<User> users = [];
            List<Expression<Func<User, bool>>> predicates = [];

            if (!string.IsNullOrEmpty(userFiltersDTO.Username))
            {
                predicates.Add(u => u.Username == userFiltersDTO.Username);
            }

            if (!string.IsNullOrEmpty(userFiltersDTO.Email))
            {
                predicates.Add(u => u.Email == userFiltersDTO.Email);
            }
            
            if (!string.IsNullOrEmpty(userFiltersDTO.UserRole))
            {
                predicates.Add(u => u.Role.Name == userFiltersDTO.UserRole);
            }

            var paginatedResult = await _unitOfWork.UserRepository.GetUsersAsync(pageNumber, pageSize, predicates);

            var dtoResult = new PaginatedResult<UserReadOnlyDTO>()
            {
                Data = _mapper.Map<List<UserReadOnlyDTO>>(paginatedResult.Data),
                TotalRecords = paginatedResult.TotalRecords,
                PageNumber = paginatedResult.PageNumber,
                PageSize = paginatedResult.PageSize
            };

            _logger.LogInformation("Retrieved {Count} users", dtoResult.Data.Count);
            return dtoResult;
        }

        public async Task<UserReadOnlyDTO?> GetUserByUsernameAsync(string username)
        {
            try
            {
                User? user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

                if (user is null)
                {
                    throw new EntityNotFoundException("User with " + username + " not found.");
                }

                _logger.LogInformation("User found with username={Username}", username);
                return _mapper.Map<UserReadOnlyDTO>(user);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogError("Error retreiving user with username={Username}.", username);
                throw;
            }
        }

        public async Task<User?> VerifyAndGetUserAsync(UserLoginDTO credentials)
        {
            try
            {
                User? user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(credentials.Username!);

                if (user is null || !_encryptionUtil.IsValidPassword(credentials.Password!, user.Password))
                {
                    throw new EntityNotAuthorizedException("Bad Credentials");
                }
                _logger.LogInformation("User with username={Username} verified for login.", credentials.Username);
                return user;
            }
            catch (EntityNotAuthorizedException)
            {
                _logger.LogError("Authentication failed for username={Username}", credentials.Username);
                return null;
            }
        }
    }
}
