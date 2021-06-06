using AuthenticationAPI.Models;
using AuthenticationAPI.Services;
using AuthenticationAPI.UserData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AuthenticationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private IUserData _userData;

        private IMessagePublisher _messagePublisher;

        public AuthenticationController(IUserData userData, IMessagePublisher messagePublisher)
        {
            _userData = userData;
            _messagePublisher = messagePublisher;
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
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = _userData.GetUserById(id);

            if (user != null)
            {
                await _messagePublisher.Publish(new Models.User
                {
                    Id = id
                }, "deleteuser");
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
    }
}
