using AuthenticationAPI.Models;
using AuthenticationAPI.UserData;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AuthenticationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private IUserData _userData;

        public AuthenticationController(IUserData userData)
        {
            _userData = userData;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok(_userData.GetAllUsers());
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(Guid id)
        {
            var user = _userData.GetUserById(id);

            if (user != null)
            {
                return Ok(user);
            }

            return NotFound($"User with id: {id} was not found");
        }

        [HttpPost]
        public IActionResult AddUser(User user)
        {
            _userData.AddUser(user);

            return Created(HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path + "/" + user.Id, user);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(Guid id)
        {
            var user = _userData.GetUserById(id);

            if (user != null)
            {
                _userData.DeleteUser(user);
                return Ok();
            }
            return NotFound($"User with id: {id} was not found");
        }

        [HttpPatch("{id}")]
        public IActionResult ModifyUser(Guid id, User user)
        {
            var existingUser = _userData.GetUserById(id);

            if (existingUser != null)
            {
                user.Id = existingUser.Id;
                _userData.ModifyUser(user);
                return Ok();
            }
            return NotFound($"User with id: {id} was not found");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
