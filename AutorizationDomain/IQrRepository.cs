using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutorizationDomain.Queries.Object;

namespace AutorizationDomain
{
    public interface IQrRepository
    {
        QrInfo SaveQrCode(string login, string qrBase64);

        QrInfo? GetQrCodeByLogin(string login);
    }
}
