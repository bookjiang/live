using live.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace live.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly LiveMultiContext _context;

        public AdminController(LiveMultiContext context)
        {
            _context = context;
        }


        [HttpGet("{id}")]
        public ActionResult<Admin> Get(int id)
        {
            return _context.Admins.Find(id);
        }

        [HttpPost("login")]
        public JsonResult login(Admin admin)
        {
            ResultState resultState = new ResultState();
            if (!UserNameExists(admin.name))
            {
                resultState.success = false;
                resultState.message = "用户名不存在";
                return new JsonResult(resultState);
            }
            var admin1 = _context.Admins.Where(x => x.name == admin.name).FirstOrDefault();
            if (admin.psd == admin1.psd)
            {
                resultState.success = true;
                resultState.message = "登录成功";
                resultState.value = admin1;
                return new JsonResult(resultState);

            }
            else
            {
                resultState.success = false;
                resultState.message = "密码错误";
                return new JsonResult(resultState);
            }



        }
        private bool UserNameExists(string name)
        {
            return _context.Admins.Any(e => e.name == name);
        }



    }
}