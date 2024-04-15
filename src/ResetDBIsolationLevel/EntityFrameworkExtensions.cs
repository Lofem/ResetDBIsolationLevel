using System.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore.ResetDBIsolationLevel;

/// <summary>
/// Extension methods for <see cref="DbContextOptionsBuilder"/>
/// </summary>
public static class EntityFrameworkExtensions
{
    /// <summary>
    /// Grace reset connection isolation level to default
    /// </summary>
    /// <param name="optionsBuilder"><see cref="DbContextOptionsBuilder"/></param>
    /// <param name="defaultIsolationLevel">isolation level by default</param>
    /// <typeparam name="TDbContextOptionsBuilder">The type derived from <see cref="DbContextOptionsBuilder"/></typeparam>
    /// <returns><see cref="DbContextOptionsBuilder"/>.</returns>
    public static TDbContextOptionsBuilder IsolationLevelResetForPoolConnection<TDbContextOptionsBuilder>(
        this TDbContextOptionsBuilder optionsBuilder,
        IsolationLevel defaultIsolationLevel = IsolationLevel.ReadCommitted)
        where TDbContextOptionsBuilder : DbContextOptionsBuilder
    {
        var coreOptionsExtension = optionsBuilder.Options.FindExtension<CoreOptionsExtension>();

        if (coreOptionsExtension is null)
        {
            throw new InvalidOperationException($"{nameof(CoreOptionsExtension)} not found");
        }

        if (coreOptionsExtension.Interceptors?.OfType<IsolationLevelInterceptor>().Any() == true)
        {
            return optionsBuilder;
        }

        optionsBuilder.AddInterceptors(new IsolationLevelInterceptor(defaultIsolationLevel));

        return optionsBuilder;
    }
}