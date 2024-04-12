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
    /// <returns><see cref="DbContextOptionsBuilder"/>.</returns>
    public static DbContextOptionsBuilder IsolationLevelResetForPoolConnection(
        this DbContextOptionsBuilder optionsBuilder,
        IsolationLevel defaultIsolationLevel = IsolationLevel.ReadCommitted)
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

        return optionsBuilder.AddInterceptors(new IsolationLevelInterceptor(defaultIsolationLevel));
    }
}