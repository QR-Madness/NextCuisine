using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NextCuisine.Data;

namespace NextCuisine.Models
{
    [Table("guests")]
    public class Guest
    {
        // random unique ID string
        [Key]
        [Column("uid")]
        public string Uid { get; set; } = DataTools.RandomString(10);
        [Column("creation_date")]
        public DateTime CreationDateTime { get; set; } = DateTime.Now;
        // this will shadow as a display and login name
        [Column("username")]
        public string Username { get; set; } = String.Empty;
        // login password
        [Column("password")]
        public string Password { get; set; } = String.Empty;
        // required for password resets 
        [Column("recovery_email")]
        public string RecoveryEmail { get; set; } = String.Empty;
    }
}
