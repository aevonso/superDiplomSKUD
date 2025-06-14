using DashboardDomain.Queries.Object;
using DashboardDomain.Queries;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IGenerateReportService _report;

    public ReportsController(IGenerateReportService report)
    {
        _report = report;
    }

    [HttpPost]
    public IActionResult Generate([FromBody] ReportCriteria criteria)
    {
        // Логируем полученные параметры
        Console.WriteLine($"Received criteria: {criteria}");

        // Проводим предварительную обработку и проверку параметров
        if (criteria.FromDate.HasValue && criteria.ToDate.HasValue && criteria.FromDate > criteria.ToDate)
        {
            return BadRequest(new { message = "Дата начала не может быть позже даты конца." });
        }

        // Генерация отчета с учетом всех фильтров
        try
        {
            var reportResult = _report.Execute(criteria);
            return File(reportResult.Content, reportResult.MimeType, reportResult.FileName);
        }
        catch (Exception ex)
        {
            // Логируем ошибку
            Console.WriteLine($"Error generating report: {ex.Message}");
            return BadRequest(new { message = "Ошибка при генерации отчета.", error = ex.Message });
        }
    }
}
