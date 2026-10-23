using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.Core;
using SchoolApp.Core.Filters;
using SchoolApp.DTO;
using SchoolApp.Exceptions;
using SchoolApp.Services;
using System.Security.Claims;

namespace SchoolApp.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UsersController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public UsersController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Gets a user by their ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The user details.</returns>
        /// <response code="200">Returns the requested user.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="403">If the user lacks permission to view the user.</response>
        /// <response code="404">If no user exists with the given ID.</response>
        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(UserReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserReadOnlyDTO>> GetUserById(int id)
        {
            EnsureCanViewUser(id);
            var user = await _applicationService.UserService.GetUserByIdAsync(id);
            return Ok(user);
        }

        /// <summary>
        /// Gets a user by their username.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <returns>The user details.</returns>
        /// <response code="200">Returns the requested user.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="404">If no user exists with the given username.</response>
        [HttpGet("by-username/{username}")]
        [Authorize]
        [ProducesResponseType(typeof(UserReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserReadOnlyDTO>> GetUserByUsername(string username)
        {
            EnsureCanViewUser(username);
            var user = await _applicationService.UserService.GetUserByUsernameAsync(username);
            return Ok(user);
        }

        /// <summary>
        /// Gets a paginated list of users with optional filtering.
        /// </summary>
        /// <param name="pageNumber">The page number (1-based). Default is 1.</param>
        /// <param name="pageSize">The number of items per page. Default is 10.</param>
        /// <param name="filters">Optional filters for username, email, and role.</param>
        /// <returns>A paginated list of users matching the filters.</returns>
        /// <response code="200">Returns the paginated user list.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="403">If the user lacks permission to list users.</response>
        [HttpGet]
        [Authorize(Policy = "VIEW_USERS")]
        [ProducesResponseType(typeof(PaginatedResult<UserReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedResult<UserReadOnlyDTO>>> GetUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] UserFiltersDTO? filters = null)
        {
            var result = await _applicationService.UserService
                .GetPaginatedUsersFilteredAsync(pageNumber, pageSize, filters ?? new UserFiltersDTO());

            return Ok(result);
        }

        private void EnsureCanViewUser(int targetUserId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var isOwnProfile = currentUserId == targetUserId;

            EnsureCanViewUserCore(isOwnProfile);
        }

        private void EnsureCanViewUser(string username)
        {
            var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;
            var isOwnProfile = string.Equals(currentUsername, username, StringComparison.OrdinalIgnoreCase);

            EnsureCanViewUserCore(isOwnProfile);
        }

        private void EnsureCanViewUserCore(bool isOwnProfile)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Self-view: Teachers και Students μπορούν να δουν τον εαυτό τους
            if (isOwnProfile && (currentUserRole == "TEACHER" || currentUserRole == "STUDENT"))
            {
                return;
            }

            // Other-view: χρειάζεται VIEW_TEACHER ή VIEW_STUDENT capability
            //var canViewOthers = User.HasClaim("capability", "VIEW_USER+TEACHER");

            //if (canViewOthers)
            //{
            //    return;
            //}

            throw new EntityForbiddenException("User",
                "You do not have permission to view this user.");
        }
    }
}