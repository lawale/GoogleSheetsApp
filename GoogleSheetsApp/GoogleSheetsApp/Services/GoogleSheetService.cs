using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using GoogleSheetsApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource.AppendRequest;

[assembly: Dependency(typeof(GoogleSheetService))]
namespace GoogleSheetsApp.Services
{
    public class GoogleSheetService : IGoogleSheetService
    {
        private const string SpreadSheetId = "###### <Insert Google Sheet Id Here> #####";

        private readonly string ApplicationName = "### <Insert Sheet Name Here> ###";

        private readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };

        private GoogleCredential GetCredentials()
        {
            //Place Sheet in Root Directory
            var assembly = typeof(GoogleSheetService).GetTypeInfo().Assembly;
            var assemblyName = assembly.GetName().Name;

            var path = assembly + ".credentials.json";
            var doesExist = File.Exists(path);

            if (!doesExist) return null;

            using var stream = assembly.GetManifestResourceStream($"{assemblyName}.credentials.json");
            return GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }

        private SheetsService IntializeSheetConnection()
        {
            var credential = GetCredentials();
            
            var sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            return sheetsService;
        }

        public async Task<string> AddDataAsync(List<IList<object>> data)
        {
            string range = "'Form Responses 1'!A2:H";

            var dataValueRange = new ValueRange
            {
                Range = range,
                Values = data
            };

            using var sheetsService = IntializeSheetConnection();
            AppendValuesResponse response;
            try
            {
                var request = sheetsService.Spreadsheets.Values.Append(dataValueRange, SpreadSheetId, range);
                request.ValueInputOption = ValueInputOptionEnum.USERENTERED;
                response = await request.ExecuteAsync();
            }
            catch
            {
                return null;
            }

            return JsonConvert.SerializeObject(response);


        }


    }
}
