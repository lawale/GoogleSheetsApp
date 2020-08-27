using GoogleSheetsApp.Models;
using GoogleSheetsApp.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace GoogleSheetsApp.ViewModels
{
    public class FormsViewModel : BindableBase
    {
        private readonly IDatabaseService databaseService;
        private readonly IGoogleSheetService googleSheetService;
        private readonly IResponseRetryService responseRetryService;

        private UserResponse response;
        public FormsViewModel()
        {
            Response = new UserResponse();
            databaseService = DependencyService.Get<IDatabaseService>();
            googleSheetService = DependencyService.Get<IGoogleSheetService>();
            responseRetryService = DependencyService.Get<IResponseRetryService>();
            RetryCommand = new Command(async () => await ResubmitResponsesAsync());
            SubmitCommand = new Command(async () => await SubmitResponseAsync());
            LoadCommand = new Command(async () => await LoadUnsentResponsesAsync());
        }

        private int count;
        public int Count
        {
            get => count;
            set => SetProperty(ref count, value);
        }

        public ICommand LoadCommand { get; }

        public UserResponse Response
        {
            set => SetProperty(ref response, value);

            get => response;
        }

        public ICommand RetryCommand { get; }

        public ICommand SubmitCommand { get; }

        private async Task<bool> CheckForUnsentResponsesAsync()
        {
            if (Count < 1)
            {
                await MaterialDialog.Instance.AlertAsync("You do not have any unsent responses.", "Error");
                return false;
            }

            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                await MaterialDialog.Instance.AlertAsync("No internet access. Please retry when you have internet connection.", "Error");
                return false;
            }

            return true;
        }

        private async Task<bool> EnsureValidationsAsync()
        {
            Response.Validate("Email");
            if (response.HasErrors)
            {
                await MaterialDialog.Instance.AlertAsync("You haven't filled the form.", "Error");
                return false;
            }

            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                await SaveResponseInDbAsync();
                return false;
            }

            return true;
        }

        private async Task HandleResponseSubmitRetryAsync()
        {
            var shouldRetry = await MaterialDialog.Instance.ConfirmAsync("Your response was unable to deliver. You can retry now or schedule it for a later time", "Error", "RETRY", "SCHEDULE LATER");
            if (shouldRetry.HasValue && shouldRetry.Value)
            {
                var result = await googleSheetService.AddDataAsync(response.ToFormsData());
                if (string.IsNullOrEmpty(result))
                {
                    await HandleSaveResponseForRetryAsync(true);
                    return;
                }
                await MaterialDialog.Instance.AlertAsync("Your response has being submitted.", "Success");
                Response = new UserResponse();
                return;
            }
            await HandleSaveResponseForRetryAsync(false);
        }

        private async Task HandleSaveResponseForRetryAsync(bool wasRetried)
        {
            if (wasRetried)
                await MaterialDialog.Instance.AlertAsync("Error submitting response. Response will be delivered later.", "Error");
            else
                await MaterialDialog.Instance.AlertAsync("Your Response will be submitted later.", "Error");

            await databaseService.SaveResponseAsync(Response);
            await LoadUnsentResponsesAsync();
        }

        private async Task LoadUnsentResponsesAsync() => Count = await databaseService.GetUnsubmittedResponsesCountAsync();

        private async Task ResubmitResponsesAsync()
        {
            if (!await CheckForUnsentResponsesAsync()) return;
            bool response;

            using (await MaterialDialog.Instance.LoadingDialogAsync("Submitting your response..."))
            {
                response = await responseRetryService.ExecuteAsync();
            }

            if (!response)
            {
                await MaterialDialog.Instance.AlertAsync("Unable to Submit Response. Submission will be retried later", "Success");
                return;
            }

            await MaterialDialog.Instance.AlertAsync("Your response has being submitted.", "Success");
            await LoadUnsentResponsesAsync();
        }

        private async Task SaveResponseInDbAsync()
        {
            await MaterialDialog.Instance.AlertAsync("No internet access. Your Response will be delivered later.", "Error");
            await databaseService.SaveResponseAsync(Response);
            await LoadUnsentResponsesAsync();
        }
        private async Task SubmitResponseAsync()
        {
            if (! await EnsureValidationsAsync()) return;

            string result;
            using (await MaterialDialog.Instance.LoadingDialogAsync("Submitting your response"))
            {
                 result = await googleSheetService.AddDataAsync(response.ToFormsData());
            }
            
            if(string.IsNullOrEmpty(result))
            {
                await HandleResponseSubmitRetryAsync();
                return;
            }

            await MaterialDialog.Instance.AlertAsync("Your response has being submitted.", "Success");
            Response = new UserResponse();
            return;
        }
    }
}
