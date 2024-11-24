using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Transport.Data
{
    public class UserDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        // First Name, Last Name, Middle Name, and other strings are nullable
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [StringLength(100)]
        public string? MiddleName { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(15)]
        public string? Contact { get; set; }

        [StringLength(100)]
        public string? Password { get; set; }

        // DateTime should be nullable if you want it to accept nulls
        public DateTime? CreatedDate { get; set; }

        [StringLength(250)]
        public string? Address { get; set; }

        [StringLength(250)]
        public string? Role { get; set; }

        [StringLength(50)]
        public string? ProfileImage { get; set; }

        [StringLength(50)]
        public string? AadharImage { get; set; }

        [StringLength(50)]
        public string? PancardImage { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }
    }
}
