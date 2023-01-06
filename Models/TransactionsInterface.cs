using Retail_Banking.ViewModels;
using System.Threading.Tasks;

namespace Retail_Banking.Models
{
    public interface ITransactionsInterface
    {
        public Task Transfer(Transactions transactions);
        public Task<CustomerAccountTransactions> Transactions(int AccountID);
        public Task<(Account, Account)> FromAccountAndToAccountData(int FromAccountID, int ToAccountID);
    }
}
