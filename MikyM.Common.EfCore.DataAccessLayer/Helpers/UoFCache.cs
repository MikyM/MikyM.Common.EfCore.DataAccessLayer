using System.Collections.Generic;
using System.Reflection;
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
        EntityTypeIdTypeDictionary ??= AppDomain.CurrentDomain.GetAssemblies().SelectMany(x =>
                x.GetTypes().Where(y => y.IsClass && !y.IsAbstract && y.IsAssignableTo(typeof(IEntityBase))))
            .ToDictionary(x => x, x => x.GetIdType());

        CachedCrudRepos = new Dictionary<Type, Type>();
        CachedReadOnlyRepos = new Dictionary<Type, Type>();
        foreach (var (entityType, idType) in EntityTypeIdTypeDictionary)
        {
            CachedCrudRepos.TryAdd(entityType, typeof(Repository<,>).MakeGenericType(entityType, idType));
            CachedReadOnlyRepos.TryAdd(entityType, typeof(ReadOnlyRepository<,>).MakeGenericType(entityType, idType));
        }
    }

    internal static IEnumerable<Type> CachedRepositoryClassTypes { get; }
    internal static IEnumerable<Type> CachedRepositoryInterfaceTypes { get; }
    internal static Dictionary<Type, Type> CachedRepositoryInterfaceImplTypes { get; }
    internal static Dictionary<Type,Type> EntityTypeIdTypeDictionary { get; }
    internal static Dictionary<Type, Type> CachedReadOnlyRepos { get; }
    internal static Dictionary<Type, Type> CachedCrudRepos { get; }
}
