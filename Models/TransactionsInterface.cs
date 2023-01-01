using Retail_Banking.ViewModels;

namespace Retail_Banking.Models
{
    public interface TransactionsInterface
    {
        public void Transfer(Transactions transactions);
        public CustomerAccountTransactions Transactions(int AccountID);
        public (Account, Account) FromAccountAndToAccountData(int FromAccountID, int ToAccountID);
    }
}
