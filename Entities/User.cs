using Microsoft.Extensions.Primitives;

namespace AccountAPI.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Pesel { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; } 
        public DateTime? DateOfBirth { get; set; }
        public float? EnergyConsumption { get; set; }

    }
}
