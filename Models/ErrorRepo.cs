using System.Linq;

namespace Retail_Banking.Models
{
    public class ErrorRepo : ErrorInterface
    {
        public ManagementContext ManagementContext;
        public ErrorRepo(ManagementContext managementContext)
        {
            ManagementContext = managementContext;
        }
        public string GetErrorMessage(int ErrorID)
        {
            return ManagementContext.Error.Where(c => c.ErrorID == ErrorID).FirstOrDefault().ErrorMessage;
        }
    }
}
