using System.Collections.Generic;
using System.Reflection;
using MikyM.Common.DataAccessLayer.Repositories;
using MikyM.Common.Domain.Entities;
using MikyM.Common.EfCore.DataAccessLayer.Repositories;
using MikyM.Common.Utilities.Extensions;

namespace MikyM.Common.EfCore.DataAccessLayer.Helpers;

/// <summary>
/// UoF Cache.
/// </summary>
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
        EntityTypes ??= AppDomain.CurrentDomain.GetAssemblies().SelectMany(x =>
            x.GetTypes().Where(y => y.IsClass && !y.IsAbstract && y.IsAssignableTo(typeof(AggregateRootEntity)))).ToList();

        CachedCrudRepos = new();
        CachedReadOnlyRepos = new();
        foreach (var entityType in EntityTypes)
        {
            CachedCrudRepos.TryAdd(entityType, typeof(Repository<>).MakeGenericType(entityType));
            CachedReadOnlyRepos.TryAdd(entityType, typeof(ReadOnlyRepository<>).MakeGenericType(entityType));
        }
    }

    internal static IEnumerable<Type> CachedRepositoryClassTypes { get; }
    internal static IEnumerable<Type> CachedRepositoryInterfaceTypes { get; }
    internal static Dictionary<Type, Type> CachedRepositoryInterfaceImplTypes { get; }
    internal static IEnumerable<Type> EntityTypes { get; }
    internal static Dictionary<Type, Type> CachedReadOnlyRepos { get; }
    internal static Dictionary<Type, Type> CachedCrudRepos { get; }
    internal static ConstructorInfo RepoCtor { get; }
    internal static ConstructorInfo ReadOnlyRepoCtor { get; }
}
