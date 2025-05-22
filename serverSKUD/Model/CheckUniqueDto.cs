namespace serverSKUD.Model
{
    public class CheckUniqueDto
    {
        public string Email { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string PassportSeria { get; set; } = null!;
        public string PassportNumber { get; set; } = null!;
    }
}
