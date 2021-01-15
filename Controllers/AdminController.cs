using live.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace live.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly LiveMultiContext _context;

        public AdminController(LiveMultiContext context)
        {
            _context = context;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.id == id);
        }


        [HttpGet("{id}")]
        public ActionResult<Admin> Get(int id)
        {
            return _context.Admins.Find(id);
        }

        //api/Admin/3
        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUserByuserId(int userId)
        {
            var user =await _context.Users.FindAsync(userId);
            ResultState resultState = new ResultState();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            if (!UserExists(user.id))
            {
                resultState.success = true;
                resultState.code = 1;
                resultState.message = "成功删除";
                return new JsonResult(resultState);
            }
            resultState.message = "删除失败";
            return new JsonResult(resultState);
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
                resultState.message = "用户名或密码错误";
                return new JsonResult(resultState);
            }


        }






        public bool UserNameExists(string name)
        {
            return _context.Admins.Any(e => e.name == name);
        }



    }
}