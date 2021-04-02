using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace live.utils
{
    public static class InputFile
    {
        public static List<string> inputFile(IFormFileCollection file)
        {
            //ResultState resultState = new ResultState();
            //List<string> fileList = new List<String>();
            List<string> fileList = new List<String>();

            if (file.Count == 0)
            {
                fileList[0] = "null";
                return fileList;
            }
            //string keyword1 = file[0].FileName;
            //string filePath = "E:\\音乐项目文件夹\\" + Path.GetFileNameWithoutExtension(keyword1) + "\\";
            string filePath = "/usr/local/nginx/html/images/";

            //Console.WriteLine("在foreach外面");
            //IFormFileCollection file = items.GetFile
            try
            {
                //Console.WriteLine("在foreach上面一行");
                foreach (var item in file)
                {
                    //RecordVideo recordVideo = new RecordVideo();
                    string keyword = item.FileName;
                    //recordVideo.size = item.Length / 1024;
                    //recordVideo.type = recordVideo.keyword.Substring(recordVideo.keyword.LastIndexOf('.') + 1).ToUpper();
                    // recordVideo.guid = Guid.NewGuid().ToString();
                    //Console.WriteLine("在foreach里面");
                    //string filePath = hostEnv.ContentRootPath + "/wwwroot/upload/" + recordVideo.guid + @"/";
                    //string filePath = "/usr/local/nginx/html/mp4/";
                    

                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    using (FileStream fs = System.IO.File.Create(filePath + keyword))
                    {
                        // 复制文件
                        item.CopyTo(fs);
                        // 清空缓冲区数据
                        fs.Flush();
                    }
                    //String path = filePath.Replace("\\", "/") + s;
                    //recordVideo.url = "http://" + _accessor.HttpContext.Request.Host + "/upload/" + recordVideo.guid + "/" + recordVideo.filename;
                    String url = "http://47.105.112.118/images/" + keyword;
                    //recordVideo.createTime = DateTime.Now.ToString();
                    //recordVideo.status = 0;  //上传时默认未审核
                    //recordVideo.anchor_id = anchor_id;
                    //recordVideo.category = category;
                    //recordVideo.parentId = 0;
                    //recordVideo.isDeal = 0;
                    //if (url.Contains("mp3") || url.Contains("MP3"))
                    //{
                    //    fileList[0] = url;
                    //}
                    //if (url.Contains("jpg") || url.Contains("JPG") || url.Contains("png") || url.Contains("PNG"))
                    //{
                    //    fileList[1] = url;
                    //}

                    fileList.Add(url);

                }
            }
            catch (Exception e)
            {
                
                fileList[0] = "error";
                return fileList;
            }
            return fileList;


        }

    }
}
