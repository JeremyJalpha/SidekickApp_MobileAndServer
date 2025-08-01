using FluentAssertions;
using MAS_Shared.Data;
using MAS_Shared.Models;
using MAS_Shared.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TrackingServer.Services;

namespace Server_Test.TrackingServer_Tests
{
    public class JwtEndToEndTests
    {
        private readonly JwtIssueConfig _jwtConfig;
        private readonly AppDbContext _dbContext;
        private readonly JwtService _jwtService;

        public JwtEndToEndTests()
        {
            _jwtConfig = new JwtIssueConfig
            {
                Key = "DXgj43VikKvImniIds0p2d4q/O3Nruk5O/7MY/LN5Q4=",
                Issuer = "https://localhost:443",
                Audiences = new List<string> { "https://localhost:443" },
                TokenLifetimeMinutes = 60
            };

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("JwtTestDb")
                .Options;

            _dbContext = new AppDbContext(options);
            SeedTestData(_dbContext);

            _jwtService = new JwtService(_dbContext, Options.Create(_jwtConfig));
        }

        private void SeedTestData(AppDbContext db)
        {
            db.Users.Add(new ApplicationUser
            {
                CellNumber = "27646436186",
                Id = "user2",
                UserName = "TestDriver",
            });

            db.Roles.Add(new Microsoft.AspNetCore.Identity.IdentityRole
            {
                Id = "role3",
                Name = "Driver"
            });

            db.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
            {
                UserId = "user2",
                RoleId = "role3"
            });

            db.SaveChanges();
        }

        // TODO: Fix up Diogo's Test.
        //[Fact]
        //public async Task OperatorToken_ShouldContainOperatorRoleClaim_AndBeValid()
        //{
        //    string userId = "operator-789";
        //    string phone = "2788001122";
        //    string role = "Operator";

        //    db.SaveChanges();
        //    var jwt = JwtHelperUtil.GenerateJwt(
        //        userId,
        //        phone,
        //        role,
        //        _jwtConfig.Issuer,
        //        _jwtConfig.Key,
        //        _jwtConfig.Audiences,
        //        _jwtConfig.ExpiryMinutes
        //    );

        //    // Should contain correct claims
        //    JwtHelperUtil.ExtractClaim(jwt, "phone_number").Should().Be(phone);
        //    JwtHelperUtil.ExtractClaim(jwt, "sub").Should().Be(userId);
        //    JwtHelperUtil.ExtractClaim(jwt, "role").Should().Be(role);

        //    // Should validate on client side
        //    JwtHelperUtil.ClientSideJWTPrevalidation(jwt, _jwtConfig.Audiences, _jwtConfig.Issuer)
        //        .Should().BeTrue();
        //    Console.WriteLine($"Token:{jwt}");
        //}

        [Fact]
        public Task ShortLivedToken_ShouldBeGeneratedAndValidClientSide()
        {
            string phoneNumber = "27646436186";
            string userId = "user2";
            string role = "Driver";

            var jwt = JwtHelperUtil.GenerateJwt(
                userId,
                phoneNumber,
                role,
                _jwtConfig.Issuer,
                _jwtConfig.Key,
                _jwtConfig.Audiences,
                _jwtConfig.TokenLifetimeMinutes
            );

            bool isValid = JwtHelperUtil.ClientSideJWTPrevalidation(
                jwt,
                _jwtConfig.Audiences,
                _jwtConfig.Issuer
            );

            isValid.Should().BeTrue();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task LongLivedToken_ShouldBeReturned_FromAuthHub()
        {
            string phoneNumber = "27646436186";

            var slt = JwtHelperUtil.GenerateJwt(
                "user2", phoneNumber, "Driver",
                _jwtConfig.Issuer, 
                _jwtConfig.Key,
                _jwtConfig.Audiences, 
                5 // short expiry
            );

            string? extractedPhone = JwtHelperUtil.ExtractClaim(slt, "phone_number");
            extractedPhone.Should().NotBeNull();
            extractedPhone!.Should().Be(phoneNumber);
            var (longLivedJwt, error) = await _jwtService.GenerateJwtAsync(extractedPhone);

            longLivedJwt.Should().NotBeNullOrEmpty();
            error.Should().BeNull();

            JwtHelperUtil.ClientSideJWTPrevalidation(
                longLivedJwt,
                _jwtConfig.Audiences,
                _jwtConfig.Issuer
            )
            .Should().BeTrue();
        }
    }
}