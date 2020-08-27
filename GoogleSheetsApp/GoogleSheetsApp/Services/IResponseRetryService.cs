using System;
using System.Threading.Tasks;

namespace GoogleSheetsApp.Services
{
    public interface IResponseRetryService : IDisposable
    {
        Task<bool> ExecuteAsync();
    }
}