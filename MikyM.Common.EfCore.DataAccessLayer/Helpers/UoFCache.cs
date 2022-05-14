using System.Collections.Generic;
using MikyM.Common.DataAccessLayer.Repositories;
using MikyM.Common.EfCore.DataAccessLayer.Repositories;
using MikyM.Common.Utilities.Extensions;

namespace MikyM.Common.EfCore.DataAccessLayer.Helpers
{
    internal static class UoFCache
    {
        static UoFCache()
        {
            CachedRepositoryClassTypes ??= AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes().Where(t =>
                    t.IsClass && !t.IsAbstract && t.GetInterface(nameof(IRepositoryBase)) is not null))
                .ToList();
            CachedRepositoryInterfaceTypes ??= AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes().Where(t =>
                    t.IsInterface && t.GetInterface(nameof(IRepositoryBase)) is not null))
                .ToList();
            CachedRepositoryInterfaceImplTypes ??= CachedRepositoryInterfaceTypes.ToDictionary(intr => intr,
                intr => CachedRepositoryClassTypes.FirstOrDefault(intr.IsDirectAncestor))!;
        }

        internal static IEnumerable<Type> CachedRepositoryClassTypes { get; }
        internal static IEnumerable<Type> CachedRepositoryInterfaceTypes { get; }
        internal static Dictionary<Type, Type> CachedRepositoryInterfaceImplTypes { get; }
    }
}