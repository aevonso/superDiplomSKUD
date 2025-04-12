using AutorizationDomain.Queries.Object;
using AutorizationDomain.Utilities;
using serviceSKUD;
using System;

namespace AutorizationDomain.Queries
{
    public class GenerateQrCommandService : ICommandService<GenerateQrDto>
    {
        private readonly IQrRepository _qrRepository;

        public GenerateQrCommandService(IQrRepository qrRepository)
        {
            _qrRepository = qrRepository ?? throw new ArgumentNullException(nameof(qrRepository));
        }

        public void Execute(GenerateQrDto obj)
        {
            if (string.IsNullOrWhiteSpace(obj.Login))
            {
                throw new ArgumentException("Login не может быть пустым.", nameof(obj.Login));
            }

            string qrBase64 = QrGenerator.Generate(obj.Login);
            QrInfo qrInfo = _qrRepository.SaveQrCode(obj.Login, qrBase64);
        }
    }
}
