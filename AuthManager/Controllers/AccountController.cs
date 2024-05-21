using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using AuthManager.Contract;
using MSCore.Util.JwtToken;
using System.Security.Claims;
//using ServiceAdapter.JwtToken;

namespace AuthManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenUtils _jwtTokenUtils;
        public AccountController(IConfiguration configuration, IJwtTokenUtils jwtTokenUtils)
        {
            _configuration = configuration;
            _jwtTokenUtils = jwtTokenUtils;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="usermodel">登录参数</param>
        /// <returns>返回用户信息及token</returns>
        [HttpPost]
        [Route("Login")]
        public object Login([FromBody] LoginModel usermodel)
        {
            var user = new { userName = "admin", passWord = "123", userCode = "1001" }; //_dbContext.SYS_User.Where(d => d.userName == usermodel.UserName && d.isDel == false && d.status == 0).FirstOrDefault();

            if (user != null)
            {
                if (user.passWord != usermodel.PassWord)
                {
                    return new { success = false, message = "用户密码错误！" };
                }

                #region 生成token

                //生成token，去掉敏感字段信息
                IEnumerable<Claim> claims = new[] { new Claim("role", "admin") };
                var view = _jwtTokenUtils.getToken(new { userName = "admin", userCode = "1001" }, claims);

                #endregion
                #region 保存刷新令牌短码

                //user.shortToken = view.RefreshToken.Split(".")[2];
                //_dbContext.SYS_User.Update(user);
                //_dbContext.SaveChanges();

                #endregion
                return new { success = true, data = view, message = "登录成功。" };
            }
            else
            {
                return new { success = false, message = "系统不存在该用户！" };
            }
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Logout")]
        public ActionResult<Object> Logout()
        {
            //var user =_dbContext.SYS_User.Where(v => v.userCode.Equals(CurrentUserId)).FirstOrDefault();
            //if (user == null) return new { success = true, data = "", message = "已注销" };
            //user.shortToken = "";
            //_dbContext.SYS_User.Update(user);
            //_dbContext.SaveChanges();

            return new { success = true, data = "", message = "已注销" };
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <param name="refreshModel">访问令牌参数</param>
        /// <returns>返回令牌信息</returns>
        [HttpPost("refresh")]
        [Authorize(Roles = "refresh")]//验证权限
        public Object Refresh(RefreshModel refreshModel)
        {
            var user = new { userName = "admin", passWord = "123", userCode = "1001", roles = "用户", shortToken = "U-mEAJlvwdQ16V6V1w60PuHnNFKAwO_VJOVdrX_CKb4" }; //_dbContext.SYS_User.Where(v => v.userCode.Equals(CurrentUserId)).FirstOrDefault();
            if (user == null)
            {
                return BadRequest(new { success = false, message = "传入访问令牌错误" });
            }
            var oldToken = Request.Headers["Authorization"].ToString().Substring(7).Split(".")[2];

            if (user.shortToken != oldToken)
            {
                return Unauthorized(new { success = false, message = "刷新令牌已失效" });
            }
            IEnumerable<Claim> claims = new[] { new Claim("role", "admin") };
            var view = _jwtTokenUtils.refresh(refreshModel, user,claims);
            ((IDictionary<string, object>)view).TryGetValue("success", out object code);
            ((IDictionary<string, object>)view).TryGetValue("data", out object data);
            if (code.ToString() == "false")
            {
                return BadRequest(view);
            }

            // 返回成功信息，写出token
            return new { success = true, data = data, message = "换取token成功。" };
        }

        /// <summary>
        /// hello
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "user")]
        [HttpGet("hello")]
        public Object hello()
        {
            // 返回成功信息，写出token
            return new { success = true, data = "adfad", message = "hello" };
        }


        /// <summary>
        /// hello
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpGet("helloadmin")]
        public Object helloadmin()
        {
            // 返回成功信息，写出token
            return new { success = true, data = "adfad11111", message = "hello11111" };
        }

    }
}
