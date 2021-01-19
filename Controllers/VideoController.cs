using live.Models;
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
        private string _targetFilePath;

        public VideoController(LiveMultiContext context)
        {
            _context = context;
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




        [HttpPost("uploadFile")]
        public JsonResult uploadFile(IFormFileCollection file,[FromForm]int anchor_id, [FromForm] string category)
        {
            ResultState resultState = new ResultState();
            List<RecordVideo> fileList = new List<RecordVideo>();
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
                    string filePath = "/usr/local/nginx/html/mp4";
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
                    recordVideo.url = "http://218.244.154.17/" + recordVideo.keyword;
                    recordVideo.createTime = DateTime.Now.ToString();
                    recordVideo.status = 0;  //上传时默认未审核
                    recordVideo.anchor_id = anchor_id;
                    recordVideo.category = category;
                    //recordVideo.parentId = 0;
                    //recordVideo.isDeal = 0;
                    fileList.Add(recordVideo);


                }
            }
            catch(Exception e)
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





    }
}
