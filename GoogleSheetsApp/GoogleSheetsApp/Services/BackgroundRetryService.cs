using Matcha.BackgroundService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GoogleSheetsApp.Services
{
    public class BackgroundRetryService : IPeriodicTask
    {
        public TimeSpan Interval => new TimeSpan(0, 20, 0);

        public async Task<bool> StartJob()
        {
            var retryService = DependencyService.Get<IResponseRetryService>();

            //return true to continue service
            return !(await retryService.ExecuteAsync());
        }
    }
}
