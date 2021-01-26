using live.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using live.utils;

namespace live.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("any")]
    public class AdminController : ControllerBase
    {
        private readonly LiveMultiContext _context;
        private readonly ICookieHelper _helper;


        public AdminController(LiveMultiContext context, ICookieHelper helper)
        {
            _context = context;
            _helper = helper;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.id == id);
        }

        private bool UserNameExists(string name)
        {
            return _context.Users.Any(e => e.name == name);
        }
        private bool AdminNameExists(string name)
        {
            return _context.Admins.Any(e => e.name == name);
        }
        private bool KeywordExists(string keyword)
        {
            return _context.KeyWords.Any(e => e.keyword == keyword);
        }

        /// <summary>
        /// 查询管理员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<Admin> Get(int id)
        {
            return _context.Admins.Find(id);
        }

        /// <summary>
        /// 修改管理员个人信息
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        [HttpPut("UpdateAdminInfo")]
        public JsonResult UpdateAdminInfo(Admin admin)
        {
            ResultState resultState = CheckCookie();
            if (resultState.code == 1)
            {
                var admin1 = _context.Admins.Find(admin.id);
                if (admin.name == admin1.name && admin.tel == admin1.tel && admin.role == admin1.role && admin.psd == admin1.psd)
                {
                    resultState.message = "未做任何修改";
                    return new JsonResult(resultState);
                }
                else if (admin.name != admin1.name && AdminNameExists(admin.name))
                {
                    resultState.message = "用户名已存在 请重新设定";
                    resultState.value = admin1;
                    return new JsonResult(resultState);
                }
                //修改用户信息
                admin1.name = admin.name;
                admin1.tel = admin.tel;
                admin1.role = admin.role;
                admin1.psd = admin.psd;
                _context.SaveChanges();
                resultState.success = true;
                resultState.message = "修改成功";
                resultState.code = 1;
                resultState.value = admin;
            }
            return new JsonResult(resultState);
        }
       


        /// <summary>
        /// 通过id查询video
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetVideo/{id}")]
        public ActionResult<RecordVideo> GetVideo(int id)
        {
            ResultState resultState = CheckCookie();
            if(resultState.code == 1)
            {
                resultState.value = _context.RecordVideos.Find(id);
                return new JsonResult(resultState);
            }
            resultState.success = false;
            return new JsonResult(resultState);
        }


        /// <summary>
        /// 通过id查询User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetUser/{id}")]
        public JsonResult GetUser(int id)
        {
            ResultState resultState = CheckCookie();
            if (resultState.code == 1)
            {
                resultState.value = _context.Users.Find(id);
                if (resultState.value == null)
                {
                    resultState.success = false;
                    resultState.message = "用户不存在";

                }

            }
            return new JsonResult(resultState);


        }


        /// <summary>
        /// 查询用户表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns> 
        [HttpPost("UserInfoList")]
        public JsonResult UserInfoList([FromBody] QueryParameters query)
        {
            ResultState resultState = CheckCookie();
            if (resultState.code == 1)
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
                resultState.success = true;
                resultState.code = 1;
                resultState.message = "查询成功";
                resultState.value = pageUsers;

            }

            
            return new JsonResult(resultState);
        }

        /// <summary>
        /// 根据用户Id删除用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>     
        [HttpDelete("DeleteUser/{userId}")]
        public async Task<ActionResult> DeleteUserByuserId(int userId)
        {
            ResultState resultState = CheckCookie();

            if (resultState.code == 1)
            {               
                var user =await _context.Users.FindAsync(userId);
                if(user == null)
                {
                    resultState.success = false;
                    resultState.message = "不存在此用户";
                    return new JsonResult(resultState);
                }                   
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                if (!UserExists(user.id))
                {
                    resultState.message = "成功删除";
                    return new JsonResult(resultState);
                }
                resultState.success = false;
                resultState.message = "删除失败";
            }
            
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
            ResultState resultState = CheckCookie();
            if (resultState.code == 1)
            {
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
            }
            return new JsonResult(resultState);
        }

        /// <summary>
        /// 添加管理员用户
        /// </summary>
        /// <param name="Admin"></param>
        /// <returns></returns>       
        [HttpPost("AddAdmin")]  
        public JsonResult AddAdmin(Admin Admin)
        {
            ResultState resultState = CheckCookie();
            if (resultState.code == 1)
            {      
                if (AdminNameExists(Admin.name))
                {
                    resultState.success = false;
                    resultState.message = "管理员用户名已存在";
                    return new JsonResult(resultState);
                }
                _context.Admins.Add(Admin);
                _context.SaveChanges();
                resultState.message = "添加管理员成功";
                resultState.value = Admin;       
            }
            return new JsonResult(resultState);
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>       
        [HttpPut("ModifyUserByUser")]
        public JsonResult ModifyUserByUser(User user)
        {
            ResultState resultState = CheckCookie();
            if (resultState.code == 1)
            {
                var user1 = _context.Users.Find(user.id);
                if(user.name == user1.name && user.tel == user1.tel && user.id_no == user1.id_no && user.role == user1.role && user.psd == user1.psd && user.status == user1.status)
                {
                    resultState.message = "未做任何修改";
                    return new JsonResult(resultState);
                }
                else if(user.name != user1.name && UserNameExists(user.name))
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
                user1.status = user.status;
                _context.SaveChanges();
                resultState.success = true;
                resultState.message = "修改成功";
                resultState.code = 1;
                resultState.value = user;
            }
            return new JsonResult(resultState);

        }


        /// <summary>
        /// 管理员登录
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>     
        [HttpPost("login")]
        public JsonResult Login(Admin admin)
        {
            ResultState resultState = new ResultState();
            if (!AdminNameExists(admin.name))
            {
                resultState.message = "用户名不存在";
                return new JsonResult(resultState);
            }
            var admin1 = _context.Admins.Where(x => x.name == admin.name).FirstOrDefault();
            if (admin.psd == admin1.psd)
            {
                resultState.success = true;
                resultState.message = "登录成功";
                resultState.code = 1;
                resultState.value = admin1;
                _helper.SetCookie("token", admin1.id + "," + admin1.name + "," + admin1.tel + ","  + admin1.role, 66);

                return new JsonResult(resultState);

            }
            else
            {
                resultState.message = "用户名或密码错误";
                return new JsonResult(resultState);
            }

        }


        /// <summary>
        /// 管理员退出登录
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("AdminLogout")]
        public JsonResult AdminLogout()
        {

            var resultState = new ResultState();
            _helper.DeleteCookie("token");            
            resultState.success = true;
            resultState.message = "注销成功";
            return new JsonResult(resultState);
        }


        /// <summary>
        /// 查询关键字表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("KeywordInfoList")]
        public JsonResult KeywordInfoList([FromBody] QueryParameters query)
        {
            ResultState resultState = CheckCookie();
            if (resultState.code == 1)
            {
                int count = _context.KeyWords.Count();
                List<Keyword> temp = new List<Keyword>();
                //初始化关键词表
                var pageKeywords = new PageInfoList();
                pageKeywords.items = temp;
                pageKeywords.count = count;
                pageKeywords.pageIndex = query.pageIndex;
                pageKeywords.pageSize = query.pageSize;
                //查询
                if (query.pageIndex <= 0)
                {
                    temp = (List<Keyword>)_context.KeyWords.Take(query.pageSize).ToList();
                    pageKeywords.items = temp;
                    pageKeywords.pageIndex = 1;
                }
                else if (query.pageSize * query.pageIndex > count)
                {
                    temp = (List<Keyword>)_context.KeyWords.Skip(count - (count % query.pageSize)).Take((count % query.pageSize)).ToList();
                    pageKeywords.items = temp;
                    pageKeywords.pageIndex = count / query.pageSize + 1;
                }
                else
                {
                    temp = _context.KeyWords.Skip((query.pageIndex - 1) * query.pageSize).Take(query.pageSize).ToList();
                    pageKeywords.items = temp;
                }
                resultState.success = true;
                resultState.code = 1;
                resultState.message = "查询成功";
                resultState.value = pageKeywords;
            }
            
            return new JsonResult(resultState);
        }


        /// <summary>
        /// 添加关键字
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpPost("AddKeyword")]
        public JsonResult AddKeyword([FromBody] Keyword keyword)
        {
            ResultState resultState = CheckCookie();
            if (resultState.code == 1)
            {
                if (KeywordExists(keyword.keyword))
                {
                    resultState.message = "该关键词已录入";
                    return new JsonResult(resultState);
                }
                _context.KeyWords.Add(keyword);
                _context.SaveChanges();
                resultState.success = true;
                resultState.message = "关键词添加成功";
                resultState.code = 1;
                resultState.value = keyword;
            }              
            return new JsonResult(resultState);
        }

        /// <summary>
        /// 删除关键字
        /// </summary>
        /// <param name="keywordId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteKeyword/{keywordId}")]
        public async Task<ActionResult> DeleteKeywordBykeywordId(int keywordId)
        {
            ResultState resultState = CheckCookie();
            if (resultState.code == 1)
            {
                var keyword = await _context.KeyWords.FindAsync(keywordId);

                _context.KeyWords.Remove(keyword);
                await _context.SaveChangesAsync();
                if (!UserExists(keyword.id))
                {
                    resultState.success = true;
                    resultState.code = 1;
                    resultState.message = "成功删除";
                    return new JsonResult(resultState);
                }
                resultState.message = "删除失败";
            }
            return new JsonResult(resultState);
        }

        /// <summary>
        /// 查询评论总表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("CommentInfoList")]
        public JsonResult CommentInfoList([FromBody] QueryParameters query)
        {
            ResultState resultState = CheckCookie();
            if (resultState.code == 1)
            {
                int count = _context.Comments.Count();
                List<Comment> temp = new List<Comment>();
                //初始化关键词表
                var pageComments = new PageInfoList();
                pageComments.items = temp;
                pageComments.count = count;
                pageComments.pageIndex = query.pageIndex;
                pageComments.pageSize = query.pageSize;
                //查询
                if (query.pageIndex <= 0)
                {
                    temp = (List<Comment>)_context.Comments.Take(query.pageSize).ToList();
                    pageComments.items = temp;
                    pageComments.pageIndex = 1;
                }
                else if (query.pageSize * query.pageIndex > count)
                {
                    temp = (List<Comment>)_context.Comments.Skip(count - (count % query.pageSize)).Take((count % query.pageSize)).ToList();
                    pageComments.items = temp;
                    pageComments.pageIndex = count / query.pageSize + 1;
                }
                else
                {
                    temp = _context.Comments.Skip((query.pageIndex - 1) * query.pageSize).Take(query.pageSize).ToList();
                    pageComments.items = temp;
                }

                resultState.success = true;
                resultState.code = 1;
                resultState.message = "查询成功";
                resultState.value = pageComments;
            }
            return new JsonResult(resultState);
        }

        /// <summary>
        /// 视频上架
        /// </summary>
        /// <param name="vedioId"></param>
        /// <returns></returns>
        [HttpPut("VedioOnline/{vedioId}")]
        public JsonResult VedioOnline(int vedioId)
        {
            ResultState resultState = CheckCookie();
            if (resultState.code == 1)
            {
                var recordVideo = _context.RecordVideos.Find(vedioId);
                if (recordVideo.status == 0)
                {
                    recordVideo.status = 1;
                    _context.SaveChanges();
                    resultState.success = true;
                    resultState.code = 1;
                    resultState.message = "视频上架成功";
                    resultState.value = recordVideo;
                    return new JsonResult(resultState);
                }
                resultState.value = recordVideo;
                resultState.message = "视频已上架 上传失败";
            }
            return new JsonResult(resultState);
        }

        /// <summary>
        /// 视频下架
        /// </summary>
        /// <param name="vedioId"></param>
        /// <returns></returns>
        [HttpPut("VedioOffline/{vedioId}")]
        public JsonResult VedioOffline(int vedioId)
        {
            ResultState resultState = CheckCookie();
            if (resultState.code == 1)
            {
                var recordVideo = _context.RecordVideos.Find(vedioId);
                if (recordVideo.status == 1)
                {
                    recordVideo.status = 0;
                    _context.SaveChanges();
                    resultState.success = true;
                    resultState.code = 1;
                    resultState.message = "视频成功下架";
                    resultState.value = recordVideo;
                    return new JsonResult(resultState);
                }
                resultState.message = "视频已下架 操作失败";
                resultState.value = recordVideo;
            }
            return new JsonResult(resultState);
        }

        /// <summary>
        /// 更新视频信息
        /// </summary>
        /// <param name="recordVideo"></param>
        /// <returns></returns>
        [HttpPut("UpdateVedioInfo")]
        public JsonResult UpdateVedioInfo(RecordVideo recordVideo)
        {
            ResultState resultState = CheckCookie();
            if (resultState.code == 1)
            {
                var recordVideo1 = _context.RecordVideos.Find(recordVideo.id);
                if (recordVideo.status == recordVideo1.status && recordVideo.category == recordVideo1.category && recordVideo.keyword == recordVideo1.keyword)
                {
                    resultState.message = "未做任何修改";
                    resultState.value = recordVideo;
                    return new JsonResult(resultState);
                }
                recordVideo1.status = recordVideo.status;
                recordVideo1.category = recordVideo.category;
                recordVideo1.keyword = recordVideo.keyword;
                _context.SaveChanges();
                resultState.success = true;
                resultState.code = 1;
                resultState.value = recordVideo;
                resultState.message = "视频信息更新成功";
            }
            return new JsonResult(resultState);
        }

        /// <summary>
        /// 管理员获取所有视频列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("videoList")]
        public JsonResult VideoList([FromBody] QueryParameters query)
        {
            ResultState resultState = CheckCookie();
            if (resultState.code == 1)
            {
                int count = _context.RecordVideos.Count();
                List<RecordVideo> temp = new List<RecordVideo>();
                PageInfoList pageUsers = new PageInfoList();
                if (query.pageIndex <= 0)
                {
                    temp = (List<RecordVideo>)_context.RecordVideos.Take(query.pageSize).ToList();
                    //temp = (List<RecordVideo>)_context.RecordVideos.Take(query.pageSize).ToList();
                    pageUsers.items = temp;
                    pageUsers.count = count;
                    pageUsers.pageIndex = 1;
                    pageUsers.pageSize = query.pageSize;
                }
                else if (query.pageSize * query.pageIndex > count)
                {
                    temp = (List<RecordVideo>)_context.RecordVideos.Skip(count - (count % query.pageSize)).Take((count % query.pageSize)).ToList();
                    pageUsers.items = temp;
                    pageUsers.count = count;
                    pageUsers.pageIndex = count / query.pageSize + 1;
                    pageUsers.pageSize = query.pageSize;
                }
                else
                {
                    temp = _context.RecordVideos.Skip((query.pageIndex - 1) * query.pageSize).Take(query.pageSize).ToList();
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
                resultState.success = true;
                resultState.message = "查询成功";
                resultState.value = pageUsers;
            }
            return new JsonResult(resultState);

        }

        private ResultState CheckCookie()
        {

            string s = _helper.GetCookie("token");
            if (s == null)
            {
                return new ResultState(false, "请登录", 0, null);
            }
            var a = s.Split(",");
            try
            {
                var admin = _context.Admins.Find(int.Parse(a[0]));
                if (admin != null)
                {
                    return new ResultState(true, "验证成功", 1, null);
                }
                else
                {
                    return new ResultState(false, "无效cookie", 0, null);

                }
            }
            catch (Exception e)
            {
                return new ResultState(false, "无效cookie", 0, null);
            }


        }



    }
}