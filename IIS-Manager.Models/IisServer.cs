using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IIS_Manager.Models
{
    public class IisServer
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [NotMapped]
        private string _name;
        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrEmpty(value) ? Address : value;
        }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Service { get; set; }

        [Required]
        public string Username { get; set; }

        [NotMapped]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string PasswordHash { get; set; }

        [NotMapped]
        public List<AppPool> AppPools { get; set; }
        [NotMapped]
        public string ErrorMessage { get; set; }
        [NotMapped]
        public string[] HealthCheck { get; set; }
    }
}
