using System;
using Microsoft.Extensions.Logging;

namespace Boticario.Api.ExtensionLogger
{
    public class AppLoggerProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> _filtro;
        private readonly string _connectionString;

        public AppLoggerProvider(Func<string, LogLevel, bool> filtro)
        {
            _filtro = filtro;            
        }

        public ILogger CreateLogger(string nomeCategoria)
        {
            return new AppLogger(nomeCategoria, _filtro);
        }

        public void Dispose()
        {

        }
    }
}
