using System.ComponentModel.DataAnnotations;

namespace AccountAPI.Models
{
    public class RegisterUserDto
    {

        public string Name { get; set; }
      
        public string LastName { get; set; }
        
        public string Password { get; set; }
        
        public string ConfirmPassword { get; set; }
        public string Pesel { get; set; }
       
        public string Email { get; set; }
       
        public string PhoneNumber { get; set; }
        
        public DateTime? DateOfBirth { get; set; }

        public float? EnergyConsumption { get; set; }
    }
}
