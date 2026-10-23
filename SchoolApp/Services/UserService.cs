using AutoMapper;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using SchoolApp.Core;
using SchoolApp.Core.Filters;
using SchoolApp.DTO;
using SchoolApp.Exceptions;
using SchoolApp.Models;
using SchoolApp.Repositories;
using SchoolApp.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace SchoolApp.Services
{
    public class UserService : IUserService
    {
        private readonly IEncryptionUtil _encryptionUtil;
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _configuration;

        public UserService(IEncryptionUtil encryptionUtil, IUnitOfWork unitOfWork, 
            IMapper mapper, ILogger<UserService> logger, IConfiguration configuration)
        {
            _encryptionUtil = encryptionUtil;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
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
            
            User? user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

            if (user is null)
            {
                throw new EntityNotFoundException("User", "Enity not found.");
            }

            _logger.LogInformation("User found with username={Username}", username);
            return _mapper.Map<UserReadOnlyDTO>(user);
           
        }

        public async Task<User?> VerifyAndGetUserAsync(UserLoginDTO credentials)
        {
            User? user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(credentials.Username!);

            if (user is null || !_encryptionUtil.IsValidPassword(credentials.Password!, user.Password))
            {
                throw new EntityNotAuthorizedException("Auth", "Bad Credentials");
            }
            _logger.LogInformation("User with username={Username} verified for login.", credentials.Username);
            return user;
        }

        public string CreateUserToken(User user)
        {
            var secretKey = _configuration["Jwt:Secret"]!;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsInfo = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name)

            };

            claimsInfo.AddRange(user.Role.Capabilities
                    .Select(c => new Claim("Capability", c.Name)));

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claimsInfo,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: signingCredentials
            );

            // Serialize the token to a string
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        }
    }
}

