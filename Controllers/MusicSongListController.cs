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

        public MusicSongListController(LiveMultiContext context,ICookieHelper helper)
        {
            _context = context;
            _helper = helper;

        }


        /// <summary>
        /// 分页获取所有歌单列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("getSongList")]
        public JsonResult getSongList([FromBody] QueryParameters query)
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

            //PageInfoList<User> pageUsers = new PageInfoList<User>(temp, count, query.pageIndex, query.pageSize);
            //pageUsers.items = temp;
            //pageUsers.count = count;
            //pageUsers.pageIndex = query.pageIndex;
            //pageUsers.pageSize = query.pageSize;
            ResultState resultState = new ResultState();
            resultState.success = true;
            resultState.message = "查询成功";
            resultState.value = pageSongList;
            return new JsonResult(resultState);
        }


        /// <summary>
        /// 通过用户id分页获取所有歌单列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("getSongListByUserId/{id}")]
        public JsonResult getSongListByUserId([FromBody] QueryParameters query,int id)
        {
            int count = _context.MusicSongLists.Where(x => x.user_id == id).Count();
            int pageSize1 = query.pageSize;
            List<MusicSongList> temp = new List<MusicSongList>();
            PageInfoList pageSongList = new PageInfoList();
            if (query.pageIndex <= 0)
            {
                temp = (List<MusicSongList>)_context.MusicSongLists.Where(x=>x.user_id==id).Take(query.pageSize).ToList();
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

            //PageInfoList<User> pageUsers = new PageInfoList<User>(temp, count, query.pageIndex, query.pageSize);
            //pageUsers.items = temp;
            //pageUsers.count = count;
            //pageUsers.pageIndex = query.pageIndex;
            //pageUsers.pageSize = query.pageSize;
            ResultState resultState = new ResultState();
            resultState.success = true;
            resultState.message = "查询成功";
            resultState.value = pageSongList;
            return new JsonResult(resultState);

        }

        //增加歌单
        //TODO

        //删除歌单
        //TODO

        //改歌单
        //TODO


        //通过歌单id分页索取其列表中歌曲详细信息
        //TODO



    }
}
