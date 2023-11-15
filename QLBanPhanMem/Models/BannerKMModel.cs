using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanPhanMem.Models
{
    [Table("BannerKM", Schema = "dbo")]
    public class BannerKMModel
    {
        public string? HINHANH { get; set; }
        [Required]
        public int? MAPM { get; set; }
        [ForeignKey("MAPM")]
        public virtual PhanMemModel? PhanMem { get; set; }
    }
}
