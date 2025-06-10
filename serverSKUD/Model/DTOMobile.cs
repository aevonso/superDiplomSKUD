namespace serverSKUD.Model
{
    public class MobileDeviceDto
    {
        public int Id { get; set; }
        public string DeviceName { get; set; } = null!;

        // Теперь возвращаем клиенту
        public string DeviceCode { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public string EmployeeName { get; set; } = null!;
    }
    public class CreateMobileDeviceDto
    {
        public int EmployerId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
    }
    public class ValidateCodeResponse
    {
        public bool IsValid { get; set; }
    }
    public class UpdateMobileDeviceDto
    {
        public string DeviceName { get; set; } = string.Empty;
    }

    public class MobileDeviceFilter
    {
        public string? DeviceName { get; set; }
        public int? EmployerId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

}
