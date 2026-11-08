using System;
using System.Threading;
using System.Threading.Tasks;
using AuthUsersService.Application.Extensions;
using AuthUsersService.Application.Users;
using AuthUsersService.Application.Users.ChangePassword;
using AuthUsersService.Domain;
using AuthUsersService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace AuthUsersService.Tests.Users;

public sealed class ChangePasswordCommandTests
{
    private const long UserId = 7;
    private const string CurrentPassword = "Current#123";
    private const string NewPassword = "Updated#456";

    [Fact]
    public async Task Send_ShouldRevokeAllSessions_WhenPasswordChanged()
    {
        // Arrange
        var userRepository = CreateUserRepository();
        var refreshTokenRepository = new Mock<IRefreshTokenRepository>();

        using var provider = CreateProvider(userRepository, refreshTokenRepository);
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        var result = await mediator.Send(new ChangePasswordCommand(UserId, CurrentPassword, NewPassword));

        // Assert
        Assert.Equal(1, result);
        userRepository.Verify(
            repository => repository.UpdatePasswordAsync(UserId, It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
        refreshTokenRepository.Verify(
            repository => repository.RevokeAllForUserAsync(
                UserId,
                RefreshTokenRevokeReason.PasswordChanged,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Send_ShouldThrowUnauthorized_AndKeepSessions_WhenCurrentPasswordIsWrong()
    {
        // Arrange
        var userRepository = CreateUserRepository();
        var refreshTokenRepository = new Mock<IRefreshTokenRepository>();

        using var provider = CreateProvider(userRepository, refreshTokenRepository);
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => mediator.Send(new ChangePasswordCommand(UserId, "Wrong#123", NewPassword)));

        // Assert
        Assert.Contains("пароль", exception.Message);
        userRepository.Verify(
            repository => repository.UpdatePasswordAsync(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        refreshTokenRepository.Verify(
            repository => repository.RevokeAllForUserAsync(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IUserRepository> CreateUserRepository()
    {
        var user = User.Create(
            "nickname",
            "user@example.com",
            BCrypt.Net.BCrypt.HashPassword(CurrentPassword),
            DateTimeOffset.UtcNow);
        user.SetId(UserId);

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(repository => repository.GetByIdAsync(UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        return userRepository;
    }

    private static ServiceProvider CreateProvider(
        Mock<IUserRepository> userRepository,
        Mock<IRefreshTokenRepository> refreshTokenRepository)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddSingleton(userRepository.Object);
        services.AddSingleton(refreshTokenRepository.Object);

        return services.BuildServiceProvider();
    }
}
