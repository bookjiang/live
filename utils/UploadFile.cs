using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace live.utils
{
    public class UploadFile
    {
        public List<string> uploadFile(IFormFileCollection file)
        {
            //ResultState resultState = new ResultState();
            List<string> fileList = new List<String>();
            if (file.Count == 0)
            {
                fileList.Add("null");
                return fileList;
            }

            //IFormFileCollection file = items.GetFile
            try
            {
                foreach (var item in file)
                {
                    //RecordVideo recordVideo = new RecordVideo();
                    string keyword = item.FileName;
                    //recordVideo.size = item.Length / 1024;
                    //recordVideo.type = recordVideo.keyword.Substring(recordVideo.keyword.LastIndexOf('.') + 1).ToUpper();
                    // recordVideo.guid = Guid.NewGuid().ToString();

                    //string filePath = hostEnv.ContentRootPath + "/wwwroot/upload/" + recordVideo.guid + @"/";
                    string filePath = "/usr/local/nginx/html/images/";
                    //string filePath = "E:\\";

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
                    String url = "http://218.244.154.17/images/" + keyword;
                    //recordVideo.createTime = DateTime.Now.ToString();
                    //recordVideo.status = 0;  //上传时默认未审核
                    //recordVideo.anchor_id = anchor_id;
                    //recordVideo.category = category;
                    //recordVideo.parentId = 0;
                    //recordVideo.isDeal = 0;
                    fileList.Add(url);


                }
            }
            catch (Exception e)
            {
                fileList.Add("error");
                return fileList;
            }
            return fileList;


        }
       
    }
}
