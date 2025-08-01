using Microsoft.EntityFrameworkCore;
using Moq;
using MAS_Shared.Data;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using TrackingServer.Services;

namespace SidekickApp_Test.Sidekick_Mobile_Tests
{
    //public class JwtServiceTests
    //{
    //    private readonly Mock<AppDbContext> _mockDbContext;
    //    private readonly Mock<IConfiguration> _mockConfig;
    //    private readonly JwtService _jwtService;

    //    public JwtServiceTests()
    //    {
    //        _mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
    //        _mockConfig = new Mock<IConfiguration>();
    //        _jwtService = new JwtService(_mockDbContext.Object, _mockConfig.Object);
    //    }

    //    [Fact]
    //    public async Task GenerateJWT_UserNotFound_ReturnsNull()
    //    {
    //        // Arrange
    //        var loginReq = new LoginRequestModel { SLT = "1234567890" };
    //        _mockDbContext.Setup(db => db.Users.FirstOrDefaultAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<CancellationToken>()))
    //                      .ReturnsAsync((ApplicationUser?)null);

    //        // Act
    //        var result = await _jwtService.GenerateJWTAsync(loginReq.SLT);

    //        // Assert
    //        Assert.False(result.Item1 == null);
    //        Assert.False(string.IsNullOrEmpty(result.Item1.UserCellNumber));
    //        Assert.Equal("User Account or User.PhoneNumber was null", result.Item1.UserCellNumber);
    //        Assert.Null(result.Item2);
    //    }

    //    [Fact]
    //    public async Task GenerateJWT_InvalidKey_ThrowsException()
    //    {
    //        // Arrange
    //        var loginReq = new LoginRequestModel { SLT = "1234567890" };
    //        var user = new ApplicationUser { PhoneNumber = "7111111111" };
    //        _mockDbContext.Setup(db => db.Users.FirstOrDefaultAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<CancellationToken>()))
    //                      .ReturnsAsync(user);
    //        _mockConfig.Setup(config => config["Jwt:Key"]).Returns(string.Empty);

    //        // Act & Assert
    //        await Assert.ThrowsAsync<InvalidOperationException>(() => _jwtService.GenerateJWTAsync(loginReq.SLT));
    //    }

    //    [Fact]
    //    public async Task GenerateJWT_ValidUser_ReturnsToken()
    //    {
    //        // Arrange
    //        var loginReq = new LoginRequestModel { SLT = "1234567890" };
    //        var user = new ApplicationUser { PhoneNumber = "7111111111" };
    //        var role = new IdentityRole { Id = "1", Name = "User" };
    //        _mockDbContext.Setup(db => db.Users.FirstOrDefaultAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<CancellationToken>()))
    //                      .ReturnsAsync(user);
    //        _mockDbContext.Setup(db => db.Roles.FirstOrDefault(It.IsAny<Expression<Func<IdentityRole, bool>>>()))
    //                      .Returns(role);
    //        _mockConfig.Setup(config => config["Authentication:Schemes:Bearer:ValidIssuer"]).Returns("issuer");
    //        _mockConfig.Setup(config => config["Authentication:Schemes:Bearer:ValidAudiences"]).Returns("audience");
    //        _mockConfig.Setup(config => config["Jwt:Key"]).Returns("supersecretkey");

    //        // Act
    //        var result = await _jwtService.GenerateJWTAsync(loginReq.SLT);

    //        // Assert
    //        Assert.False(result.Item1 == null);
    //        Assert.NotNull(result.Item2);
    //        Assert.Equal("7111111111", result.Item1.UserCellNumber);
    //        Assert.Equal("User", result.Item1.Role);
    //        Assert.NotNull(result.Item1.AccessToken);
    //        Assert.NotEmpty(result.Item1.AccessToken);
    //    }
    //}
}
