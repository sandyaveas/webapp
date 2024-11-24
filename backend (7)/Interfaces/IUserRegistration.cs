using Transport.Data;
using Transport.Dto;
using Transport.Model;

namespace Transport.Repositories.Interfaces
{
    public interface IUserRegistration
    {
        Task<UserDetail> Registration(UserModel userModel);
        Task<LoginResult> Login(UserModel userModel);

        Task<string> UploadProfileImage(string userEmail, IFormFile profileImage);
        Task<string> UploadAadharAndPan(string userEmail, IFormFile aadharCard, IFormFile panCard);
    }
}
