using Microsoft.EntityFrameworkCore;
using SchoolApp.Core;
using SchoolApp.Data;
using SchoolApp.Models;
using System.Linq.Expressions;

namespace SchoolApp.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(SchoolMvcDbfirstContext context) : base(context)
    {
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(u => u.Role)           // Eager Loading - Join
                .ThenInclude(r => r.Capabilities)
            .FirstOrDefaultAsync(u => u.Username == username || u.Email == username);
    }

    public async Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize, 
        List<Expression<Func<User, bool>>> predicates)
    {
        IQueryable<User> query = _context.Users;    // δεν εκτελείται

        if (predicates is not null && predicates.Count > 0)
        {
            foreach (var predicate in predicates)
            {
                query = query.Where(predicate);     // Υπονοείται το AND
            }
        }

        int totalRecords = await query.CountAsync();
        int skip = (pageNumber - 1) * pageSize;

        var data = await query
            .OrderBy(u => u.Id)     // paginated πάντα με OrderBy πριν το skip
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<User>(data, totalRecords, pageNumber, pageSize);
    }
}
