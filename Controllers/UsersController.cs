using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using live.Models;
using Microsoft.AspNetCore.Mvc;

namespace live.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private  LiveMultiContext _context;

        public UsersController(LiveMultiContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.id == id);
        }

        public bool UserNameExists(string name)
        {
            return _context.Users.Any(e => e.name == name);
        }




        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("logon")]
        public JsonResult register(User user)
        {
            ResultState resultState = new ResultState();
            if (UserNameExists(user.name))
            {
                resultState.success = false;
                resultState.message = "用户已存在";
                return new JsonResult(resultState);
            }

            _context.Users.Add(user);
            _context.SaveChanges();
            resultState.success = true;
            resultState.code = 1;
            resultState.message = "注册成功";
            resultState.value = user;
            return new JsonResult(resultState);
        }



        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public JsonResult login(User user)
        {
            ResultState resultState = new ResultState();
            if (!UserNameExists(user.name))
            {
                resultState.success = false;
                resultState.message = "用户名不存在";
                return new JsonResult(resultState);
            }
            var user1 = _context.Users.Where(x => x.name == user.name).FirstOrDefault();  //lambda表达式写错=>写成==>
            if (user.psd == user1.psd)
            {
                resultState.success = true;
                resultState.message = "登录成功";
                resultState.value = user1;
                return new JsonResult(resultState);

            }
            else
            {
                resultState.success = false;
                resultState.message = "密码错误";
                return new JsonResult(resultState);
            }


        }

        [HttpPut("updateInfo")]
        public JsonResult updateInfo(User user)
        {
            ResultState resultState = new ResultState();
            if (!UserExists(user.id))
            {
                resultState.success = false;
                resultState.message = "用户id不存在";
                return new JsonResult(resultState);
            }
            var user1 = _context.Users.Find(user.id);

            if (user1.name != user.name && UserNameExists(user.name))
            {
                resultState.success = false;
                resultState.message = "用户名已存在，请更换";
                return new JsonResult(resultState);

            }
            //直接用user1=user赋值不行，原理未知
            user1.name = user.name;
            user1.id_no = user.id_no;
            user1.tel = user.tel;
            user1.psd = user.psd;
            //_context.Users.Update(user);
            var count = _context.SaveChanges();
            if (count == 1)
            {
                resultState.success = true;
                resultState.message = "信息更新成功";
                resultState.value = user;
            }
            else
            {
                resultState.success = false;
                resultState.message = "信息更新失败";
                return new JsonResult(resultState);
            }
                
            return new JsonResult(resultState);



        }




        [HttpPost("userInfoList")]
        public JsonResult list([FromBody] QueryParameters query)
        {
            int count = _context.Users.Count();
            int pageSize1 = query.pageSize;
            List<User> temp = new List<User>();
            PageInfoList pageUsers = new PageInfoList();
            if (query.pageIndex<=0)
            {
                 temp = (List<User>)_context.Users.Take(query.pageSize).ToList();
                pageUsers.items = temp;
                pageUsers.count = count;
                pageUsers.pageIndex = 1;
                pageUsers.pageSize = query.pageSize;
            }
            else if(query.pageSize * query.pageIndex >count)
            {
                 temp = (List<User>)_context.Users.Skip(count - (count % query.pageSize)).Take((count % query.pageSize)).ToList();
                pageUsers.items = temp;
                pageUsers.count = count;
                pageUsers.pageIndex = count / query.pageSize;
                pageUsers.pageSize = query.pageSize;
            }
            else
            {
                 temp = _context.Users.Skip((query.pageIndex - 1) * query.pageSize).Take(query.pageSize).ToList();
                pageUsers.items = temp;
                pageUsers.count = count;
                pageUsers.pageIndex = query.pageIndex;
                pageUsers.pageSize = query.pageSize;
            }

            //PageInfoList<User> pageUsers = new PageInfoList<User>(temp, count, query.pageIndex, query.pageSize);
            //pageUsers.items = temp;
            //pageUsers.count = count;
            //pageUsers.pageIndex = query.pageIndex;
            //pageUsers.pageSize = query.pageSize;
            ResultState resultState = new ResultState();
            resultState.success = true;
            resultState.message = "查询成功";
            resultState.value = pageUsers;
            return new JsonResult(resultState);


        }








    }
}
