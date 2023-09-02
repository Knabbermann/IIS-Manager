using System.ComponentModel.DataAnnotations.Schema;
using System.Management.Automation;

namespace IIS_Manager.Models
{
    public class AppPool
    {
        public string Name { get; set; } = "Unknown";
        public string State { get; set; } = "Unknown";
        public string RuntimeVersion { get; set; } = "Unknown";
        public List<string?> Applications { get; set; } = new ();
        public bool IsFavorite { get; set; }
    }
}
