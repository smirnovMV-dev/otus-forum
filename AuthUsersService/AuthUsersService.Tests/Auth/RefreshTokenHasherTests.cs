using AuthUsersService.Application.Auth.Tokens;
using Xunit;

namespace AuthUsersService.Tests.Auth;

public sealed class RefreshTokenHasherTests
{
    [Fact]
    public void ComputeHash_ShouldReturnSameValue_ForSameToken()
    {
        // Arrange
        const string token = "refresh-token";

        // Act
        var firstHash = RefreshTokenHasher.ComputeHash(token);
        var secondHash = RefreshTokenHasher.ComputeHash(token);

        // Assert
        Assert.Equal(firstHash, secondHash);
    }

    [Fact]
    public void ComputeHash_ShouldReturnDifferentValues_ForDifferentTokens()
    {
        // Arrange
        const string firstToken = "refresh-token-1";
        const string secondToken = "refresh-token-2";

        // Act
        var firstHash = RefreshTokenHasher.ComputeHash(firstToken);
        var secondHash = RefreshTokenHasher.ComputeHash(secondToken);

        // Assert
        Assert.NotEqual(firstHash, secondHash);
    }

    [Fact]
    public void ComputeHash_ShouldReturnBase64EncodedSha256()
    {
        // Arrange
        const string token = "refresh-token";

        // Act
        var hash = RefreshTokenHasher.ComputeHash(token);

        // Assert
        Assert.Equal(44, hash.Length);
        Assert.NotEqual(token, hash);
    }
}
