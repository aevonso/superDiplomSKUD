namespace serverSKUD.Model
{
    // EmployeeUpdateDto.cs
    public class EmployeeUpdateDto
    {
        public string LastName { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string Patronymic { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public int DivisionId { get; set; }
        public int PostId { get; set; }
        public string Login { get; set; } = default!;
        public string PassportSeria { get; set; } = default!;
        public string PassportNumber { get; set; } = default!;
        // Login и Password тут НЕ нужны
    }

}
