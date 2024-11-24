using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Transport.Model
{
    public class UserModel
    {

        public int UserID { get; set; }

        // First Name, Last Name, Middle Name, and other strings are nullable
       
        public string? FirstName { get; set; }

   
        public string? LastName { get; set; }

    
        public string? MiddleName { get; set; }

       
       
        public string? Email { get; set; }

       
        public string? Contact { get; set; }

        public string? Password { get; set; }

        // DateTime should be nullable if you want it to accept nulls
        public DateTime? CreatedDate { get; set; }

        
        public string? Address { get; set; }

       
        public string? Role { get; set; }

       
        public string? ProfileImage { get; set; }

        
        public string? AadharImage { get; set; }

       
        public string? PancardImage { get; set; }

        public string? Status { get; set; }
    }
}
