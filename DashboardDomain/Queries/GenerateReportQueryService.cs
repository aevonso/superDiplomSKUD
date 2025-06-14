using DashboardDomain.Queries.Object;
using serviceSKUD;
using System;
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
                ? _accessAttempt.GetRecentAttemptsAsync(100).GetAwaiter().GetResult()
                : Enumerable.Empty<AttemptDto>();

            if (criteria.FromDate.HasValue)
            {
                atts = atts.Where(a => a.Timestamp >= criteria.FromDate.Value).ToList();
            }

            if (criteria.ToDate.HasValue)
            {
                atts = atts.Where(a => a.Timestamp <= criteria.ToDate.Value).ToList();
            }

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
            using var doc = Xceed.Words.NET.DocX.Create(ms);  // Явно указываем для DOCX

            doc.InsertParagraph("Отчет НАТК")
                .FontSize(20).Bold();  // Для DOCX

            // Сотрудники
            if (emps.Any())
            {
                doc.InsertParagraph("Сотрудники")
                    .FontSize(16).Bold().SpacingAfter(10);
                var table = doc.AddTable(emps.Count() + 1, 3);
                table.Rows[0].Cells[0].Paragraphs[0].Append("ID");
                table.Rows[0].Cells[1].Paragraphs[0].Append("ФИО");
                table.Rows[0].Cells[2].Paragraphs[0].Append("Email");

                int r = 1;
                foreach (var e in emps)
                {
                    table.Rows[r].Cells[0].Paragraphs[0].Append(e.Id.ToString());
                    table.Rows[r].Cells[1].Paragraphs[0].Append($"{e.LastName} {e.FirstName}");
                    table.Rows[r].Cells[2].Paragraphs[0].Append(e.Email);
                    r++;
                }
                doc.InsertTable(table);
            }

            // Мобильные устройства
            if (devs.Any())
            {
                doc.InsertParagraph("Мобильные устройства")
                    .FontSize(16).Bold().SpacingAfter(10);
                var table = doc.AddTable(devs.Count() + 1, 4);
                table.Rows[0].Cells[0].Paragraphs[0].Append("ID");
                table.Rows[0].Cells[1].Paragraphs[0].Append("EmployeeId");
                table.Rows[0].Cells[2].Paragraphs[0].Append("CreatedAt");
                table.Rows[0].Cells[3].Paragraphs[0].Append("Активно");

                int r = 1;
                foreach (var d in devs)
                {
                    table.Rows[r].Cells[0].Paragraphs[0].Append(d.Id.ToString());
                    table.Rows[r].Cells[1].Paragraphs[0].Append(d.EmployeeId.ToString());
                    table.Rows[r].Cells[2].Paragraphs[0].Append(d.CreatedAt.ToString("g"));
                    table.Rows[r].Cells[3].Paragraphs[0].Append(d.IsActive ? "✓" : "✗");
                    r++;
                }
                doc.InsertTable(table);
            }

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
                    table.Rows[r].Cells[0].Paragraphs[0].Append(a.Timestamp.ToString("g"));
                    table.Rows[r].Cells[1].Paragraphs[0].Append(a.EmployeeFullName);
                    table.Rows[r].Cells[2].Paragraphs[0].Append(a.PointName);
                    table.Rows[r].Cells[3].Paragraphs[0].Append(a.IpAddress);
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

            if (emps.Any())
            {
                layout.Add(new Paragraph("Сотрудники").SetFontSize(16));
                var tableE = new Table(3);
                tableE.AddHeaderCell("ID");
                tableE.AddHeaderCell("ФИО");
                tableE.AddHeaderCell("Email");
                foreach (var e in emps)
                {
                    tableE.AddCell(e.Id.ToString());
                    tableE.AddCell($"{e.LastName} {e.FirstName}");
                    tableE.AddCell(e.Email);
                }
                layout.Add(tableE);
            }

            if (devs.Any())
            {
                layout.Add(new Paragraph("Мобильные устройства").SetFontSize(16));
                var tableD = new Table(4);
                tableD.AddHeaderCell("ID");
                tableD.AddHeaderCell("EmployeeId");
                tableD.AddHeaderCell("CreatedAt");
                tableD.AddHeaderCell("Активно");
                foreach (var d in devs)
                {
                    tableD.AddCell(d.Id.ToString());
                    tableD.AddCell(d.EmployeeId.ToString());
                    tableD.AddCell(d.CreatedAt.ToString("g"));
                    tableD.AddCell(d.IsActive ? "✓" : "✗");
                }
                layout.Add(tableD);
            }

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
                    tableA.AddCell(a.Timestamp.ToString("g"));
                    tableA.AddCell(a.EmployeeFullName);
                    tableA.AddCell(a.PointName);
                    tableA.AddCell(a.IpAddress);
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
    }
}
