using System.Data.Common;
using System.Transactions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using IsolationLevel = System.Data.IsolationLevel;


namespace Microsoft.EntityFrameworkCore.ResetDBIsolationLevel
{
    internal sealed class IsolationLevelInterceptor : DbConnectionInterceptor
    {
        private readonly IsolationLevel _defaultIsolationLevel;

        public IsolationLevelInterceptor(IsolationLevel defaultIsolationLevel)
        {
            _defaultIsolationLevel = defaultIsolationLevel;
        }

        public override void ConnectionOpened(DbConnection connection,
            ConnectionEndEventData eventData)
        {
            if (Transaction.Current is null && eventData.Context?.Database.CurrentTransaction is null)
            {
                connection.BeginTransaction(_defaultIsolationLevel).Dispose();
            }
        }

        public override async Task ConnectionOpenedAsync(DbConnection connection,
            ConnectionEndEventData eventData,
            CancellationToken cancellationToken = default)
        {
            if (Transaction.Current is null && eventData.Context?.Database.CurrentTransaction is null)
            {
                await (await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken)).DisposeAsync();
            }
        }
    }
}