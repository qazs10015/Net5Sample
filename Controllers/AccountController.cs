using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using webapi1.Helpers;
using webapi1.Models.Login;
//using webapi1.Models;

namespace webapi1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public JwtHelpers jwt { get; }
        public AccountController(JwtHelpers jwt)
        {
            this.jwt = jwt;
        }
        [HttpPost("")]
        public ActionResult<LoginResult> Login(Login model)
        {
            if (model.userName == "junyu")
            {
                // 如果需要設定角色權限可以自行設定role
                // 範例為Admin
                return new LoginResult() { Token = jwt.GenerateToken(model.userName, "Admin") };
            }
            else
            {
                return BadRequest();
            }
        }

    }
}