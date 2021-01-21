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

        public MusicSongsController(LiveMultiContext context, ICookieHelper helper)
        {
            _context = context;
            _helper = helper;
        }
        //// GET: api/MusicSongs
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<MusicSong>>> GetMusicSong()
        //{
        //    return await _context.MusicSong.ToListAsync();
        //}

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

        //private bool MusicSongExists(int id)
        //{
        //    return _context.MusicSong.Any(e => e.id == id);
        //}
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

        /// <summary>
        /// 添加歌曲
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddSong([FromBody] MusicSong song)
        {
            if(song != null)
            {
                try {
                    _context.Add(song);
                    _context.SaveChanges();
                    return new JsonResult(new ResultState(true, "添加成功", 1, song));
                }
                catch
                {
                    return new JsonResult(new ResultState(false, "发生一个异常，添加歌曲失败", 1, song));
                }
                
            }
            return new JsonResult(new ResultState(false, "一个异常发生，添加失败", 1, song));


        }
    }
}
