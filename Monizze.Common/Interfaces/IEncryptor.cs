using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Monizze.Common.Interfaces
{
    public interface IEncryptor
    {
        Task<IBuffer> ProtectAsync(string strMsg);
        Task<string> UnprotectAsync(IBuffer buffProtected);
    }
}
