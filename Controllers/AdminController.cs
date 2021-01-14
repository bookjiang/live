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


        [HttpGet("{id}")]
        public ActionResult<Admin> Get(int id)
        {
            return _context.Admins.Find(id);
        }
    }
}