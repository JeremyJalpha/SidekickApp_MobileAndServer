using MAS_Shared.Data;
using MAS_Shared.Models;
using MAS_Shared.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace TrackingServer.Services
{
    public class JwtService
    {
        private readonly AppDbContext _dbContext;
        private readonly JwtIssueConfig _jwtConfig;

        public JwtService(AppDbContext dbContext, IOptions<JwtIssueConfig> jwtConfigOptions)
        {
            _dbContext = dbContext;
            _jwtConfig = jwtConfigOptions.Value;
        }

        public async Task<(string? jwt, Exception? error)> GenerateJwtAsync(string userPhone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userPhone))
                    return (null, new Exception("Provided User Cellnumber was null or empty - 401"));

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.CellNumber == userPhone);
                if (user is null || string.IsNullOrWhiteSpace(user.CellNumber))
                    return (null, new Exception("User not found or has invalid phone number - 401"));

                var userRole = await (from ur in _dbContext.UserRoles
                                      join r in _dbContext.Roles on ur.RoleId equals r.Id
                                      where ur.UserId == user.Id
                                      select r.Name).FirstOrDefaultAsync();

                if (string.IsNullOrWhiteSpace(userRole))
                    return (null, new Exception("UserRole not found or has no Name - 401"));

                var jwt = JwtHelperUtil.GenerateJwt(
                    userId: user.Id,
                    phoneNumber: user.CellNumber,
                    role: userRole,
                    signingKey: _jwtConfig.Key,
                    issuer: _jwtConfig.Issuer,
                    audiences: _jwtConfig.Audiences,
                    tokenMinutes: _jwtConfig.TokenLifetimeMinutes
                );

                return (jwt, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (null, new Exception("Unexpected error occurred while generating a JWT - 500", ex));
            }
        }
    }
}
