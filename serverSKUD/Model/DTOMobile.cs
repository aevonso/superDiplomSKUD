namespace serverSKUD.Model
{
    public class MobileDeviceDto
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public string DeviceCode { get; set; } = string.Empty;
    }

    public class CreateMobileDeviceDto
    {
        public int EmployerId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceCode { get; set; } = string.Empty;
    }

    public class UpdateMobileDeviceDto
    {
        public string DeviceName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string DeviceCode { get; set; } = string.Empty;
    }

    public class MobileDeviceFilter
    {
        public string? DeviceName { get; set; }
        public int? EmployerId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

}
