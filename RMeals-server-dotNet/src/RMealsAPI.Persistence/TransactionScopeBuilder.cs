using System;
using System.Transactions;

namespace RMealsAPI.Persistence
{
    public class TransactionScopeBuilder
    {
        public static TransactionScope New(
            TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            TimeSpan? timeout = null,
            TransactionScopeAsyncFlowOption asyncFlowOptions = TransactionScopeAsyncFlowOption.Enabled
            )
        {
            // NOTE: this is a fix, to avoid inner transaction timeout's to shorten the outer one...
            //
            // Since each transaction (even the nested one) has it's own timeout...
            // ... and the inner one completes with the outer one when using propagation...
            // ... we must set inner ones timeout to the outer ones (or enlarge inner one's timeout)...
            if (Transaction.Current != null && transactionScopeOption == TransactionScopeOption.Required)
                timeout = TimeSpan.FromHours(6);

            var options = new TransactionOptions
            {
                IsolationLevel = isolationLevel,
                Timeout = timeout ?? TimeSpan.FromMinutes(5)
            };

            return new TransactionScope(transactionScopeOption, options, asyncFlowOptions);
        }
    }
}
