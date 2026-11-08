using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuthUsersService.Application.Auth.Models;
using AuthUsersService.Application.Auth.RefreshToken;
using AuthUsersService.Application.Auth.Tokens;
using AuthUsersService.Application.Users;
using AuthUsersService.Domain;
using Moq;
using Xunit;
using RT = AuthUsersService.Domain.Entities.RefreshToken;

namespace AuthUsersService.Tests.Auth;

public sealed class RefreshTokenServiceTests
{
    private const long UserId = 42;
    private const string StoredRefreshToken = "stored-refresh-token";
    private const string NewRefreshToken = "new-refresh-token";
    private const string NewAccessToken = "new-access-token";

    [Fact]
    public async Task RefreshAsync_ShouldRotateTokens_WhenTokenIsActive()
    {
        // Arrange
        var refreshTokenRepository = CreateRepositoryMock(CreateToken(isRevoked: false, isExpired: false));
        var service = CreateService(refreshTokenRepository);

        // Act
        var result = await service.RefreshAsync(StoredRefreshToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(NewAccessToken, result.Token);
        Assert.Equal(NewRefreshToken, result.RefreshToken);
        refreshTokenRepository.Verify(
            repository => repository.RotateAsync(
                1,
                RefreshTokenRevokeReason.Refreshed,
                It.Is<RT>(token => token.UserId == UserId),
                It.IsAny<CancellationToken>()),
            Times.Once);
        refreshTokenRepository.Verify(
            repository => repository.RevokeAsync(It.IsAny<long>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_ShouldStoreHashedNewToken()
    {
        // Arrange
        var refreshTokenRepository = CreateRepositoryMock(CreateToken(isRevoked: false, isExpired: false));
        RT? savedToken = null;
        refreshTokenRepository
            .Setup(repository => repository.RotateAsync(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.IsAny<RT>(),
                It.IsAny<CancellationToken>()))
            .Callback<long, string, RT, CancellationToken>((_, _, token, _) => savedToken = token)
            .Returns(Task.CompletedTask);

        var service = CreateService(refreshTokenRepository);

        // Act
        await service.RefreshAsync(StoredRefreshToken);

        // Assert
        Assert.NotNull(savedToken);
        Assert.Equal(RefreshTokenHasher.ComputeHash(NewRefreshToken), savedToken!.TokenHash);
        Assert.True(savedToken.IsActive);
    }

    [Fact]
    public async Task RefreshAsync_ShouldRevokeAllSessions_WhenRotatedTokenIsReused()
    {
        // Arrange
        var reusedToken = CreateToken(isRevoked: true, isExpired: false);
        reusedToken.RevokedBy = RefreshTokenRevokeReason.Refreshed;

        var refreshTokenRepository = CreateRepositoryMock(reusedToken);
        var service = CreateService(refreshTokenRepository);

        // Act
        var result = await service.RefreshAsync(StoredRefreshToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("reuse", result.Error, StringComparison.OrdinalIgnoreCase);
        refreshTokenRepository.Verify(
            repository => repository.RevokeAllForUserAsync(
                UserId,
                RefreshTokenRevokeReason.ReuseDetected,
                It.IsAny<CancellationToken>()),
            Times.Once);
        refreshTokenRepository.Verify(
            repository => repository.RotateAsync(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.IsAny<RT>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_ShouldNotRevokeAllSessions_WhenTokenWasRevokedOnLogout()
    {
        // Arrange
        var revokedToken = CreateToken(isRevoked: true, isExpired: false);
        revokedToken.RevokedBy = RefreshTokenRevokeReason.Logout;

        var refreshTokenRepository = CreateRepositoryMock(revokedToken);
        var service = CreateService(refreshTokenRepository);

        // Act
        var result = await service.RefreshAsync(StoredRefreshToken);

        // Assert
        Assert.False(result.IsSuccess);
        refreshTokenRepository.Verify(
            repository => repository.RevokeAllForUserAsync(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_ShouldFail_WhenTokenIsRevoked()
    {
        // Arrange
        var refreshTokenRepository = CreateRepositoryMock(CreateToken(isRevoked: true, isExpired: false));
        var service = CreateService(refreshTokenRepository);

        // Act
        var result = await service.RefreshAsync(StoredRefreshToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Token);
        Assert.Null(result.RefreshToken);
        refreshTokenRepository.Verify(
            repository => repository.AddAsync(It.IsAny<RT>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_ShouldFail_WhenTokenIsExpired()
    {
        // Arrange
        var refreshTokenRepository = CreateRepositoryMock(CreateToken(isRevoked: false, isExpired: true));
        var service = CreateService(refreshTokenRepository);

        // Act
        var result = await service.RefreshAsync(StoredRefreshToken);

        // Assert
        Assert.False(result.IsSuccess);
        refreshTokenRepository.Verify(
            repository => repository.FindByTokenHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RefreshAsync_ShouldFail_WhenTokenIsEmpty()
    {
        // Arrange
        var refreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var service = CreateService(refreshTokenRepository);

        // Act
        var result = await service.RefreshAsync(" ");

        // Assert
        Assert.False(result.IsSuccess);
        refreshTokenRepository.Verify(
            repository => repository.FindByTokenHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetSessionUserIdAsync_ShouldReturnUserId_WhenTokenIsActive()
    {
        // Arrange
        var refreshTokenRepository = CreateRepositoryMock(CreateToken(isRevoked: false, isExpired: false));
        var service = CreateService(refreshTokenRepository);

        // Act
        var userId = await service.GetSessionUserIdAsync(StoredRefreshToken);

        // Assert
        Assert.Equal(UserId, userId);
    }

    [Fact]
    public async Task GetSessionUserIdAsync_ShouldReturnNull_WhenTokenIsRevoked()
    {
        // Arrange
        var refreshTokenRepository = CreateRepositoryMock(CreateToken(isRevoked: true, isExpired: false));
        var service = CreateService(refreshTokenRepository);

        // Act
        var userId = await service.GetSessionUserIdAsync(StoredRefreshToken);

        // Assert
        Assert.Null(userId);
    }

    [Fact]
    public async Task RevokeAsync_ShouldRevokeToken_WhenTokenIsActive()
    {
        // Arrange
        var refreshTokenRepository = CreateRepositoryMock(CreateToken(isRevoked: false, isExpired: false));
        var service = CreateService(refreshTokenRepository);

        // Act
        var revoked = await service.RevokeAsync(StoredRefreshToken);

        // Assert
        Assert.True(revoked);
        refreshTokenRepository.Verify(
            repository => repository.RevokeAsync(1, "logout", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static Mock<IRefreshTokenRepository> CreateRepositoryMock(RT storedToken)
    {
        var refreshTokenRepository = new Mock<IRefreshTokenRepository>();
        refreshTokenRepository
            .Setup(repository => repository.FindByTokenHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(storedToken);

        return refreshTokenRepository;
    }

    private static RefreshTokenService CreateService(Mock<IRefreshTokenRepository> refreshTokenRepository)
    {
        var jwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        jwtTokenGenerator
            .Setup(generator => generator.GenerateTokenFromUserId(UserId, It.IsAny<IReadOnlyCollection<RoleName>>()))
            .Returns(NewAccessToken);
        jwtTokenGenerator
            .Setup(generator => generator.GenerateRefreshToken())
            .Returns(NewRefreshToken);

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(repository => repository.GetUserRoleNamesAsync(UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RoleName> { RoleName.Admin });

        return new RefreshTokenService(
            jwtTokenGenerator.Object,
            refreshTokenRepository.Object,
            userRepository.Object);
    }

    private static RT CreateToken(bool isRevoked, bool isExpired)
    {
        return new RT
        {
            Id = 1,
            UserId = UserId,
            TokenHash = RefreshTokenHasher.ComputeHash(StoredRefreshToken),
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            ExpiresAt = isExpired ? DateTime.UtcNow.AddMinutes(-1) : DateTime.UtcNow.AddDays(1),
            RevokedAt = isRevoked ? DateTime.UtcNow : null
        };
    }
}
