using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using live.Models;
using live.utils;


namespace live.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("any")]
    public class MusicSongListController : ControllerBase
    {
        private LiveMultiContext _context;
        private ICookieHelper _helper;
        private UploadFile _upload = new UploadFile();

        public MusicSongListController(LiveMultiContext context, ICookieHelper helper)
        {
            _context = context;
            _helper = helper;

        }



        /// <summary>
        ///核验cookie，返回结果中包含身份，resultState.code:表示user.id
        /// </summary>
        /// <returns></returns>
        private ResultState CheckCookie()
        {

            string s = _helper.GetCookie("token");
            //DateTime dateTime = _helper
            //尝试获取cookie的时间属性没有成功。

            if (s == null)
            {
                return new ResultState(false, "请登录", 0, null);
            }
            var a = s.Split(",");
            try
            {
                var user = _context.MusicUsers.Find(int.Parse(a[0]));
                if (user != null)
                {
                    if (user.role == 0)//表示管理员
                    {
                        return new ResultState(true, "管理员", user.id, null);
                    }
                    else
                    {
                        return new ResultState(true, "用户", user.id, null);
                    }

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

        /// <summary>
        /// 分页获取所有歌单列表，用户和管理员调用一个接口，后台可以判断角色
        /// </summary>
        /// <returns></returns>
        [HttpPost("getSongList")]
        public JsonResult getSongList([FromBody] QueryParameters query)
        {
            ResultState resultState = CheckCookie();
            if (resultState.message == "管理员")  //管理员登录，可以获取所有歌单
            {
                int count = _context.MusicSongLists.Count();
                int pageSize1 = query.pageSize;
                List<MusicSongList> temp = new List<MusicSongList>();
                PageInfoList pageSongList = new PageInfoList();
                if (query.pageIndex <= 0)
                {
                    temp = (List<MusicSongList>)_context.MusicSongLists.Take(query.pageSize).ToList();
                    pageSongList.items = temp;
                    pageSongList.count = count;
                    pageSongList.pageIndex = 1;
                    pageSongList.pageSize = query.pageSize;
                }
                else if (query.pageSize * query.pageIndex > count)
                {
                    temp = (List<MusicSongList>)_context.MusicSongLists.Skip(count - (count % query.pageSize)).Take((count % query.pageSize)).ToList();
                    pageSongList.items = temp;
                    pageSongList.count = count;
                    pageSongList.pageIndex = count / query.pageSize + 1;
                    pageSongList.pageSize = query.pageSize;
                }
                else
                {
                    temp = _context.MusicSongLists.Skip((query.pageIndex - 1) * query.pageSize).Take(query.pageSize).ToList();
                    pageSongList.items = temp;
                    pageSongList.count = count;
                    pageSongList.pageIndex = query.pageIndex;
                    pageSongList.pageSize = query.pageSize;
                }

                resultState.success = true;
                resultState.message = "查询成功";
                resultState.value = pageSongList;
                return new JsonResult(resultState);

            }
            else if (resultState.message == "用户")//用户登录，只能获取公有歌单
            {
                int count = _context.MusicSongLists.Where(x => x.status == 1).Count();
                int pageSize1 = query.pageSize;
                List<MusicSongList> temp = new List<MusicSongList>();
                PageInfoList pageSongList = new PageInfoList();
                if (query.pageIndex <= 0)
                {
                    temp = (List<MusicSongList>)_context.MusicSongLists.Where(x => x.status == 1).Take(query.pageSize).ToList();
                    pageSongList.items = temp;
                    pageSongList.count = count;
                    pageSongList.pageIndex = 1;
                    pageSongList.pageSize = query.pageSize;
                }
                else if (query.pageSize * query.pageIndex > count)
                {
                    temp = (List<MusicSongList>)_context.MusicSongLists.Where(x => x.status == 1).Skip(count - (count % query.pageSize)).Take((count % query.pageSize)).ToList();
                    pageSongList.items = temp;
                    pageSongList.count = count;
                    pageSongList.pageIndex = count / query.pageSize + 1;
                    pageSongList.pageSize = query.pageSize;
                }
                else
                {
                    temp = _context.MusicSongLists.Where(x => x.status == 1).Skip((query.pageIndex - 1) * query.pageSize).Take(query.pageSize).ToList();
                    pageSongList.items = temp;
                    pageSongList.count = count;
                    pageSongList.pageIndex = query.pageIndex;
                    pageSongList.pageSize = query.pageSize;
                }

                resultState.success = true;
                resultState.message = "查询成功";
                resultState.value = pageSongList;
                return new JsonResult(resultState);
            }
            return new JsonResult(resultState);


        }


        /// <summary>
        /// 通过用户id分页获取所有歌单列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("getSongListByUserId/{id}")]
        public JsonResult getSongListByUserId([FromBody] QueryParameters query, int id)
        {

            ResultState resultState = CheckCookie();
            if (resultState.message == "管理员" || resultState.code == id)
            {
                int count = _context.MusicSongLists.Where(x => x.user_id == id).Count();
                int pageSize1 = query.pageSize;
                List<MusicSongList> temp = new List<MusicSongList>();
                PageInfoList pageSongList = new PageInfoList();
                if (query.pageIndex <= 0)
                {
                    temp = (List<MusicSongList>)_context.MusicSongLists.Where(x => x.user_id == id).Take(query.pageSize).ToList();
                    pageSongList.items = temp;
                    pageSongList.count = count;
                    pageSongList.pageIndex = 1;
                    pageSongList.pageSize = query.pageSize;
                }
                else if (query.pageSize * query.pageIndex > count)
                {
                    temp = (List<MusicSongList>)_context.MusicSongLists.Where(x => x.user_id == id).Skip(count - (count % query.pageSize)).Take((count % query.pageSize)).ToList();
                    pageSongList.items = temp;
                    pageSongList.count = count;
                    pageSongList.pageIndex = count / query.pageSize + 1;
                    pageSongList.pageSize = query.pageSize;
                }
                else
                {
                    temp = _context.MusicSongLists.Where(x => x.user_id == id).Skip((query.pageIndex - 1) * query.pageSize).Take(query.pageSize).ToList();
                    pageSongList.items = temp;
                    pageSongList.count = count;
                    pageSongList.pageIndex = query.pageIndex;
                    pageSongList.pageSize = query.pageSize;
                }
                //ResultState resultState = new ResultState();
                resultState.success = true;
                resultState.message = "查询成功";
                resultState.value = pageSongList;
                return new JsonResult(resultState);
            }
            else if (resultState.code != 0)//说明用户非法操作
            {
                return new JsonResult(new ResultState(false, "用户操作非法，不能通过id获取其他用户歌单", 1, null));
            }
            return new JsonResult(resultState);


        }


        /// <summary>
        /// 增加歌单，表单提交
        /// </summary>
        /// <param name="file"></param>
        /// <param name="name"></param>
        /// <param name="describe"></param>
        /// <param name="user_id"></param>
        /// <returns></returns>
        [HttpPost("addSongList")]
        public JsonResult addSongList(IFormFileCollection file, [FromForm] string name, [FromForm] string describe, [FromForm] string user_id_1, [FromForm] string status_1 = "1")
        {
            int user_id = int.Parse(user_id_1);
            int status = int.Parse(status_1);

            MusicSongList musicSongList = new MusicSongList();
            musicSongList.name = name;
            musicSongList.describe = describe;
            musicSongList.user_id = user_id;
            musicSongList.status = status;//默认公有
            ResultState resultState = CheckCookie();
            if (resultState.message == "管理员" || resultState.code == musicSongList.user_id)
            {
                MusicSongList musicSongList1 = _context.MusicSongLists.Where(x => (x.user_id == musicSongList.user_id && x.name == musicSongList.name)).FirstOrDefault();
                if (musicSongList1 != null)
                {
                    return new JsonResult(new ResultState(false, "歌单已存在", 0, musicSongList));

                }
                List<String> urls = _upload.uploadFile(file);
                if (urls[0] == "null")
                {
                    musicSongList.cover_url = "null";
                }
                else if (urls[0] == "error")
                {
                    return new JsonResult(new ResultState(false, "封面上传失败", 0, musicSongList));

                }
                else
                {
                    musicSongList.cover_url = urls[0];
                }



                _context.MusicSongLists.Add(musicSongList);
                _context.SaveChanges();
                return new JsonResult(new ResultState(true, "添加成功", 1, musicSongList));
            }
            else if (resultState.code != 0)//说明用户非法操作
            {
                return new JsonResult(new ResultState(false, "用户操作非法，不能给别的用户添加歌单", 1, musicSongList));
            }

            return new JsonResult(resultState);


        }



        /// <summary>
        /// 删除歌单，用户和管理员调用一个接口，后台可以判断角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("deleteSongList/{id}")]
        public JsonResult deleteSongList(int id)
        {
            ResultState resultState = CheckCookie();
            MusicSongList musicSongList = _context.MusicSongLists.Find(id);
            if (resultState.message == "管理员" || resultState.code == musicSongList.user_id)
            {

                List<MusicSongAndSongList> list = _context.MusicSongAndSongLists.Where(x => x.song_list_id == musicSongList.id).ToList();
                _context.MusicSongLists.Remove(musicSongList);
                _context.MusicSongAndSongLists.RemoveRange(list);//同步删除关联表中的数据
                _context.SaveChanges();

            }
            else if (resultState.code != 0)//说明用户非法操作
            {
                return new JsonResult(new ResultState(false, "用户操作非法，不能删除别的用户歌单", 1, musicSongList));
            }

            return new JsonResult(resultState);
        }





        /// <summary>
        /// 修改歌单，操作者没有修改的属性请赋原值传递给后端
        /// </summary>
        /// <param name="file"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="describe"></param>
        /// <param name="user_id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPut("updateSongList")]
        public JsonResult updateSongList(IFormFileCollection file, [FromForm] int id, [FromForm] string name, [FromForm] string describe, [FromForm] int user_id, [FromForm] int status)
        {

            MusicSongList musicSongList = _context.MusicSongLists.Find(id);
            if (musicSongList == null)
            {
                return new JsonResult(new ResultState(false, "歌单不存在", 0, musicSongList));

            }
            musicSongList.name = name;
            musicSongList.describe = describe;
            musicSongList.user_id = user_id;
            musicSongList.status = status;
            ResultState resultState = CheckCookie();
            if (resultState.message == "管理员" || resultState.code == musicSongList.user_id)
            {
                //MusicSongList musicSongList1 = _context.MusicSongLists.Where(x => (x.user_id == musicSongList.user_id && x.name == musicSongList.name)).FirstOrDefault();

                List<String> urls = _upload.uploadFile(file);
                if (urls[0] == "null")
                {
                    musicSongList.cover_url = "null";
                }
                else if (urls[0] == "error")
                {
                    return new JsonResult(new ResultState(false, "封面上传失败", 0, musicSongList));

                }
                else
                {
                    musicSongList.cover_url = urls[0];
                }



                _context.MusicSongLists.Update(musicSongList);
                _context.SaveChanges();
                return new JsonResult(new ResultState(true, "添加成功", 1, musicSongList));
            }
            else if (resultState.code != 0)//说明用户非法操作
            {
                return new JsonResult(new ResultState(false, "用户操作非法，不能给别的用户添加歌单", 1, musicSongList));
            }

            return new JsonResult(resultState);


        }







        /// <summary>
        /// 通过歌单id分页索取其列表中歌曲详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("getSongs/{id}")]
        public JsonResult getSongs([FromBody] QueryParameters query, int id)
        {
            ResultState resultState = CheckCookie();
            if (resultState.code != 0)
            {

                int count = _context
                    .MusicSongs
                    .Join(_context.MusicSongAndSongLists.Where(x => x.song_list_id == id), musicSongs => musicSongs.id, musicSongAndSongList => musicSongAndSongList.song_id, (musicSongs, musicSongAndSongList) => new MusicSong(musicSongs.id, musicSongs.name, musicSongs.album, musicSongs.artists, musicSongs.lyric, musicSongs.play_url, musicSongs.cover_post))
                    .Count();
                int pageSize1 = query.pageSize;
                List<MusicSong> temp = new List<MusicSong>();
                PageInfoList pageSongList = new PageInfoList();
                if (query.pageIndex <= 0)
                {
                    //temp = (List<MusicSong>)_context.MusicSongLists.Where(x => x.user_id == id).Take(query.pageSize).ToList();
                    temp = (List<MusicSong>)_context
                    .MusicSongs
                    .Join(_context.MusicSongAndSongLists.Where(x => x.song_list_id == id), musicSongs => musicSongs.id, musicSongAndSongList => musicSongAndSongList.song_id, (musicSongs, musicSongAndSongList) => new MusicSong(musicSongs.id, musicSongs.name, musicSongs.album, musicSongs.artists, musicSongs.lyric, musicSongs.play_url, musicSongs.cover_post))
                    .Take(query.pageSize)
                    .ToList();
                    pageSongList.items = temp;
                    pageSongList.count = count;
                    pageSongList.pageIndex = 1;
                    pageSongList.pageSize = query.pageSize;
                }
                else if (query.pageSize * query.pageIndex > count)
                {
                    //temp = (List<MusicSongList>)_context.MusicSongLists.Where(x => x.user_id == id).Skip(count - (count % query.pageSize)).Take((count % query.pageSize)).ToList();
                    temp = (List<MusicSong>)_context
                    .MusicSongs
                    .Join(_context.MusicSongAndSongLists.Where(x => x.song_list_id == id), musicSongs => musicSongs.id, musicSongAndSongList => musicSongAndSongList.song_id, (musicSongs, musicSongAndSongList) => new MusicSong(musicSongs.id, musicSongs.name, musicSongs.album, musicSongs.artists, musicSongs.lyric, musicSongs.play_url, musicSongs.cover_post))
                    .Skip(count - (count % query.pageSize))
                    .Take((count % query.pageSize))
                    .ToList();

                    pageSongList.items = temp;
                    pageSongList.count = count;
                    pageSongList.pageIndex = count / query.pageSize + 1;
                    pageSongList.pageSize = query.pageSize;
                }
                else
                {
                    //temp = _context.MusicSongLists.Where(x => x.user_id == id).Skip((query.pageIndex - 1) * query.pageSize).Take(query.pageSize).ToList();
                    temp = (List<MusicSong>)_context
                        .MusicSongs
                        .Join(_context.MusicSongAndSongLists.Where(x => x.song_list_id == id), musicSongs => musicSongs.id, musicSongAndSongList => musicSongAndSongList.song_id, (musicSongs, musicSongAndSongList) => new MusicSong(musicSongs.id, musicSongs.name, musicSongs.album, musicSongs.artists, musicSongs.lyric, musicSongs.play_url, musicSongs.cover_post))
                        .Skip((query.pageIndex - 1) * query.pageSize)
                        .Take(query.pageSize)
                        .ToList();


                    pageSongList.items = temp;
                    pageSongList.count = count;
                    pageSongList.pageIndex = query.pageIndex;
                    pageSongList.pageSize = query.pageSize;
                }
                //ResultState resultState = new ResultState();
                resultState.success = true;
                resultState.message = "查询成功";
                resultState.value = pageSongList;
                return new JsonResult(resultState);




                //List<MusicSong> musicSongs1 = _context
                //    .MusicSongs
                //    .Join(_context.MusicSongAndSongLists.Where(x => x.song_list_id == id), musicSongs => musicSongs.id, musicSongAndSongList => musicSongAndSongList.song_id, (musicSongs, musicSongAndSongList) => new MusicSong(musicSongs.id, musicSongs.name,musicSongs.album,musicSongs.artists,musicSongs.lyric,musicSongs.play_url,musicSongs.cover_post))
                //    .ToList();
                //    return new JsonResult(new ResultState(true, "查询成功", 1, musicSongs1));

            }
            return new JsonResult(resultState);
        }


        /// <summary>
        /// 歌单添加歌曲
        /// </summary>
        /// <param name="song_id_1"></param>
        /// <param name="song_list_id_1"></param>
        /// <returns></returns>
        [HttpPost("addSongAndSongList")]
        public JsonResult addSongAndSongList([FromForm] string song_id_1, [FromForm] string song_list_id_1)
        {
            int song_id = int.Parse(song_id_1);
            int song_list_id = int.Parse(song_list_id_1);


            ResultState resultState = CheckCookie();
            if (resultState.code != 0)
            {
                MusicSongAndSongList musicSongAndSongList_1 = _context.MusicSongAndSongLists.Where(x => x.song_id == song_id && x.song_list_id == song_list_id).FirstOrDefault();
                if(musicSongAndSongList_1!=null)
                {
                    return new JsonResult(new ResultState(false, "添加条目已存在", 0, musicSongAndSongList_1));

                }
                MusicSongAndSongList musicSongAndSongList = new MusicSongAndSongList();
                musicSongAndSongList.song_id = song_id;
                musicSongAndSongList.song_list_id = song_list_id;
                _context.MusicSongAndSongLists.Add(musicSongAndSongList);
                _context.SaveChanges();
            }
            return new JsonResult(resultState);
        }



        /// <summary>
        /// 根据歌单id删除歌曲id
        /// </summary>
        /// <param name="song_id_1"></param>
        /// <param name="song_list_id_1"></param>
        /// <returns></returns>
        [HttpDelete("DeleteSongFromSongList")]
        public JsonResult DeleteSongFromSongList([FromForm] string song_id_1, [FromForm] string song_list_id_1)
        {

            int song_id = int.Parse(song_id_1);
            int song_list_id = int.Parse(song_list_id_1);


            ResultState resultState = CheckCookie();
            if (resultState.code != 0)
            {
                MusicSongAndSongList musicSongAndSongList_1 = _context.MusicSongAndSongLists.Where(x => x.song_id == song_id && x.song_list_id == song_list_id).FirstOrDefault();
                if (musicSongAndSongList_1 == null)
                {
                    return new JsonResult(new ResultState(false, "添加条目不存在", 0, musicSongAndSongList_1));

                }

                _context.MusicSongAndSongLists.Remove(musicSongAndSongList_1);
                _context.SaveChanges();
            }
            return new JsonResult(resultState);
        }


        /// <summary>
        /// 通过歌单id获取歌单详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getSongListById/{id}")]
        public JsonResult GetSongListById(int id)
        {

            MusicSongList musicSongList = _context.MusicSongLists.Find(id);
            if (musicSongList == null)
            {
                return new JsonResult(new ResultState(false, "歌单不存在", 0, null));
            }
            else
            {
                return new JsonResult(new ResultState(true, "查询成功", 0, musicSongList));
            }

        }

    }
}
