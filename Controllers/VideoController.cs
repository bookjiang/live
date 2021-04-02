﻿using live.Models;
using live.utils;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace live.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("any")]
    public class VideoController : ControllerBase
    {
        private LiveMultiContext _context;
        private readonly ICookieHelper _helper;
        //private string _targetFilePath;

        public VideoController(LiveMultiContext context,ICookieHelper helper)
        {
            _context = context;
            _helper = helper;
        }

        /// <summary>
        /// 流式文件上传
        /// </summary>
        /// <returns></returns>
        //[HttpPost("uploadFile")]
        //public async Task<JsonResult> uploadFile()
        //{
        //    ResultState resultState = new ResultState();
        //    //获取boundary
        //    var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(Request.ContentType).Boundary).Value;
        //    //得到reader
        //    var reader = new MultipartReader(boundary, HttpContext.Request.Body);
        //    //{ BodyLengthLimit = 2000 };//
        //    var section = await  reader.ReadNextSectionAsync();
        //    var filename = HttpContext.Request.Form.FirstOrDefault();

        //    //读取section
        //    while (section != null)
        //    {
        //        var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);
        //        if (hasContentDispositionHeader)
        //        {
        //            var trustedFileNameForFileStorage = Path.GetRandomFileName();
        //            await WriteFileAsync(section.Body, Path.Combine(_targetFilePath, trustedFileNameForFileStorage));
        //        }
        //        section = await reader.ReadNextSectionAsync();
        //    }
        //    return new JsonResult(resultState);
        //}

        //private static async Task<int> WriteFileAsync(System.IO.Stream stream, string path)
        //{
        //    const int FILE_WRITE_SIZE = 84975;//写出缓冲区大小
        //    int writeCount = 0;
        //    using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write, FILE_WRITE_SIZE, true))
        //    {
        //        byte[] byteArr = new byte[FILE_WRITE_SIZE];
        //        int readCount = 0;
        //        while ((readCount = await stream.ReadAsync(byteArr, 0, byteArr.Length)) > 0)
        //        {
        //            await fileStream.WriteAsync(byteArr, 0, readCount);
        //            writeCount += readCount;
        //        }
        //    }
        //    return writeCount;
        //}



        /// <summary>
        /// 文件上传
        /// </summary>
        [HttpPost("uploadFile")]
        public JsonResult uploadFile(IFormFileCollection file,[FromForm]int anchor_id, [FromForm] string category)
        {

            ResultState resultState = CheckCookie();
            if (resultState.code !=0)
            {

                //ResultState resultState = new ResultState();
                List<RecordVideo> fileList = new List<RecordVideo>();
                if (file.Count == 0)
                    return new JsonResult(new ResultState(false, "未上传文件", 0, null));

                //IFormFileCollection file = items.GetFile
                try
                {
                    foreach (var item in file)
                    {
                        RecordVideo recordVideo = new RecordVideo();
                        recordVideo.keyword = item.FileName;
                        recordVideo.size = item.Length / 1024;
                        recordVideo.type = recordVideo.keyword.Substring(recordVideo.keyword.LastIndexOf('.') + 1).ToUpper();
                        // recordVideo.guid = Guid.NewGuid().ToString();

                        //string filePath = hostEnv.ContentRootPath + "/wwwroot/upload/" + recordVideo.guid + @"/";
                        string filePath = "/usr/local/nginx/html/mp4/";
                        //string filePath = "E:\\";

                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }

                        using (FileStream fs = System.IO.File.Create(filePath + recordVideo.keyword))
                        {
                            // 复制文件
                            item.CopyTo(fs);
                            // 清空缓冲区数据
                            fs.Flush();
                        }
                        recordVideo.path = filePath.Replace("\\", "/") + recordVideo.keyword;
                        //recordVideo.url = "http://" + _accessor.HttpContext.Request.Host + "/upload/" + recordVideo.guid + "/" + recordVideo.filename;
                        recordVideo.url = "http://1.15.84.163/" + recordVideo.keyword;
                        recordVideo.createTime = DateTime.Now.ToString();
                        recordVideo.status = 0;  //上传时默认未审核
                        recordVideo.anchor_id = anchor_id;
                        recordVideo.category = category;
                        //recordVideo.parentId = 0;
                        //recordVideo.isDeal = 0;
                        fileList.Add(recordVideo);


                    }
                }
                catch (Exception e)
                {
                    resultState.value = fileList;
                    resultState.success = false;
                    resultState.message = "插入失败";
                    return new JsonResult(resultState);
                }


                //将filelist写入数据库
                //TODO


                _context.RecordVideos.AddRange(fileList);
                _context.SaveChanges();



                //FileBus fileBus = new FileBus(_context);
                //fileBus.AddList(fileList);
                resultState.value = fileList;
                resultState.success = true;
                resultState.message = "插入成功";
                return new JsonResult(resultState);

            }
            return new JsonResult(resultState);


        }

        /// <summary>
        /// 通过id删除视频
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteFile/{id}")]
        public JsonResult DeleteFile(int id)
        {

            ResultState resultState = CheckCookie();
            if (resultState.code !=0)
            {
                //只能用户发的删除自己的

                var video = _context.RecordVideos.FirstOrDefault(r => r.id == id);
                if(resultState.message=="管理员登录"||video.anchor_id==resultState.code)
                {
                    if (video == null)
                    {
                        return new JsonResult(new ResultState(false, "删除失败", 0, video));
                    }
                    _context.RecordVideos.Remove(video);
                    _context.SaveChanges();


                    return new JsonResult(new ResultState(true, "删除成功", 1, video));
                }
                else
                {
                    return new JsonResult(new ResultState(false, "非管理员和作者，无法删除", 0, video));
                }




            }
            return new JsonResult(resultState);

           

        }

        /// <summary>
        /// 获取视频列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("videoList")]
        public JsonResult VideoList([FromBody] QueryParameters query)
        {


            int count = _context.RecordVideos.Where(x => x.status == 1).Count();
            int pageSize1 = query.pageSize;
            List<RecordVideo> temp = new List<RecordVideo>();
            PageInfoList pageUsers = new PageInfoList();
            if (query.pageIndex <= 0)
            {
                temp = (List<RecordVideo>)_context.RecordVideos.Where(x=>x.status==1).Take(query.pageSize).ToList();
                //temp = (List<RecordVideo>)_context.RecordVideos.Take(query.pageSize).ToList();
                pageUsers.items = temp;
                pageUsers.count = count;
                pageUsers.pageIndex = 1;
                pageUsers.pageSize = query.pageSize;
            }
            else if (query.pageSize * query.pageIndex > count)
            {
                temp = (List<RecordVideo>)_context.RecordVideos.Where(x => x.status == 1).Skip(count - (count % query.pageSize)).Take((count % query.pageSize)).ToList();
                pageUsers.items = temp;
                pageUsers.count = count;
                pageUsers.pageIndex = count / query.pageSize+1;
                pageUsers.pageSize = query.pageSize;
            }
            else
            {
                temp = _context.RecordVideos.Where(x => x.status == 1).Skip((query.pageIndex - 1) * query.pageSize).Take(query.pageSize).ToList();
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



        /// <summary>
        /// 管理员获取视频列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("videoListByAdmin")]
        public JsonResult VideoListByAdmin([FromBody] QueryParameters query)
        {


            int count = _context.RecordVideos.Count();
            int pageSize1 = query.pageSize;
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
            ResultState resultState = new ResultState();
            resultState.success = true;
            resultState.message = "查询成功";
            resultState.value = pageUsers;
            return new JsonResult(resultState);

        }



        /// <summary>
        /// 通过id查询video
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<RecordVideo> Get(int id)
        {
            return _context.RecordVideos.Find(id);
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
                var user = _context.Users.Find(int.Parse(a[0]));
                var admin = _context.Admins.Find(int.Parse(a[0]));

                if (user != null)
                {
                    return new ResultState(true, "用户登录", user.id, null);//code=2表示用户登录进来
                }
                else if(admin != null)
                {
                    return new ResultState(true, "管理员登录", admin.id, null);//code=3表示管理员登录进来
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

    }


}




