using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace Monizze.Common.Implementations
{
    public partial class CredentialManager
    {
        private const string FolderName = "Vault";
        private const string FileNameFormat = "{0}.dat";

        private async Task SaveToCache(object value, string name)
        {
            try
            {
                var folder = await GetCacheFolder(FolderName);
                var f = await folder.CreateFileAsync(string.Format(FileNameFormat, name), CreationCollisionOption.ReplaceExisting);
                var temp = JsonConvert.SerializeObject(value);
                await FileIO.WriteBufferAsync(f, await _encryptor.ProtectAsync(temp));
            }
            catch (Exception)
            {
                //  ¯\_(ツ)_/¯
            }
        }

        public async Task<T> GetFromCache<T>(string name)
        {
            try
            {
                var folder = await GetCacheFolder(FolderName);
                var buffer = await FileIO.ReadBufferAsync(await folder.GetFileAsync(string.Format(FileNameFormat, name)));
                var temp = await _encryptor.UnprotectAsync(buffer);
                return JsonConvert.DeserializeObject<T>(temp);
            }
            catch (FileNotFoundException)
            {
                //  ¯\_(ツ)_/¯
            }
            catch (Exception)
            {
                //  ¯\_(ツ)_/¯
            }
            return default(T);
        }

        protected async Task ClearCache()
        {
            try
            {
                var localFolder = await GetCacheFolder(FolderName);
                var files = await localFolder.GetFilesAsync();
                foreach (var t in files)
                {
                    await t.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
            }
            catch (Exception)
            {
                //There is no cache folder, this causes the app to fail when the login failed
            }
        }

        protected async Task<StorageFolder> GetCacheFolder(string folderName)
        {
            try
            {
                return await ApplicationData.Current.LocalFolder.GetFolderAsync(folderName);
            }
            catch (FileNotFoundException)
            {
                //  ¯\_(ツ)_/¯
            }
            catch (Exception)
            {
                //  ¯\_(ツ)_/¯
            }
            return await ApplicationData.Current.LocalFolder.CreateFolderAsync(folderName);
        }
    }
}
