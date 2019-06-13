using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;

namespace MSGraphOneDriveIssue
{
    public static class UploadAndEditFileFunction
    {
        private static string AuthorityUrl = "https://login.microsoftonline.com/";
        private static string Tenant = "";
        private static string ClientId = "";
        private static string ClientSecret = "";
        private static string AADGraphResourceId = "https://graph.microsoft.com/";
        private static string GroupId = "";
        private static string RootFolderPath = "/TestProjectFolder";
        private static string PathToLocalFile = "C:/Desktop/test.txt"; // path to the file you need to work with
        private static string LocalFileName = "test.txt"; // path to the file you need to work with

        [FunctionName("UploadAndEditFile")]
        public static async Task<DriveItem> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var authContext = new AuthenticationContext(AuthorityUrl + Tenant);
            GraphServiceClient _graphClient = new GraphServiceClient(new DelegateAuthenticationProvider(
                   async (requestMessage) =>
                   {
                       string accessToken;
                       var credential = new ClientCredential(ClientId, ClientSecret);

                       var result = await authContext.AcquireTokenAsync(AADGraphResourceId, credential).ConfigureAwait(false);
                       accessToken = result.AccessToken;

                       // Configure the HTTP bearer Authorization Header
                       requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                   }));

            using (FileStream fs = System.IO.File.OpenRead(PathToLocalFile))
            {
                try
                {
                    DriveItem newFile;
                    var fileName = $"{LocalFileName.Split('.')[0]}{Guid.NewGuid()}.{LocalFileName.Split('.')[1]}";

                    #region action 1: upload new file
                    // is exist file
                    try
                    {
                        newFile = await _graphClient.Groups[GroupId].Drive.Root.ItemWithPath($"{RootFolderPath}/{fileName}").Request().GetAsync();
                    }
                    catch (Microsoft.Graph.ServiceException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        newFile = null;
                    }

                    // create new file
                    if (newFile == null)
                    {
                        newFile = newFile = new DriveItem()
                        {
                            File = new Microsoft.Graph.File(),
                            Name = fileName
                        };

                        newFile = await _graphClient.Groups[GroupId].Drive.Root.ItemWithPath(RootFolderPath).Children.Request().AddAsync(newFile);
                    }

                    // update exist file
                    else
                    {
                        newFile = await _graphClient.Groups[GroupId].Drive.Root.ItemWithPath($"{RootFolderPath}/{fileName}").Content.Request().PutAsync<DriveItem>(fs);
                    }
                    #endregion

                    #region action 2: modify file

                    //download file
                    var stream = await _graphClient.Groups[GroupId].Drive.Root.ItemWithPath($"{RootFolderPath}/{fileName}").Content.Request().GetAsync();

                    // get exist file
                    newFile = await _graphClient.Groups[GroupId].Drive.Root.ItemWithPath($"{RootFolderPath}/{fileName}").Request().GetAsync();

                    /*
                        do modify actions 
                     */

                    // upload modified file
                    newFile = await _graphClient.Groups[GroupId].Drive.Root.ItemWithPath($"{RootFolderPath}/{fileName}").Content.Request().PutAsync<DriveItem>(fs);
                    #endregion

                    return newFile;
                }
                catch (Microsoft.Graph.ServiceException e)
                {
                    log.LogError("Error: " + e);
                    throw e;
                }
            }
        }
    }
}
