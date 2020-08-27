using GoogleSheetsApp.Models;
using GoogleSheetsApp.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(DatabaseService))]
namespace GoogleSheetsApp.Services
{
    public class DatabaseService : IDatabaseService, IAsyncDisposable, IDisposable
    {
        private SQLiteAsyncConnection database;
        private bool disposedValue;

        private static string GetDbLocation(string db = "Responses.db3") => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), db);

        public async Task InitializeServiceAsync()
        {
            if (database != null) return;

            var dbPath = GetDbLocation();
            database = new SQLiteAsyncConnection(dbPath);
            await database.CreateTableAsync<UserResponse>();
        }

        public async Task<int> SaveResponseAsync(UserResponse response)
        {
            await InitializeServiceAsync();
            return await database.InsertAsync(response);
        }


        public async Task<int> UpdateResponseAsync(UserResponse response)
        {
            await InitializeServiceAsync();
            return await database.UpdateAsync(response);
        }

        public async Task<int> GetUnsubmittedResponsesCountAsync()
        {
            await InitializeServiceAsync();
            return await database.Table<UserResponse>().CountAsync();
        }


        public async Task<IList<UserResponse>> RetrieveUnsubmittedResponseAsync()
        {
            await InitializeServiceAsync();
            return await database.Table<UserResponse>().Where(x => !x.HasBeenSubmitted).ToListAsync();
        }

        public async Task<int> UpdateResponsesAsync(IList<UserResponse> responses)
        {
            await InitializeServiceAsync();
            return await database.UpdateAllAsync(responses);
        }

        private async ValueTask DisposeAsyncCore()
        {
            await database.CloseAsync();
            database = null;
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    database?.CloseAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
