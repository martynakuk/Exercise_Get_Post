namespace Kolokwium_Poprawa.Models
{
    public class OwnerDTO
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public ICollection<ObjectOwnerDTO> OwnerObjects { get; set; } = null!;
    }


    public class ObjectOwnerDTO
    {
        public int Id { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string Type { get; set; } = null!;
        public string WareHouse { get; set; } = null!;
    }
}
