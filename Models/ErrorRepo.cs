using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Retail_Banking.Models
{
    public class ErrorRepo : IErrorInterface
    {
        public ManagementContext ManagementContext;
        public ErrorRepo(ManagementContext managementContext)
        {
            ManagementContext = managementContext;
        }
        public async Task<string> GetErrorMessage(int ErrorID)
        {
            Error error = await ManagementContext.Error.Where(c => c.ErrorID == ErrorID).FirstAsync();
            return error.ErrorMessage;
        }
    }
}
