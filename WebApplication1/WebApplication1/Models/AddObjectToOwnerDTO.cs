using System.ComponentModel.DataAnnotations;

namespace Kolokwium_Poprawa.Models
{
    public class AddObjectToOwnerDTO
    {
        [Required]
        public int ownerId { get; set; }
        [Required]
        public int objectId { get; set; }
    }
}
