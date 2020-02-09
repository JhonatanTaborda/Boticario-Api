using System;
using Boticario.Api.Models;
using Boticario.Api.Repository;
using Microsoft.Extensions.Logging;


namespace Boticario.Api.Logger
{
    public class AppLogger : ILogger
    {
        private readonly string _nameCategory;
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly LogRepository _logRepository;
        private readonly int _messageMaxLength = 4000;

        public AppLogger(string nameCategory, Func<string, LogLevel, bool> filter)
        {
            _nameCategory = nameCategory;
            _filter = filter;
            _logRepository = new LogRepository();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventoId,
            TState state, Exception exception, Func<TState, Exception, string> format)
        {
            if (!IsEnabled(logLevel))
                return;

            if (format == null)
                throw new ArgumentNullException(nameof(format));

            var mensagem = format(state, exception);
            if (string.IsNullOrEmpty(mensagem))
            {
                return;
            }

            if (exception != null)
                mensagem += $"\n{exception.ToString()}";

            mensagem = mensagem.Length > _messageMaxLength ? mensagem.Substring(0, _messageMaxLength) : mensagem;
            var eventLog = new LogModel()
            {
                Message = mensagem,
                EventId = eventoId.Id,
                LogLevel = logLevel.ToString(),
                CreatedTime = DateTime.UtcNow
            };

            _logRepository.InsertLog(eventLog);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (_filter == null || _filter(_nameCategory, logLevel));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
