using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleSheetsApp.Services
{
    public interface IGoogleSheetService
    {
        Task<string> AddDataAsync(List<IList<object>> data);
    }
}