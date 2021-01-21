using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using live.Models;
using Microsoft.AspNetCore.Cors;
using live.utils;

namespace live.Controllers
{
    /// <summary>
    /// Comments控制器，包含一些对评论的一些基本操作
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("any")]
    public class CommentsController : ControllerBase
    {
        private readonly LiveMultiContext _context;
        private readonly ICookieHelper _helper;

        public CommentsController(LiveMultiContext context, ICookieHelper helper)
        {
            _context = context;
            _helper = helper;
        }

        /// <summary>
        /// 获取所有评论
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("GetAllComments")]
        public JsonResult GetAllComments([FromBody] QueryParameters query)
        {

            //ResultState resultState = new ResultState();
            //var comments = _context.Comments.ToList();
            var count = _context.Comments.Count();

            PageInfoList pageComments = new PageInfoList();
            List<Comment> temp;
            try
            {
                if (query.pageIndex <= 0)
                {
                    temp = _context.Comments.Take(query.pageSize).ToList();
                    //var pageComments =  new PageInfoList(count,1,query.pageSize,temp);
                    pageComments.items = temp;
                    pageComments.count = count;
                    pageComments.pageIndex = 1;
                    pageComments.pageSize = query.pageSize;
                }
                else if (query.pageSize * query.pageIndex > count)
                {
                    temp = _context.Comments.Skip(count - (count % query.pageSize)).Take((count % query.pageSize)).ToList();
                    pageComments.items = temp;
                    pageComments.count = count;
                    pageComments.pageIndex = count / query.pageSize;
                    pageComments.pageSize = query.pageSize;
                    //var pageComments = new PageInfoList(count, count / query.pageSize, query.pageSize,temp);
                }
                else
                {
                    temp = _context.Comments.Skip((query.pageIndex - 1) * query.pageSize).Take(query.pageSize).ToList();
                    pageComments.items = temp;
                    pageComments.count = count;
                    pageComments.pageIndex = query.pageIndex;
                    pageComments.pageSize = query.pageSize;
                    //var pageComments = new PageInfoList(count, query.pageIndex, query.pageSize, temp);
                }

            }
            catch
            {
                return new JsonResult(new ResultState(false, "获取评论失败", 0, null));
            }

            
            //resultState.success = true;
            //resultState.code = 1;
            //resultState.message = "获取所有评论成功";
            //resultState.value = pageComments;
            return new JsonResult(new ResultState(true, "获取评论成功",1, pageComments));
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="id">评论的id属性值</param>
        /// <returns></returns>
        [HttpDelete("DeleteComment/{id}")]
        public JsonResult DeleteComment(int id)
        {
            ResultState resultState = CheckCookie();
            //ResultState resultState = new ResultState();
            if (resultState.code == 0)
            {
                return new JsonResult(resultState);
            }
            var newComment = _context.Comments.Find(id);

            if (newComment == null)    //数据库中未找到该条评论，删除失败
            {
                return new JsonResult(new ResultState(false, "删除评论失败", 0, null));
            }
            try
            {
                _context.Comments.Remove(newComment);     //数据库找到该条评论，执行删除操作
                _context.SaveChanges();
            }
            catch
            {
                return new JsonResult(new ResultState(false, "删除评论失败", 0, null));
            }

            return new JsonResult(new ResultState(true, "删除评论成功", 1, newComment));

        }

        //判断评论是否包含敏感词(私有方法)
        private bool ContainSensitiveWord(Comment comment)
        {

            var keywords = from k in _context.KeyWords select k.keyword;

            foreach (string k in keywords)
            {
                if (comment.content.Contains(k))
                    return true;
            }

            return false;
        }


        //检查Cookies是否正常
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
                var user = _context.Users.Find(int.Parse(a[0]));
                if (user != null)
                {
                    return new ResultState(true, "验证成功", 1, null);
                }
                else
                {
                    return new ResultState(false, "无效cookie（未找到用户）", 0, null);

                }
            }
            catch
            {
                return new ResultState(false, "无效cookie(异常)", 0, null);
            }


        }

        /// <summary>
        /// 添加一条评论
        /// </summary>
        /// <param name="comment">需一条评论json对象作为参数,无需id属性(tips:依靠数据库自动生产id的值)</param>
        /// <returns></returns>
        [HttpPost("AddComment")]
        public JsonResult AddComment([FromBody] Comment comment)
        {
            ResultState resultState = CheckCookie();

            if (resultState.code == 0)
            {
                return new JsonResult(resultState);
            }
            try
            {
                if (ContainSensitiveWord(comment))       //判断敏感词
                {
                    
                    return new JsonResult(new ResultState(false, "评论包含非法词汇，请修改后添加！",0,comment));
                }
            }
            catch
            {
                return new JsonResult(new ResultState(false, "发生异常，请检查请求头参数", 0, null));

            }
            try
            {
                _context.Comments.Add(comment);
                _context.SaveChanges();

            }
            catch
            {
                return new JsonResult(new ResultState(false, "添加评论时发生异常，请检查求体参数", 0, null));

            }
            return new JsonResult(new ResultState(true, "添加评论成功", 1, comment));
        }


        /// <summary>
        /// 查询属于某视频评论
        /// </summary>
        /// <param name="videoAndPage">需要一个VideoAndPage类型的json对象,该类型由RecordVideo类型和QueryParameters类型组合为一个json对象</param>
        /// <returns></returns>
        [HttpPost("GetComments")]
        public JsonResult GetComments([FromBody] VideoAndPage videoAndPage)
        {
            ResultState resultState = CheckCookie();

            if (resultState.code == 0)
            {
                return new JsonResult(resultState);
            }
            if (videoAndPage == null || videoAndPage.RecordVideo == null || videoAndPage.QueryParameters == null)
            {
                return new JsonResult(new ResultState(false, "请求体参数异常", 0, null));
            }

            //拆包两个对象
            var recordVideo = videoAndPage.RecordVideo;
            var query = videoAndPage.QueryParameters;


            var count = _context.Comments.Where(c => c.video_id == recordVideo.id).Count();
            var comments = _context.Comments.Where(c => c.video_id == recordVideo.id).ToList();


            var temp = new List<Comment>();
            PageInfoList pageComments = new PageInfoList();
            //分页操作
            if (query.pageIndex <= 0)
            {
                temp = comments.Take(query.pageSize).ToList();
                pageComments.items = temp;
                pageComments.count = count;
                pageComments.pageIndex = 1;
                pageComments.pageSize = query.pageSize;
            }
            else if (query.pageSize * query.pageIndex > count)
            {
                temp = comments.Skip(count - (count % query.pageSize)).Take((count % query.pageSize)).ToList();
                pageComments.items = temp;
                pageComments.count = count;
                pageComments.pageIndex = count / query.pageSize;
                pageComments.pageSize = query.pageSize;
            }
            else
            {
                temp = comments.Skip((query.pageIndex - 1) * query.pageSize).Take(query.pageSize).ToList();
                pageComments.items = temp;
                pageComments.count = count;
                pageComments.pageIndex = query.pageIndex;
                pageComments.pageSize = query.pageSize;
            }

            resultState.success = true;
            resultState.code = 1;
            resultState.message = "获取评论列表成功";
            resultState.value = pageComments;

            return new JsonResult(resultState);
        }

        /// <summary>
        /// 通过id查询Comment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<Comment> Get(int id)
        {
            ResultState resultState = CheckCookie();

            if (resultState.code == 0)
            {
                return new JsonResult(resultState);            }

            return new JsonResult(_context.Comments.Find(id));
        }

    }
}
