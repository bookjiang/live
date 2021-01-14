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
        
        //删除评论
        [HttpPost("DeleteComment")]
        public JsonResult DeleteComment(Comment comment)
        {
            ResultState resultState = new ResultState();
            var newComment = _context.Comments.Find(comment.id);
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
            resultState.value = comment;
            return new JsonResult(resultState);

        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.id == id);
        }

        //添加评论
        [HttpPost("AddComment")]
        public JsonResult AddComment(Comment comment)
        {
            ResultState resultState = new ResultState();
            //    bool IsIncludeSensitiveWord = false;    预留
            
            if (comment.content.Contains("敏感词"))       //判断敏感词
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

        //通过视频这个对象查询该视频的所有评论
        [HttpGet("GetComments")]
        public JsonResult GetComments(RecordVideo recordVideo)
        {

            ResultState resultState = new ResultState();
            var comments = from c in _context.Comments select c;
            if (comments == null)
            {
                resultState.success = false;
                resultState.message = "未获取评论列表";
                resultState.value = recordVideo;
                return new JsonResult(resultState);
            }

            comments = comments.Where(c => c.video_id == recordVideo.id);
            resultState.success = true;
            resultState.code = 1;
            resultState.message = "获取成功";
            resultState.value = comments.ToArray();
            return new JsonResult(resultState);
        }
    }
}
