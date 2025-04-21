using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serviceSKUD
{
    public interface IEmailSender
    {
        Task SendCodeAsync(string toEmail, string subject, string htmlBody);
    }
}
