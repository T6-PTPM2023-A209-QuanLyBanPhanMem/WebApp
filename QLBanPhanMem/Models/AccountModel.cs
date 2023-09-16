using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanPhanMem.Models
{
    [Table("NguoiDung", Schema = "dbo")]
    public class AccountModel
    {
        
        public string? Password { get; set; }
        [Key]
        [Required]
        
        public string? Uid { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
    }
}
