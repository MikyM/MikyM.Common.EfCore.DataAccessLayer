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
        EntityTypeIdTypeDictionary ??= AppDomain.CurrentDomain.GetAssemblies().SelectMany(x =>
                x.GetTypes().Where(y => y.IsClass && !y.IsAbstract && y.IsAssignableToWithGenerics(typeof(IEntity<>))))
            .ToDictionary(x => x, x => x.GetIdType());

        CachedCrudRepos = new Dictionary<Type, Type>();
        CachedReadOnlyRepos = new Dictionary<Type, Type>();
        CachedCrudGenericIdRepos = new Dictionary<Type, Type>();
        CachedReadOnlyGenericIdRepos = new Dictionary<Type, Type>();
        
        foreach (var (entityType, idType) in EntityTypeIdTypeDictionary)
        {
            CachedCrudGenericIdRepos.TryAdd(entityType, typeof(Repository<,>).MakeGenericType(entityType, idType));
            CachedReadOnlyGenericIdRepos.TryAdd(entityType, typeof(ReadOnlyRepository<,>).MakeGenericType(entityType, idType));

            if (idType != typeof(long))
                continue;
            
            CachedCrudRepos.TryAdd(entityType, typeof(Repository<>).MakeGenericType(entityType));
            CachedReadOnlyRepos.TryAdd(entityType, typeof(ReadOnlyRepository<>).MakeGenericType(entityType));
        }
    }
    
    internal static Dictionary<Type, Type> EntityTypeIdTypeDictionary { get; }
    internal static Dictionary<Type, Type> CachedReadOnlyRepos { get; }
    internal static Dictionary<Type, Type> CachedCrudRepos { get; }
    internal static Dictionary<Type, Type> CachedReadOnlyGenericIdRepos { get; }
    internal static Dictionary<Type, Type> CachedCrudGenericIdRepos { get; }
}
