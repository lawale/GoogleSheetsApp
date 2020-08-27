using GoogleSheetsApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleSheetsApp.Services
{
    public interface IDatabaseService
    {
        Task<int> GetUnsubmittedResponsesCountAsync();
        Task InitializeServiceAsync();
        Task<IList<UserResponse>> RetrieveUnsubmittedResponseAsync();
        Task<int> SaveResponseAsync(UserResponse response);
        Task<int> UpdateResponseAsync(UserResponse response);
        Task<int> UpdateResponsesAsync(IList<UserResponse> responses);
    }
}