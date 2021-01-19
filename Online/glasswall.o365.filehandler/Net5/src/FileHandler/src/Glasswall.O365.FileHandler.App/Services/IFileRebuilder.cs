using System.Threading.Tasks;

namespace Glasswall.O365.FileHandler.App.Services
{
    public interface IFileRebuilder
    {
        public Task<byte[]> Rebuild(string base64String);
    }
}
