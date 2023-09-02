using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IIS_Manager.Models
{
    public class Favorite
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string AssetType { get; set; }

        [Required]
        public string AssetId { get; set; }

        [Required]
        public bool IsPrivate { get; set; }

        [Required]
        public int DisplayOrder { get; set; }

        public string WebUserId { get; set; }

        [ForeignKey("WebUserId")]
        [ValidateNever]
        public WebUser User { get; set; }

        [ValidateNever]
        [NotMapped]
        public string? AssetServer { get; set; }

        [ValidateNever]
        [NotMapped]
        public string? AssetName { get; set; }

        [ValidateNever]
        [NotMapped]
        public string? AssetState { get; set; }
    }
}
