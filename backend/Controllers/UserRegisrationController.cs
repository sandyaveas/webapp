using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Transport.Model;
using Transport.Repositories.Implimentation;
using Transport.Repositories.Interfaces;

namespace Transport.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserRegisrationController : ControllerBase
    {
        private readonly IUserRegistration userRegistration;

        public UserRegisrationController(IUserRegistration userRegistration)
        {
            this.userRegistration = userRegistration;

        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> AddUser([FromBody] UserModel userModel)
        {
            var result = await userRegistration.Registration(userModel);

            if (result != null)
            {
                return Ok(new { message = "Registration successful, please login." });
            }
            return BadRequest("Registration failed.");
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserModel userModel)
        {
            // Call the service to handle login
            var loginResult = await userRegistration.Login(userModel);

            if (loginResult.Message == "Login successful")
            {
                // Return success with email in the response
                return Ok(new { message = loginResult.Message, email = loginResult.Email });
            }
            else if (loginResult.Message == "Profile page redirect")
            {
                // If profile image is missing, return redirect message
                return Ok(new { message = loginResult.Message , email = loginResult.Email });
            }

            // If login is not successful, return Unauthorized
            return Unauthorized(new { message = loginResult.Message });
        }





        [HttpPost]
        [Route("upload-profile-image/{userEmail}")]
        public async Task<IActionResult> UploadProfileImage(string userEmail, [FromForm] IFormFile profileImage)
        {
            Console.WriteLine($"Received request to upload image for user: {userEmail}");

            var result = await userRegistration.UploadProfileImage(userEmail, profileImage);

            if (result == "Profile image uploaded successfully")
            {
                return Ok(new { message = result });
            }
            return BadRequest(result);
        }







        


    }
}
