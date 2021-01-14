using live.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace live.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly LiveMultiContext _context;

        public AdminController(LiveMultiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToList();
        }

        public async Task<IActionResult> GetRecordVedio(int id)
        {
            var record_vedio = await _context.RecordVideos.FindAsync(id);
            return (IActionResult)record_vedio;
        }

        public bool IsOnline(int roomId)//直播间id
        {
            var status = _context.RecordVideos.FindAsync(roomId);
            //return _context.RecordVideos.Any(e => e.status == status);
        }

        [HttpGet("{id}")]
        public ActionResult<Admin> Get(int id)
        {
            return _context.Admins.Find(id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public async Task<ActionResult<User>> ModVedioStatus(int id)
        {
            //var user = User GetUser(id);
            var user = await _context.Users.FindAsync(id);
            if (IsOnline(status))
            {
                return 
            }
            
            return user;
        }
    }
}