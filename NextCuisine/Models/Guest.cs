using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextCuisine.Models
{
    [Table("guests")]
    public class Guest
    {
        // random unique ID string
        [Key]
        [Column("uid")]
        public string Uid { get; set; } = DataTools.RandomString(10);
        // this will shadow as a display and login name
        [Column("username")]
        public string Username { get; set; }
        // login password
        [Column("password")]
        public string Password { get; set; }
        // required for password resets 
        [Column("recovery_email")]
        public string RecoveryEmail { get; set; }
    }
}
