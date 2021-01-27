﻿using live.Models;
using live.utils;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace live.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("any")]
    public class MusicUsersController : ControllerBase 
    {
        private LiveMultiContext _context;
        private readonly ICookieHelper _helper;

        public MusicUsersController(LiveMultiContext context, ICookieHelper helper)
        {
            _context = context;
            _helper = helper;
        }

        private int GetRole()
        {          
            String s = _helper.GetCookie("token");
            var a = s.Split(",");
            return int.Parse(a[4]);
        }

        private bool UserExists(int id)
        {
            return _context.MusicUsers.Any(e => e.id == id);
        }

        private bool UserNameExists(string name)
        {
            return _context.MusicUsers.Any(e => e.name == name);
        }

        private ResultState CheckCookie()
        {

            string s = _helper.GetCookie("token");
            //DateTime dateTime = _helper
            //尝试获取cookie的时间属性没有成功。

            if (s == null)
            {
                return new ResultState(false, "请登录", 0, null);
            }
            var a = s.Split(",");
            try
            {
                var user = _context.MusicUsers.Find(int.Parse(a[0]));
                if (user != null)
                {
                    return new ResultState(true, "验证成功", 1, null);
                }
                else
                {
                    return new ResultState(false, "无效cookie,不存在操作用户", 0, null);

                }
            }
            catch (Exception e)
            {
                return new ResultState(false, "无效cookie", 0, null);
            }


        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="user">User</param>
        /// <returns></returns>
        [HttpPost("logon")]
        public JsonResult register([FromBody] MusicUser user)
        {
            ResultState resultState = new ResultState();
            if (UserNameExists(user.name))
            {
                resultState.success = false;
                resultState.message = "用户已存在";
                return new JsonResult(resultState);
            }

            _context.MusicUsers.Add(user);
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
        public JsonResult login([FromBody] MusicUser user)
        {
            //string cookie = _helper.GetCookie(user.id.ToString());
            ////如果用户已经登录则返回
            //if(cookie!=null)
            //{
            //    return new JsonResult(new ResultState(true, "已登录", 0, user));
            //}

            ResultState resultState = new ResultState();
            if (!UserNameExists(user.name))
            {
                resultState.success = false;
                resultState.message = "用户名不存在";
                return new JsonResult(resultState);
            }
            var user1 = _context.MusicUsers.Where(x => x.name == user.name).FirstOrDefault();  //lambda表达式写错=>写成==>
            if (user.psd == user1.psd)
            {
                if(user1.role ==1 && user.role == 0)
                {
                    resultState.success = false;
                    resultState.message = "登录失败 权限不够";
                    return new JsonResult(resultState);
                }                
                resultState.success = true;
                resultState.message = "登录成功";
                resultState.value = user1;
                _helper.SetCookie("token", user1.id + "," + user1.name + "," + user1.tel + "," + user1.id_no + "," + user1.role, 66);
                return new JsonResult(resultState);
            }           
            resultState.success = false;
            resultState.message = "密码错误";
            return new JsonResult(resultState);
          


        }

        /// <summary>
        /// 用户信息更新
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("updateInfo")]
        public JsonResult updateInfo([FromBody] MusicUser user)
        {
            ResultState resultState = CheckCookie();
            if (resultState.code == 1)
            {
                //ResultState resultState = new ResultState();
                if (!UserExists(user.id))
                {
                    resultState.success = false;
                    resultState.message = "用户id不存在";
                    return new JsonResult(resultState);
                }
                var user1 = _context.MusicUsers.Find(user.id);

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
            return new JsonResult(resultState);





        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("userInfoList")]
        public JsonResult userInfoList([FromBody] QueryParameters query)
        {
            ResultState resultState = CheckCookie();
            if (resultState.code == 1 && GetRole() == 0)
            {
                int count = _context.MusicUsers.Count();
                List<MusicUser> temp = new List<MusicUser>();
                PageInfoList pageUsers = new PageInfoList();
                if (query.pageIndex <= 0)
                {
                    temp = (List<MusicUser>)_context.MusicUsers.Take(query.pageSize).ToList();
                    pageUsers.items = temp;
                    pageUsers.count = count;
                    pageUsers.pageIndex = 1;
                    pageUsers.pageSize = query.pageSize;
                }
                else if (query.pageSize * query.pageIndex > count)
                {
                    temp = (List<MusicUser>)_context.MusicUsers.Skip(count - (count % query.pageSize)).Take((count % query.pageSize)).ToList();
                    pageUsers.items = temp;
                    pageUsers.count = count;
                    pageUsers.pageIndex = count / query.pageSize + 1;
                    pageUsers.pageSize = query.pageSize;
                }
                else
                {
                    temp = _context.MusicUsers.Skip((query.pageIndex - 1) * query.pageSize).Take(query.pageSize).ToList();
                    pageUsers.items = temp;
                    pageUsers.count = count;
                    pageUsers.pageIndex = query.pageIndex;
                    pageUsers.pageSize = query.pageSize;
                }


                resultState.success = true;
                resultState.message = "查询成功";
                resultState.value = pageUsers;
                return new JsonResult(resultState);

            }
            if(GetRole() == 1)
            {
                resultState.success = false;
                resultState.message = "权限不够 无法查看";
            }
            return new JsonResult(resultState);

        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        public JsonResult delete(int id)
        {         
            ResultState resultState = CheckCookie();
            if(GetRole() == 1)
            {
                resultState.success = false;
                resultState.message = "用户权限不够";
                return new JsonResult(resultState);
            }
            if (resultState.code == 1)
            {
                var user = _context.MusicUsers.Find(id);
                if (user == null)
                {
                    resultState.success = false;
                    resultState.message = "用户不存在";

                    return new JsonResult(resultState);
                }
                if (user.role == 0)
                {
                    return new JsonResult(new ResultState(false, "权限不够，不能删除其他管理员用户", 0, user));

                }

                _context.MusicUsers.Remove(user);
                _context.SaveChanges();
                resultState.success = true;
                resultState.message = "删除成功";
                return new JsonResult(resultState);

            }
            return new JsonResult(resultState);

        }

        /// <summary>
        /// 登出，删除cookie
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("logout/{id}")]
        public JsonResult logout(int id)
        {
            _helper.DeleteCookie("token");
            return new JsonResult(new ResultState(true, "注销成功", 1, id));

        }

        /// <summary>
        /// 刷新cookie
        /// </summary>
        /// <returns></returns>
        [HttpGet("refresh")]
        public JsonResult refresh()
        {
            ResultState resultState = CheckCookie();
            if (resultState.code == 1)
            {
                string s = _helper.GetCookie("token");
                _helper.SetCookie("token", s, 66);
                return new JsonResult(new ResultState(true, "刷新成功", 1, _helper.GetCookie("token")));

            }
            return new JsonResult(resultState);


        }
    }


}
