using System.Threading.Tasks;

namespace Retail_Banking.Models
{
    public interface IErrorInterface
    {
        public Task<string> GetErrorMessage(int ErrorID);
    }
}
