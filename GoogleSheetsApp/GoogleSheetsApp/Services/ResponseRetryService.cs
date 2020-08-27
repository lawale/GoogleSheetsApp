using GoogleSheetsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GoogleSheetsApp.Services
{
    public class ResponseRetryService : IResponseRetryService
    {
        private readonly IDatabaseService databaseService;
        private readonly IGoogleSheetService googleSheetService;
        private readonly SemaphoreSlim semaphorSlim = new SemaphoreSlim(1, 1);

        public ResponseRetryService()
        {
            databaseService = DependencyService.Get<IDatabaseService>();
            googleSheetService = DependencyService.Get<IGoogleSheetService>();
        }

        public async Task<bool> ExecuteAsync()
        {
            await semaphorSlim.WaitAsync();

            {
                var unsubmittedResponses = (await databaseService.RetrieveUnsubmittedResponseAsync()).ToList();
                var formdata = unsubmittedResponses.Select(x => x.ToFormsSingleData()).ToList();

                if (formdata is null || !formdata.Any()) return true;

                var result = await googleSheetService.AddDataAsync(formdata);

                if (string.IsNullOrEmpty(result)) return false;

                unsubmittedResponses.ForEach(x => x.HasBeenSubmitted = true);
                await databaseService.UpdateResponsesAsync(unsubmittedResponses);
            }

            semaphorSlim.Release();
            return true;
        }

        public void Dispose()
        {
            try
            {
                ((IDisposable)semaphorSlim)?.Dispose();
            }
            catch (ObjectDisposedException ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
            }

        }
    }
}
