using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transport.Data;
using Transport.Repositories.Implimentation;
using Transport.Repositories.Interfaces;

namespace Transport.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Fileupload : ControllerBase
    {
        
        private readonly IUserRegistration userRegistration;
        private readonly TransportDbContext _transportDb;
        private readonly IWebHostEnvironment _environment;
        public Fileupload(IUserRegistration userRegistration, TransportDbContext transportDb, IWebHostEnvironment environment)
        {
            this.userRegistration = userRegistration;
            _transportDb = transportDb;
            _environment = environment;

        }

        
        

        [HttpPost]
        [Route("upload-aadhar-pan/{userEmail}")]
        public async Task<IActionResult> UploadAadharAndPan(string userEmail, [FromForm] List<IFormFile> files)
        {
            try
            {
                if (files == null || files.Count != 2)
                {
                    return BadRequest("Exactly two files must be uploaded.");
                }

                // Ensure that the user exists
                var user = await _transportDb.userDetail.FirstOrDefaultAsync(u => u.Email == userEmail);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                // Segregate files by their names or extensions
                var aadharCard = files.FirstOrDefault(f => f.FileName.Contains("aadhar", StringComparison.OrdinalIgnoreCase));
                var panCard = files.FirstOrDefault(f => f.FileName.Contains("pan", StringComparison.OrdinalIgnoreCase));

                if (aadharCard == null || panCard == null)
                {
                    return BadRequest("Both Aadhar and PAN cards must be uploaded. Ensure files are named accordingly.");
                }

                // Validate file formats
                var validExtensions = new List<string> { ".jpeg", ".jpg", ".png", ".pdf" };
                if (!validExtensions.Contains(Path.GetExtension(aadharCard.FileName).ToLower()) ||
                    !validExtensions.Contains(Path.GetExtension(panCard.FileName).ToLower()))
                {
                    return BadRequest("Invalid file format. Allowed formats are .jpeg, .jpg, .png, .pdf.");
                }

                // Create a folder for the user if it doesn't exist
                var userFolderPath = Path.Combine(_environment.WebRootPath, "UserDoc", userEmail);
                Directory.CreateDirectory(userFolderPath);

                // Save the Aadhar card
                var aadharFileName = $"{Guid.NewGuid()}_Aadhar_{aadharCard.FileName}";
                var aadharFilePath = Path.Combine(userFolderPath, aadharFileName);
                using (var stream = new FileStream(aadharFilePath, FileMode.Create))
                {
                    await aadharCard.CopyToAsync(stream);
                }

                // Save the PAN card
                var panFileName = $"{Guid.NewGuid()}_PAN_{panCard.FileName}";
                var panFilePath = Path.Combine(userFolderPath, panFileName);
                using (var stream = new FileStream(panFilePath, FileMode.Create))
                {
                    await panCard.CopyToAsync(stream);
                }

                // Optionally update the user in the database with file names or paths
                user.AadharImage = aadharFileName;
                user.PancardImage = panFileName;
                await _transportDb.SaveChangesAsync();

                return Ok(new { message = "Aadhar and PAN cards uploaded successfully", aadharFile = aadharFileName, panFile = panFileName });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

    }
}
