using DashboardDomain.Queries.Object;
using serviceSKUD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xceed.Words.NET; // Для DOCX
using iText.Kernel.Pdf; // Для PDF
using iText.Layout; // Для PDF
using iText.Layout.Element; // Для PDF
using iText.Kernel.Font; // Для PDF
using iText.IO.Font.Constants; // Для PDF
using DashboardDomain.IRepository;

namespace DashboardDomain.Queries
{
    public class GenerateReportQueryService : IGenerateReportService
    {
        private readonly IEmployeeRepository _employee;
        private readonly IMobileDeviceRepository _mobile;
        private readonly IAccessAttemptRepository _accessAttempt;

        public GenerateReportQueryService(
            IEmployeeRepository employee,
            IMobileDeviceRepository mobile,
            IAccessAttemptRepository accessAttempt)
        {
            _employee = employee;
            _mobile = mobile;
            _accessAttempt = accessAttempt;
        }

        public ReportResult Execute(ReportCriteria criteria)
        {
            var emps = criteria.IncludeEmployees
                ? _employee.GetAllAsync().GetAwaiter().GetResult()
                : Enumerable.Empty<EmployeeDto>();

            var devs = criteria.IncludeMobileDevices
                ? _mobile.GetAllAsync(onlyActive: true).GetAwaiter().GetResult()
                : Enumerable.Empty<MobileDeviceDto>();

            
            var atts = criteria.IncludeAccessAttempts
      ? _accessAttempt.GetFilteredAttemptsAsync(
          criteria.FromDate,
          criteria.ToDate,
          null, // pointId
          null, // employeeId
          1000).GetAwaiter().GetResult() 
      : Enumerable.Empty<AttemptDto>();


            if (criteria.SuccessOnly)
            {
                atts = atts.Where(a => a.Success).ToList();
            }
            else if (criteria.FailedOnly)
            {
                atts = atts.Where(a => !a.Success).ToList();
            }

            return criteria.Format?.ToLower() == "docx"
                ? BuildDocx(emps, devs, atts)
                : BuildPdf(emps, devs, atts);
        }

        // Генерация DOCX отчета
        private ReportResult BuildDocx(
            IEnumerable<EmployeeDto> emps,
            IEnumerable<MobileDeviceDto> devs,
            IEnumerable<AttemptDto> atts)
        {
            using var ms = new MemoryStream();
            using var doc = DocX.Create(ms);

            doc.InsertParagraph("Отчет НАТК")
                .FontSize(20).Bold();

            // Сотрудники
       

            // Попытки доступа
            if (atts.Any())
            {
                doc.InsertParagraph("Попытки доступа")
                    .FontSize(16).Bold().SpacingAfter(10);
                var table = doc.AddTable(atts.Count() + 1, 5);
                table.Rows[0].Cells[0].Paragraphs[0].Append("Время");
                table.Rows[0].Cells[1].Paragraphs[0].Append("Сотрудник");
                table.Rows[0].Cells[2].Paragraphs[0].Append("Точка");
                table.Rows[0].Cells[3].Paragraphs[0].Append("IP");
                table.Rows[0].Cells[4].Paragraphs[0].Append("Успех");

                int r = 1;
                foreach (var a in atts)
                {
                    string ipText = GetDisplayIp(a.IpAddress);
                    table.Rows[r].Cells[0].Paragraphs[0].Append(a.Timestamp.ToString("g"));
                    table.Rows[r].Cells[1].Paragraphs[0].Append(a.EmployeeFullName);
                    table.Rows[r].Cells[2].Paragraphs[0].Append(a.PointName);
                    table.Rows[r].Cells[3].Paragraphs[0].Append(ipText);
                    table.Rows[r].Cells[4].Paragraphs[0].Append(a.Success ? "Успех" : "Провал");
                    r++;
                }
                doc.InsertTable(table);
            }

            doc.Save();
            return new ReportResult
            {
                Content = ms.ToArray(),
                MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                FileName = "report.docx"
            };
        }

        // Генерация PDF отчета
        private ReportResult BuildPdf(
            IEnumerable<EmployeeDto> emps,
            IEnumerable<MobileDeviceDto> devs,
            IEnumerable<AttemptDto> atts)
        {
            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var layout = new Document(pdf);

            // Загружаем шрифт с поддержкой кириллицы
            var font = PdfFontFactory.CreateFont("c:/windows/fonts/times.ttf", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
            layout.SetFont(font);

            layout.Add(new Paragraph("Отчет НАТК").SetFontSize(20));

            
            // Попытки доступа
            if (atts.Any())
            {
                layout.Add(new Paragraph("Попытки доступа").SetFontSize(16));
                var tableA = new Table(5);
                tableA.AddHeaderCell("Время");
                tableA.AddHeaderCell("Сотрудник");
                tableA.AddHeaderCell("Точка");
                tableA.AddHeaderCell("IP");
                tableA.AddHeaderCell("Успех");
                foreach (var a in atts)
                {
                    string ipText = GetDisplayIp(a.IpAddress);
                    tableA.AddCell(a.Timestamp.ToString("g"));
                    tableA.AddCell(a.EmployeeFullName);
                    tableA.AddCell(a.PointName);
                    tableA.AddCell(ipText);
                    tableA.AddCell(a.Success ? "Успех" : "Провал");
                }
                layout.Add(tableA);
            }

            layout.Close();
            return new ReportResult
            {
                Content = ms.ToArray(),
                MimeType = "application/pdf",
                FileName = "report.pdf"
            };
        }

        // Метод для корректного отображения IP-адресов
        private string GetDisplayIp(string ipAddress)
        {
            // Если уже пришла локализованная строка - возвращаем как есть
            if (ipAddress == "Неизвестный IP")
                return ipAddress;

            // Стандартная обработка
            if (string.IsNullOrWhiteSpace(ipAddress))
                return "Неизвестный IP";

            string cleanIp = ipAddress.Trim();

            if (cleanIp == "::1")
                return "127.0.0.1";

            return cleanIp;
        }
    }
}