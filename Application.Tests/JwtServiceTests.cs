using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Xunit;

public class JwtServiceTests
{
    private const string Key = "a-test-key-with-at-least-32-bytes";

    private static JwtService CreateService(string key = Key, string issuer = "issuer", string audience = "audience") =>
        new(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:SigningKey"] = key,
            ["Jwt:Issuer"] = issuer,
            ["Jwt:Audience"] = audience
        }).Build());

    [Fact]
    public async Task GeneratedTokenContainsClaimsAndValidates()
    {
        var service = CreateService();
        var token = await service.GenerateToken("user@example.com", "admin");
        var claims = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims;
        Assert.Contains(claims, claim => claim.Type == ClaimTypes.Email && claim.Value == "user@example.com");
        Assert.Contains(claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "admin");
        Assert.True(await service.ValidateToken(token));
    }

    [Fact]
    public async Task InvalidTokensAreRejected()
    {
        var service = CreateService();
        var token = await service.GenerateToken("user@example.com", "admin");
        Assert.False(await service.ValidateToken("not-a-token"));
        Assert.False(await CreateService("another-test-key-at-least-32-bytes").ValidateToken(token));
        Assert.False(await CreateService(issuer: "other").ValidateToken(token));
        Assert.False(await CreateService(audience: "other").ValidateToken(token));

        var expired = new JwtSecurityToken("issuer", "audience", expires: DateTime.UtcNow.AddMinutes(-1),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)), SecurityAlgorithms.HmacSha256));
        Assert.False(await service.ValidateToken(new JwtSecurityTokenHandler().WriteToken(expired)));
    }

    [Fact]
    public void ShortSigningKeyIsRejected() => Assert.Throws<InvalidOperationException>(() => CreateService("short"));
}