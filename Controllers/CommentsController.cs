using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using live.Models;

namespace live.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly LiveMultiContext _context;

        public CommentsController(LiveMultiContext context)
        {
            _context = context;
        }

        // 获取数据库所有评论,无需参数（供测试开发用）
        [HttpGet("GetAllComments")]
        public JsonResult GetAllComments()
        {

            ResultState resultState = new ResultState();
            var comments = _context.Comments.ToList();
            
            resultState.success = true;
            resultState.code = 1;
            resultState.message = "获取成功";
            resultState.value = comments;
            return new JsonResult(resultState);
        }



        // 删除某条评论（管理员权限）
        [HttpPost("DeleteComment")]
        public JsonResult DeleteComment(Comment comment)
        {
            ResultState resultState = new ResultState();
            var newComment = _context.Comments.Where(c => c.id == comment.id).FirstOrDefault();
            if (newComment == null)    //数据库中未找到该条评论，删除失败
            {
                resultState.success = false;
                resultState.code = 0;
                resultState.message = "删除评论失败！";
                resultState.value = comment;
                return new JsonResult(resultState);
            }
            
            _context.Comments.Remove(newComment);          //数据库找到该条评论，执行删除操作
            _context.SaveChanges();

            resultState.success = true;
            resultState.code = 1;
            resultState.message = "删除成功";
            resultState.value = newComment;
            return new JsonResult(resultState);

        }

        //判断评论是否包含敏感词
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

        // 添加评论
        [HttpPost("AddComment")]
        public JsonResult AddComment(Comment comment)
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

        // 查询评论(分页)
        [HttpPost("GetComments")]
        public JsonResult GetComments(VideoAndPage videoAndPage)
        {
            //拆包两个对象
            var recordVideo = videoAndPage.RecordVideo;
            var query = videoAndPage.QueryParameters;

            ResultState resultState = new ResultState();

            var comments = from c in _context.Comments select c;
            comments = comments.Where(c => c.video_id == recordVideo.id);

            var count = comments.Count();

            int pageSize1 = query.pageSize;
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
    }
}
