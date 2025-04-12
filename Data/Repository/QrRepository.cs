using AutorizationDomain;
using AutorizationDomain.Queries.Object;
using Data;
using Data.Tables;

public class QrRepository : IQrRepository
{
    private readonly Connection _context;

    public QrRepository(Connection context)
    {
        _context = context;
    }

    public QrInfo SaveQrCode(string login, string qrBase64)
    {
        var employee = _context.Employees
            .FirstOrDefault(e => e.Login == login);

        if (employee == null) throw new Exception("Сотрудник не найден");

        var entity = new QrCode
        {
            EmployeeId = employee.Id,
            QrBase64 = qrBase64
        };
        _context.QrCodes.Add(entity);
        _context.SaveChanges();

        return new QrInfo
        {
            Id = entity.Id,
            Login = employee.Login,
            QrBase64 = entity.QrBase64,
            CreatedAt = entity.CreatedAt
        };
    }

    public QrInfo? GetQrCodeByLogin(string login)
    {
        var query = from emp in _context.Employees
                    join code in _context.QrCodes on emp.Id equals code.EmployeeId
                    where emp.Login == login && code.IsActive
                    select new QrInfo
                    {
                        Id = code.Id,
                        Login = emp.Login,
                        QrBase64 = code.QrBase64,
                        CreatedAt = code.CreatedAt
                    };
        return query.FirstOrDefault();
    }
}
