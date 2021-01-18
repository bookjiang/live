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
    [Route("api/[controller]/[action]")]
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

        private bool UserNameExists(string name)
        {
            return _context.Users.Any(e => e.name == name);
        }

        [HttpGet("{id}")]
        public ActionResult<Admin> Get(int id)
        {
            return _context.Admins.Find(id);
        }

        /// <summary>
        /// 查询用户表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("UserInfoList")]
        public JsonResult UserInfoList([FromBody] QueryParameters query)
        {
            int count = _context.Users.Count();
            List<User> temp = new List<User>();
            //初始化用户表
            PageInfoList pageUsers = new PageInfoList();
            pageUsers.items = temp;
            pageUsers.count = count;
            pageUsers.pageIndex = query.pageIndex;
            pageUsers.pageSize = query.pageSize;
            //查询
            if (query.pageIndex <= 0)
            {
                temp = (List<User>)_context.Users.Take(query.pageSize).ToList();
                pageUsers.items = temp;
                pageUsers.pageIndex = 1;
            }
            else if (query.pageSize * query.pageIndex > count)
            {
                temp = (List<User>)_context.Users.Skip(count - (count % query.pageSize)).Take((count % query.pageSize)).ToList();
                pageUsers.items = temp;
                pageUsers.pageIndex = count / query.pageSize + 1;
            }
            else
            {
                temp = _context.Users.Skip((query.pageIndex - 1) * query.pageSize).Take(query.pageSize).ToList();
                pageUsers.items = temp;
            }

            ResultState resultState = new ResultState();
            resultState.success = true;
            resultState.code = 1;
            resultState.message = "查询成功";
            resultState.value = pageUsers;
            return new JsonResult(resultState);
        }

        /// <summary>
        /// 根据用户Id删除用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>     
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

        /// <summary>
        /// 添加普通用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>     
        [HttpPost("AddUser")]  
        public JsonResult AddUser(User user)
        {
            ResultState resultState = new ResultState();
            if(UserNameExists(user.name))
            {
                resultState.message = "用户名已存在";
                return new JsonResult(resultState);
            }
            _context.Users.Add(user);
            _context.SaveChanges();
            resultState.success = true;
            resultState.code = 1;
            resultState.message = "添加用户成功";
            resultState.value = user;
            return new JsonResult(resultState);
        }

        /// <summary>
        /// 添加管理员用户
        /// </summary>
        /// <param name="Admin"></param>
        /// <returns></returns>       
        [HttpPost("AddAdmin")]  
        public JsonResult AddAdmin(User Admin)
        {
            ResultState resultState = new ResultState();
            if (UserNameExists(Admin.name))
            {
                resultState.message = "用户名已存在";
                return new JsonResult(resultState);
            }
            _context.Users.Add(Admin);
            _context.SaveChanges();
            resultState.success = true;
            resultState.code = 1;
            resultState.message = "添加管理员成功";
            resultState.value = Admin;
            return new JsonResult(resultState);
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>       
        [HttpPut]
        public JsonResult ModifyUserByUser(User user)
        {           
            ResultState resultState = new ResultState();
            var user1 = _context.Users.Find(user.id);
            if(user.name == user1.name& user.tel == user1.tel&user.id_no == user1.id_no & user.role == user1.role & user.psd == user1.psd)
            {
                resultState.message = "未做任何修改";
                return new JsonResult(resultState);
            }
            else if(user.name != user1.name&UserNameExists(user.name))
            {
                resultState.message = "用户名已存在 请重新设定";
                resultState.value = user1;
                return new JsonResult(resultState);
            }  
            //修改用户信息
            user1.name = user.name;
            user1.id_no = user.id_no;
            user1.tel = user.tel;
            user1.role = user.role;
            user1.psd = user.psd;
            _context.SaveChanges();
            resultState.success = true;
            resultState.message = "修改成功";
            resultState.code = 1;
            resultState.value = user;
            return new JsonResult(resultState);
        }


        /// <summary>
        /// 管理员登录
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>     
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

      

        //视频上下架





    }
}