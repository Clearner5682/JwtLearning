using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace _01_Demo
{
    public class JwtHelper
    {
        public static string IssueJwt(IConfiguration configuration,TokenModelJwt tokenModel)
        {
            string iss = configuration["JwtSetting:Issuer"];
            string aud = configuration["JwtSetting:Audience"];
            string secretKey = configuration["JwtSetting:SecretKey"];


            // 声明，jwt主体
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Jti,tokenModel.Id.ToString()),// jwt id
                new Claim(JwtRegisteredClaimNames.Iat,new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),//issued at，签发时间(时间戳)
                new Claim(JwtRegisteredClaimNames.Nbf,new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),//not before，jwt生效开始时间(时间戳)
                new Claim(JwtRegisteredClaimNames.Exp,new DateTimeOffset(DateTime.Now.AddSeconds(100)).ToUnixTimeSeconds().ToString()),//expire，过期时间(时间戳)
                new Claim(JwtRegisteredClaimNames.Iss,iss),//issuer，签发人
                new Claim(JwtRegisteredClaimNames.Aud,aud),//audience，jwt接收人
            };
            claims.AddRange(tokenModel.Roles.Split(',').Select(o => new Claim(ClaimTypes.Role, o)));

            // 秘钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 生成Signature
            var jwt = new JwtSecurityToken(issuer:iss,audience:aud,claims:claims,signingCredentials:creds);
            var jwtHandler = new JwtSecurityTokenHandler();
            var encodedJwt= jwtHandler.WriteToken(jwt);

            return encodedJwt;
        }
    }
}
