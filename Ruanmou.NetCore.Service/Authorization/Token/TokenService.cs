﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ruanmou04.NetCore.Service.Core.Tokens.Dtos;
using Ruanmou04.EFCore.Model.DtoHelper;
using Ruanmou04.EFCore.Model.Token.Dtos;
using Ruanmou.NetCore.Interface;
using System.IdentityModel.Tokens.Jwt;

namespace Ruanmou04.NetCore.Service.Core.Authorization.Tokens
{
    /// <summary>
    /// 令牌实现
    /// </summary>
    public class TokenService : ITokenService
    {

        ISysUserService _usersRepository;
        TokenAuthConfiguration _configuration;
        IAuthenticationService _authenticationService;
        IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="tokenInformationRepository"></param>
        /// <param name="tokenInformationDetailRepository"></param>
        /// <param name="configurationUserSystemsRepository"></param>
        public TokenService(


             TokenAuthConfiguration configuration,
             IAuthenticationService authenticationService,
             IHttpContextAccessor httpContextAccessor,
             ISysUserService usersRepository
             )
        {
            _authenticationService = authenticationService;
            _configuration = configuration;// new TokenAuthConfiguration(); 
            this.httpContextAccessor = httpContextAccessor;
            _usersRepository = usersRepository;
        }

        /// <summary>
        /// 生成令牌 并保存到数据库
        /// </summary>
        /// <param name="account"></param>
        /// <param name="systemID"></param>
        /// <returns>string</returns>
        public async Task<AjaxResult> GenerateTokenAsync(GenerateTokenDto generateDto)
        {
            AjaxResult result = new AjaxResult("");
            try
            {
                var tokenInfoMation = new TokenInformation();
                tokenInfoMation = await CreateTokenDataAsync(generateDto, tokenInfoMation);
                result.data = tokenInfoMation.Token;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message);
                result.success = false;
            }
            result.success = true;
            return result;
        }

        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>AjaxResult</returns>
        public async Task<AjaxResult> ConfirmVerificationAsync(VerificationTokenDto dto)
        {
            AjaxResult result = new AjaxResult("");
            try
            {

#warning 未实现
                var tokenDetailObj = new TokenInformation();

                if (tokenDetailObj == null) //没有令牌 
                {
                    result.msg = "未找到token信息,账号或SystemID有误";
                }
                else
                {
                    if (tokenDetailObj.FailureTime < DateTime.Now) //已过期 
                    {
                        result.msg = "token已过期，请重新登录";
                    }
                    else
                    {
                        result.success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message);
                result.success = false;
            }
            return result;
        }

        /// <summary>
        /// 创建系统与令牌关联数据 TokenInformationDetail
        /// </summary>
        /// <param name="tokenID"></param>
        /// <param name="systemID"></param>
        /// <returns></returns>
        private TokenInformationDetail CreateTokenDetailData(Guid tokenID, string systemID)
        {
            TokenInformationDetail tokenDetail = null;
            try
            {
                tokenDetail = new TokenInformationDetail()
                {
                    Id = Guid.NewGuid(),
                    SystemID = systemID,
                    TokenInformationID = tokenID
                };
                //tokenDetail = tokenInformationDetailRepository.Insert(tokenDetail);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message);
            }
            return tokenDetail;
        }

        /// <summary>
        /// 新建token数据
        /// </summary>
        /// <param name="account"></param>
        /// <param name="systemID"></param>
        /// <returns></returns>
        private async Task<TokenInformation> CreateTokenDataAsync(GenerateTokenDto generateDto, TokenInformation tokenInformation)
        {
            //TokenInformation token = null;
            try
            {
                if (tokenInformation == null)
                {
                    tokenInformation = new TokenInformation()
                    {
                        Id = Guid.NewGuid(),
                        Account = generateDto.Account,
                        Token = await CreateTokenAsync(generateDto),
                        IsEffective = 0,//正常
                        FailureTime = DateTime.Now.Add(_configuration.Expiration)
                    };
                   // tokenInformationRepository.Insert(tokenInformation);
                }
                else
                {
                    await CreateTokenAsync(generateDto, tokenInformation);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message);
            }
            return tokenInformation;
        }

        /// <summary>
        /// 根据账号和userid生成token
        /// </summary>
        /// <param name="account"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        private async Task<string> CreateTokenAsync(GenerateTokenDto generateDto, TokenInformation tokenInformation = null)
        {
            var accessToken = "";
            long longUserId = 1;

            //generateDto.Id.ToList().ForEach(x => { if (x != 0) longUserId = longUserId * x; });
            ClaimsIdentity identity = new ClaimsIdentity(Ruanmou.Core.Utility.StaticConstraint.AuthenticationScheme);//
            identity.AddClaim(new Claim(ClaimTypes.Name, generateDto.Name.ToString()));
            //identity.AddClaim(new Claim(ClaimTypes.PrimarySid, generateDto.Account.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.PrimarySid, generateDto.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Sid, longUserId.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, generateDto.Id.ToString()));

            TimeSpan tokenExpiration = _configuration.Expiration;
            if (tokenInformation == null)
            {
                //tokenExpiration = TimeSpan.FromHours(10);
                accessToken = WriteAccessToken(CreateJwtClaims(identity), tokenExpiration);
            }
            //else
            //{
            //    tokenExpiration = tokenInformation.FailureTime.Value - DateTime.Now;
            //}
            //var props = new AuthenticationProperties()
            //{
            //    IssuedUtc = DateTime.Now,
            //    ExpiresUtc = DateTime.Now.Add(tokenExpiration),
            //    IsPersistent = true
            //};
            //ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            //await _authenticationService.SignInAsync(httpContextAccessor.HttpContext, CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
            return accessToken;
        }

        /// <summary>
        /// 根据身份写入令牌
        /// </summary>
        /// <param name="account"></param>
        /// <returns>string</returns>
        private string WriteAccessToken(IEnumerable<Claim> claims, TimeSpan expiration)
        {
            var now = DateTime.Now;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration),
                signingCredentials: _configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        /// <summary>
        /// 创建身份
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        private List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });
            return claims;
        }

        /// <summary>
        /// 注销token 
        /// 1. 删除数据库token数据  2.删除写入的token数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<AjaxResult> LoginOutTokenAsync(LoginOutTokenDto loginOutTokenDto)
        {
            AjaxResult result = new AjaxResult("");
            try
            {
                //var tokenObj = await tokenInformationRepository.FirstOrDefaultAsync(x => x.Token == loginOutTokenDto.Token);
                //if (tokenObj != null)
                //{
                //    await tokenInformationRepository.DeleteAsync(tokenObj);
                //    await SignOutTokenAsync(loginOutTokenDto);
                //    result.success = true;
                //}
            }
            catch (Exception ex)
            {
                result.msg = ex.Message;
                result.success = false;
            }
            return result;
        }

        /// <summary>
        /// 删除写入的token
        /// </summary>
        /// <param name="account"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        private async Task SignOutTokenAsync(LoginOutTokenDto loginOutTokenDto)
        {
            var props = new Microsoft.AspNetCore.Authentication.AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                IsPersistent = true
            };
            await _authenticationService.SignOutAsync(httpContextAccessor.HttpContext, Ruanmou.Core.Utility.StaticConstraint.AuthenticationScheme, props);
        }

        /// <summary>
        /// 各句token获取TokenInformation的实体
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<TokenInformation> GetTokenInformationByTokenAsync(string token)
        {
            TokenInformation info = null;
            if (token != "")
            {
                //info = await tokenInformationRepository.FirstOrDefaultAsync(x => x.Token == token);
            }
            return info;
        }
    }
}
