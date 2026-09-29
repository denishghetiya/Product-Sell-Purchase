using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ProductSellPurchase.ViewModels;

namespace ProductSellPurchase.Helper
{
    public interface IJwtHelper
    {
        string GenerateJSONWebToken(UserInfoViewModel userInfo);
    }

    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _config;

        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJSONWebToken(UserInfoViewModel userInfo)
        {
            var Key = _config.GetValue<string>("AppSettings:Secret");
            var Issuer = _config.GetValue<string>("BaseAPIURL");
            var audience = _config.GetValue<string>("WebBaseUrl");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var permClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", userInfo.UserId.ToString()),
                new Claim("Username", userInfo.Username),
                new Claim("Email", userInfo.Email),
                new Claim("UserTypeId", userInfo.UserTypeId.ToString()),
                new Claim("UserTypeName", userInfo.UserTypeName),
            };

            var token = new JwtSecurityToken(
                Issuer,
                audience,
                permClaims,
                expires: DateTime.UtcNow.AddMinutes(!string.IsNullOrEmpty(_config.GetSection("TokenExpirationTimeout").Value)
                    ? double.Parse(_config.GetSection("TokenExpirationTimeout").Value)
                    : 60),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
