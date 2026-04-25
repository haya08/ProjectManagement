using ProjectManagement.DTOs.Users;
using ProjectManagement.Models;
using System.IdentityModel.Tokens.Jwt;

namespace ProjectManagement.BL.Interfaces
{
    public interface IJWT
    {
        Task<JwtSecurityToken> GenerateToken(ApplicationUser user);
        TbRefreshToken GenerateRefreshToken();
        void SetRefreshTokenInCookie(string refreshToken, DateTime expires);
    }
}
