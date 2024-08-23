using System.Threading.Channels;

namespace MoneyTransfer
{
    public class BankRepository
    {
        private readonly BankContext _ctxt;

        public BankRepository(BankContext dbContext) => _ctxt = dbContext;

        //A real payment system would take the debit command as a message that’s added to a queue;
        //then the items in the queue are processed one at a time and in order to prevent the
        //balance from being incorrect due to simultaneous transactions.
        public async Task DebitAsync(string acc, decimal amt)
        {
            var account = await GetAccountAsync(acc);

            if ((account.Balance - amt) < 0)
            {
                throw new InsufficientFundsException($"Account {acc} funds insufficient");
            }

            account.Balance -= amt;
            await _ctxt.SaveChangesAsync();
        }

        public async Task CreditAsync(string acc, decimal amt)
        {
            var account = await GetAccountAsync(acc);
            account.Balance += amt;
            await _ctxt.SaveChangesAsync();
        }

        ////Simple Transfer
        //public Task TransferAsync(
        //  string from, string to, decimal amt) => _ctxt.ExecuteRetryableTransactionAsync(async () =>
        //  {
        //      await CreditAsync(to, amt);
        //      await DebitAsync(from, amt);
        //  });

        //A transactional version of the BankRepository.TransferAsync

        //It’s entirely possible that you won’t capture all the failure conditions in the transaction and call Rollback.
        //The transaction object is disposable, which is why we use using. When the transaction object is disposed,
        //and if an explicit rollback or commit wasn’t called, the transaction will roll back by default.
        //If you don’t use using, the garbage collector will dispose the object eventually, but you don’t control when.
        public async Task TransferAsync(string from, string to, decimal amt)
        {
            using var tx = await _ctxt.Database.BeginTransactionAsync();

            try
            {
                await CreditAsync(to, amt);
                await DebitAsync(from, amt);
                //Commits only if no exception is thrown
                await tx.CommitAsync();
            }
            catch
            {
                //Explicitly undoes changes
                await tx.RollbackAsync();
                //Clears EF Core’s cache
                _ctxt.ChangeTracker.Clear();
                throw;
            }
        }

        //Using a query like Single, First, or Select on a DbSet always results in a query to the database.
        //But Find is one of the ways in which EF Core checks its local cache first and queries the database
        //only if the entity isn’t there. As you use the DbContext in your code, EF Core keeps track of the
        //entities you retrieve and modify through change tracking.
        //Find grabs the local version and prevents a database round trip, which is faster but comes with a caveat.
        //When a transaction rolls back, locally cached entities sometimes don’t get the memo and have the wrong values.
        //We’ll see how to combat this problem by clearing the change tracker
        private async Task<Account> GetAccountAsync(string acc)
        {
            var account = await _ctxt.Accounts.FindAsync(acc);
            return account ?? throw new ArgumentException("No account found: " + acc, nameof(acc));
        }
    }
}
