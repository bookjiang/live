using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using live.Models;
using live.utils;

namespace live.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicSongsController : ControllerBase
    {
        private readonly LiveMultiContext _context;
        private readonly ICookieHelper _helper;
        //private InputFile inputfile = new InputFile();

        public MusicSongsController(LiveMultiContext context, ICookieHelper helper)
        {
            _context = context;
            _helper = helper;
        }


        //// GET: api/MusicSongs/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<MusicSong>> GetMusicSong(int id)
        //{
        //    var musicSong = await _context.MusicSong.FindAsync(id);

        //    if (musicSong == null)
        //    {
        //        return NotFound();
        //    }

        //    return musicSong;
        //}

        //// PUT: api/MusicSongs/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutMusicSong(int id, MusicSong musicSong)
        //{
        //    if (id != musicSong.id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(musicSong).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!MusicSongExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/MusicSongs
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPost]
        //public async Task<ActionResult<MusicSong>> PostMusicSong(MusicSong musicSong)
        //{
        //    _context.MusicSong.Add(musicSong);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetMusicSong", new { id = musicSong.id }, musicSong);
        //}

        //// DELETE: api/MusicSongs/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<MusicSong>> DeleteMusicSong(int id)
        //{
        //    var musicSong = await _context.MusicSong.FindAsync(id);
        //    if (musicSong == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.MusicSong.Remove(musicSong);
        //    await _context.SaveChangesAsync();

        //    return musicSong;
        //}

        private bool MusicSongExists(int id)
        {
            return _context.MusicSongs.Any(e => e.id == id);
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
                var user = _context.MusicUsers.Find(int.Parse(a[0]));
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

        //// GET: api/MusicSongs
        /// <summary>
        /// 获取所有评论(可以在查询字符串中设置pageSize和pageIndex,不设置的话，默认pageIndex=1,pageSize=5)
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetMusicSong([FromQuery] int? pageIndex, [FromQuery] int? pageSize)
        {
            ResultState resultState = CheckCookie();

            if (resultState.code == 0)
            {
                return new JsonResult(resultState);
            }

            if (pageIndex == null)
            {
                pageIndex = 1;
            }
            if (pageSize == null || pageSize <= 0)
            {
                pageSize = 5;
            }

            //int pageSize = 5;

            var count = await _context.MusicSongs.CountAsync();

            List<MusicSong> list;
            try
            {
                if (pageIndex <= 0)
                {
                    pageIndex = 1;
                    list = await _context.MusicSongs.Take((int)pageSize).ToListAsync();

                }
                else if (pageSize * pageIndex > count)
                {
                    list = await _context.MusicSongs.Skip((int)(count - (count % pageSize))).Take((int)(count % pageSize)).ToListAsync();
                }
                else
                {
                    list = await _context.MusicSongs.Skip((int)((pageIndex - 1) * pageSize)).Take((int)pageSize).ToListAsync();
                }
                
            }
            catch
            {
                return new JsonResult(new ResultState(false, "获取歌曲列表失败", 0, null));
            }

            return new JsonResult(new ResultState(true, "获取歌曲列表成功", 1, list));
        }
 

        /// <summary>
        /// 添加歌曲
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        //[HttpPost("AddSong")]
        //public JsonResult AddSong([FromBody] MusicSong song)
        //{
        //    if(song != null)
        //    {
        //        try {
        //            var sameSong = _context.MusicSongs.
        //                Where(c => c.name == song.name && c.album == song.album && c.artists == song.artists).
        //                FirstOrDefault();

        //            if (sameSong == null)
        //            {
        //                _context.Add(song);
        //                _context.SaveChanges();

        //                var addsong = _context.MusicSongs.
        //                    Where(c => c.name == song.name && c.album == song.album && c.artists == song.artists).
        //                    FirstOrDefault();

        //                return new JsonResult(new ResultState(true, "添加成功", 1, addsong));

        //            }
        //            return new JsonResult(new ResultState(false, "添加失败，歌曲库中已存在改该歌曲", 0, sameSong));

        //        }
        //        catch
        //        {
        //            return new JsonResult(new ResultState(false, "发生异常，添加歌曲失败", 0, song));
        //        }
                
        //    }
        //    return new JsonResult(new ResultState(false, "添加失败，请检查请求体参数", 0, song));
        //}


        /// <summary>
        /// 通过id查询歌曲
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public JsonResult GetSong(int id)
        {
            ResultState resultState = CheckCookie();

            if (resultState.code == 0)
            {
                return new JsonResult(resultState);
            }
            var song = _context.MusicSongs.Find(id);
            if(song == null)
            {
                return new JsonResult(new ResultState(false, "未查询到该歌曲", 0, null));
            }
            return new JsonResult(new ResultState(true,"查询成功", 1, song));
        }

        /// <summary>
        /// 通过查询Query String筛选歌曲列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="artists"></param>
        /// <returns></returns>
        [HttpGet("GetSongs")]
        public JsonResult GetSongs([FromQuery] string name, [FromQuery] string artists)
        {
            if (name == null && artists == null)
            {
                return new JsonResult(new ResultState(false, "请设置查询条件", 0, null));
            }

            else if (name == null && artists != null)
            {
                var songs = _context.MusicSongs.Where(s => s.artists.Contains(artists)).ToList();
                if (songs.Count() == 0)
                {
                    return new JsonResult(new ResultState(true, "曲库中未包含符合条件的歌曲", 1, null));
                }

                return new JsonResult(new ResultState(true, "查询成功", 0, songs));
            }
            else if (name != null && artists == null)
            {
                var songs = _context.MusicSongs.Where(s => s.name.Contains(name)).ToList();
                if (songs.Count() == 0)
                {
                    return new JsonResult(new ResultState(true, "曲库中未包含符合条件的歌曲", 1, null));
                }

                return new JsonResult(new ResultState(true, "查询成功", 1, songs));
            }
            else
            {
                var songs = _context.MusicSongs.Where(s => s.name.Contains(name) && s.artists.Contains(artists)).ToList();
                if (songs.Count() == 0)
                {
                    return new JsonResult(new ResultState(true, "曲库中未包含符合条件的歌曲", 1, null));
                }
                return new JsonResult(new ResultState(true, "查询成功", 1, songs));
            }

        }




        /// <summary>
        /// 根据id删除歌曲
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteSong/{id}")]
        public JsonResult DeleteSong(int id)
        {
            ResultState resultState = CheckCookie();

            if (resultState.code == 0)
            {
                return new JsonResult(resultState);
            }
            var song = _context.MusicSongs.Find(id);
            if(song == null)
            {
                return new JsonResult(new ResultState(true, "曲库中不存在该歌曲", 1, null));
            }
            try
            {
                _context.MusicSongs.Remove(song);
                _context.SaveChanges();
                return new JsonResult(new ResultState(true, "删除歌曲成功", 1, song));
            }
            catch
            {
                return new JsonResult(new ResultState(false, "删除歌曲失败", 0, null));
            }
        }



        /// <summary>
        /// 修改歌曲文本信息
        /// </summary>
        /// <param name="id">歌曲的id属性</param>
        /// <param name="musicSong">歌曲json对象</param>
        /// <returns></returns>
        [HttpPut("UpdateSongTextInfo/{id}")]
        public JsonResult UpdateSongTextInfo(int id, MusicSong musicSong)
        {
            ResultState resultState = CheckCookie();

            if (resultState.code == 0)
            {
                return new JsonResult(resultState);
            }
            if (id != musicSong.id)
            {
                return new JsonResult(new ResultState(false, "请检查参数", 0, null));
            }
            
            var song = _context.MusicSongs.Find(id);
            if(song == null)
            {
                return new JsonResult(new ResultState(false, "修改信息失败", 0, null));
            }
            //  var properties = typeof(MusicSong).GetProperties();
            //  foreach (System.Reflection.PropertyInfo info in properties)
            //  {

            //  }

            //  暂时未发现什么好办法修改属性
            if (musicSong.name != null)
            {
                song.name = musicSong.name;
            }
            if (musicSong.album != null)
            {
                song.album = musicSong.album;
            }
            if (musicSong.artists != null)
            {
                song.artists = musicSong.artists;
            }
            if (musicSong.lyric != null)
            {
                song.lyric = musicSong.lyric;
            }
            if (musicSong.play_url != null)
            {
                song.play_url = musicSong.play_url;
            }
            if (musicSong.cover_post != null)
            {
                song.cover_post = musicSong.cover_post;
            }
            try
            {
                _context.SaveChanges();
            }
            catch
            {
                return new JsonResult(new ResultState(false, "修改失败", 0, musicSong));
            }

            return new JsonResult(new ResultState(true,"修改成功",1,song));
        }

     /// <summary>
     /// 上传歌曲
     /// </summary>
     /// <param name="MusicFile">音乐本体文件</param>
     /// <param name="PictureFile">音乐封面图片</param>
     /// <param name="name"></param>
     /// <param name="album"></param>
     /// <param name="artists"></param>
     /// <param name="lyric"></param>
     /// <returns></returns>
        [HttpPost("AddSong")]
        public JsonResult AddSong([FromForm] IFormFileCollection MusicFile, [FromForm] IFormFileCollection PictureFile,
            [FromForm] string name, [FromForm] string album, 
            [FromForm] string artists, [FromForm] string lyric)
        {
            
            var file1 = MusicFile;
            var file2 = PictureFile;
            ResultState resultState = CheckCookie();

            if (resultState.code == 0)
            {
                return new JsonResult(resultState);
            }
            //如果文件为空，返回错误信息

            if (file1 == null ||file2 == null)
            {
                return new JsonResult(new ResultState(false, "未添加文件", 0, null));
            }

            
            try
            {
                List<String> urls1 = InputFile.inputFile(file1);
                List<String> urls2 = InputFile.inputFile(file2);

                if (urls1[0] == "null" || urls2[0]=="null")
                {
                  
                    return new JsonResult(new ResultState(false, "文件参数异常", 0, null));
                }
                else if (urls1[0] == "error" || urls2[0] == "error")
                {
                    return new JsonResult(new ResultState(false, "音乐上传失败", 0, null));

                }
                else
                {
                    var song = new MusicSong(name,album,artists,lyric,urls1[0],urls2[0]);
                    _context.MusicSongs.Add(song);
                    _context.SaveChanges();
                    return new JsonResult(new ResultState(true, "上传成功", 1, song));
                }

            }
            catch
            {
                return new JsonResult(new ResultState(false, "上传失败，请检查参数名称", 0, null));
            }

        }
            
    }
}
