namespace AccountAPI.Models
{
    public class UserInformationsdto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Pesel { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public float? EnergyConsumption { get; set; }

    }
}
