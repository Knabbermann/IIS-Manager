using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IIS_Manager.Utility;

namespace IIS_Manager.Models
{
    public class Log
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Type { get; set; } = StaticDetails.LogTypeInformation;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public string Message { get; set; }

        [Required]
        public string WebUserId { get; set; }

        [ForeignKey("WebUserId")]
        [ValidateNever]
        public WebUser WebUser { get; set; }
    }
}
