using System.Net.Mail;
using System.Net;
using Transport.Data;
using Transport.Model;
using Transport.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Transport.Dto;

namespace Transport.Repositories.Implimentation
{
    public class UserRegistration : IUserRegistration
    {
        private readonly TransportDbContext _transportDb;
        private readonly IWebHostEnvironment _environment;
        public UserRegistration(TransportDbContext transportDb, IWebHostEnvironment environment)
        {
            _transportDb = transportDb;
            _environment = environment;
        }

        public async Task<LoginResult> Login(UserModel userModel)
        {
            
            var validUser = await _transportDb.userDetail
                .FirstOrDefaultAsync(user => user.Email == userModel.Email || user.Contact == userModel.Contact);

            if (validUser != null)
            {
                
                if (BCrypt.Net.BCrypt.Verify(userModel.Password, validUser.Password))
                {
                  
                    if (string.IsNullOrEmpty(validUser.ProfileImage))
                    {
                        return new LoginResult
                        {
                            IsSuccessful = true,
                            Email = validUser.Email,
                            Message = "Profile page redirect"
                        };
                    }

                    
                    return new LoginResult
                    {
                        IsSuccessful = true,
                        Email = validUser.Email,
                        Message = "Login successful"
                    };
                }
            }

            
            return new LoginResult
            {
                IsSuccessful = false,
                Message = "Invalid credentials or incorrect password"
            };
        }


        public async Task<UserDetail> Registration(UserModel userModel)
        {
           
            bool isFirstUser = !_transportDb.userDetail.Any();

            if (isFirstUser)
            {
              
                var existingUser = await _transportDb.userDetail
                    .FirstOrDefaultAsync(u => u.Email == userModel.Email || u.Contact == userModel.Contact);

                if (existingUser != null)
                {
                    throw new InvalidOperationException("Email or Contact already registered.");
                }
            }
            
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userModel.Password);

            

          
            var userDetail = new UserDetail
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                MiddleName = userModel.MiddleName,
                Email = userModel.Email,
                Contact = userModel.Contact,
                Password = hashedPassword,
                CreatedDate = DateTime.Now,
                Role = isFirstUser ? "Admin" : "User",  
                Status = isFirstUser ? "Active":"Inactive"  
            };

            // Add the user to the database
            _transportDb.userDetail.Add(userDetail);
            await _transportDb.SaveChangesAsync();

            // Generate and send OTP
            //var otp = GenerateOtp();

            // Send OTP to email
            //SendOtpEmail(userDetail.Email, otp);

            // Send OTP to contact number (SMS - Here we simulate the SMS sending)
            //SendOtpSms(userDetail.Contact, otp);

            // Return the user details or any relevant info
            return userDetail;
        }


        // Simple OTP generator (you can make it more complex if needed)
        private string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();  // 6-digit OTP
        }

        private void SendOtpEmail(string email, string otp)
        {
            try
            {
                var fromAddress = new MailAddress("your-email@example.com", "Transport App");
                var toAddress = new MailAddress(email);
                const string subject = "Your OTP for Registration";
                string body = $"Your OTP is: {otp}";

                var smtp = new SmtpClient
                {
                    Host = "smtp.example.com",
                    Port = 587,
                    Credentials = new NetworkCredential("your-email@example.com", "your-email-password"),
                    EnableSsl = true
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                // Handle any errors while sending email (log it, etc.)
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        // Simulate sending OTP via SMS (replace with an actual SMS provider in production)
        private void SendOtpSms(string contact, string otp)
        {
            // Use a real SMS API to send OTP via SMS
            Console.WriteLine($"Sending OTP {otp} to {contact} via SMS.");
        }




        public async Task<string> UploadProfileImage(string userEmail, IFormFile profileImage)
        {
            try
            {
                if (profileImage == null)
                {
                    return "Profile Image is required.";
                }

                var validExtensions = new List<string> { ".jpeg", ".png", ".gif", ".jpg" };
                var extension = Path.GetExtension(profileImage.FileName);

                if (!validExtensions.Contains(extension.ToLower()))
                {
                    return "Invalid image format. Only .jpeg, .jpg, .png, .gif are allowed.";
                }

                // Check if user exists by email (assuming the user model has an Email property)
                var user = await _transportDb.userDetail.FirstOrDefaultAsync(u => u.Email == userEmail);
                if (user == null)
                {
                    return "User not found.";
                }

                // Create "UserDoc" folder if it doesn't exist
                var userDocFolderPath = Path.Combine(_environment.WebRootPath, "UserDoc");
                if (!Directory.Exists(userDocFolderPath))
                {
                    Directory.CreateDirectory(userDocFolderPath);
                }

                // Create user's specific folder
                var userFolderPath = Path.Combine(userDocFolderPath, userEmail);
                if (!Directory.Exists(userFolderPath))
                {
                    Directory.CreateDirectory(userFolderPath);
                }

                // Create ProfileImage folder for the user if it doesn't exist
                var profileImageFolderPath = Path.Combine(userFolderPath, "ProfileImage");
                if (!Directory.Exists(profileImageFolderPath))
                {
                    Directory.CreateDirectory(profileImageFolderPath);
                }

                var profileImageFileName = $"{Guid.NewGuid()}_{profileImage.FileName}";
                var profileImageFilePath = Path.Combine(profileImageFolderPath, profileImageFileName);

                // Save the Profile Image to disk
                using (var stream = new FileStream(profileImageFilePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }

                // Update the user's profile image path in the database
                user.ProfileImage = profileImageFileName;
                await _transportDb.SaveChangesAsync();

                return "Profile image uploaded successfully";
            }
            catch (Exception ex)
            {
                // Handle the exception by returning an error message
                return $"An error occurred: {ex.Message}";
            }
        }




        public async Task<string> UploadAadharAndPan(string userEmail, IFormFile aadharCard, IFormFile panCard)
        {
            if (aadharCard == null || panCard == null)
            {
                return "Both Aadhar Card and PAN Card are required.";
            }

            var userFolderPath = Path.Combine(_environment.WebRootPath, "UserDoc", userEmail.ToString());
            Directory.CreateDirectory(userFolderPath);

            // Define file paths for Aadhar Card and PAN Card
            var aadharCardFileName = $"{Guid.NewGuid()}_{aadharCard.FileName}";
            var panCardFileName = $"{Guid.NewGuid()}_{panCard.FileName}";

            var aadharCardFilePath = Path.Combine(userFolderPath, "AadharCard", aadharCardFileName);
            var panCardFilePath = Path.Combine(userFolderPath, "PanCard", panCardFileName);

            // Create necessary folders
            Directory.CreateDirectory(Path.Combine(userFolderPath, "AadharCard"));
            Directory.CreateDirectory(Path.Combine(userFolderPath, "PanCard"));

            // Save files to disk
            using (var stream = new FileStream(aadharCardFilePath, FileMode.Create))
            {
                await aadharCard.CopyToAsync(stream);
            }

            using (var stream = new FileStream(panCardFilePath, FileMode.Create))
            {
                await panCard.CopyToAsync(stream);
            }

            // Update user record in the database
            var user = await _transportDb.userDetail.FindAsync(userEmail);
            if (user == null)
            {
                return "User not found.";
            }

            user.AadharImage = aadharCardFileName;
            user.PancardImage = panCardFileName;

            await _transportDb.SaveChangesAsync();

            return "Aadhar and PAN cards uploaded successfully";
        }
    }
}
