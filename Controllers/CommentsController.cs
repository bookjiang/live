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

        // 获取数据库所有评论（供测试开发用）
        [HttpGet("GetAllComments")]
        public JsonResult GetAllComments()
        {

            ResultState resultState = new ResultState();
            var comments = _context.Comments.ToArray();
            if (comments == null)
            {
                resultState.success = false;
                resultState.message = "未获取评论列表";
                return new JsonResult(resultState);
            }

            
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

        //判断评论是否包含敏感词
        private bool ContainSensitiveWord(Comment comment)
        {
            var test = new List<string> { "1", "2", "3"};
            foreach (string s in test)
            {
                if (comment.content.Contains(s))
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

        // 查询评论（待扩展分页操作）
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
