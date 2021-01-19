using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using live.Models;
using Microsoft.AspNetCore.Cors;

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

        public CommentsController(LiveMultiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有评论
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("GetAllComments")]
        public JsonResult GetAllComments([FromBody] QueryParameters query)
        {
           
            ResultState resultState = new ResultState();
            var comments = _context.Comments.ToList();
            var count = comments.Count();

            PageInfoList pageComments = new PageInfoList();
            List<Comment> temp;
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
            resultState.message = "获取所有评论成功";
            resultState.value = pageComments;
            return new JsonResult(resultState);
        }



        
        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="id">评论的id属性值</param>
        /// <returns></returns>
        [HttpDelete("DeleteComment/{id}")]
        public JsonResult DeleteComment(int id)
        {
            ResultState resultState = new ResultState();
            var newComment = _context.Comments.Where(c => c.id == id).FirstOrDefault();
            if (newComment == null)    //数据库中未找到该条评论，删除失败
            {
                resultState.success = false;
                resultState.message = "删除评论失败！";
                resultState.value = newComment;
                return new JsonResult(resultState);
            }
            
            _context.Comments.Remove(newComment);          //数据库找到该条评论，执行删除操作
            _context.SaveChanges();

            resultState.success = true;
            resultState.code = 1;
            resultState.message = "删除评论成功";
            resultState.value = newComment;
            return new JsonResult(resultState);

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

        /// <summary>
        /// 添加一条评论
        /// </summary>
        /// <param name="comment">需一条评论json对象作为参数,无需id属性(tips:依靠数据库自动生产id的值)</param>
        /// <returns></returns>
        [HttpPost("AddComment")]
        public JsonResult AddComment([FromBody] Comment comment)
        {
            ResultState resultState = new ResultState();
           
            
            if (ContainSensitiveWord(comment)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            )       //判断敏感词
            {
                resultState.success = false;
                resultState.code = 0;
                resultState.message = "添加评论失败！";
                resultState.value = comment;
                return new JsonResult(resultState);
            }
             
            _context.Comments.Add(comment);
            _context.SaveChanges();

            resultState.success = true;
            resultState.code = 1;
            resultState.message = "添加成功";
            resultState.value = comment;
            return new JsonResult(resultState);
        }


        /// <summary>
        /// 查询属于某视频评论
        /// </summary>
        /// <param name="videoAndPage">需要一个VideoAndPage类型的json对象,该类型由RecordVideo类型和QueryParameters类型组合为一个json对象</param>
        /// <returns></returns>
        [HttpPost("GetComments")]
        public JsonResult GetComments([FromBody] VideoAndPage videoAndPage)
        {
            //拆包两个对象
            var recordVideo = videoAndPage.RecordVideo;
            var query = videoAndPage.QueryParameters;

            ResultState resultState = new ResultState();

            
            var comments =_context.Comments.Where(c => c.video_id == recordVideo.id).ToList();

            var count = comments.Count();

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
            return _context.Comments.Find(id);
        }

    }
}
