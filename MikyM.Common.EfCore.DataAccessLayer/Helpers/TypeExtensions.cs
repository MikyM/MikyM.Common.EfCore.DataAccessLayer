namespace MikyM.Common.EfCore.DataAccessLayer.Helpers;

/// <summary>
/// Type extensions.
/// </summary>
internal static class TypeExtensions
{
    internal static Type GetIdType(this Type type)
    {
        var generic = type.GetInterfaces()
            .First(x => x.IsInterface && x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEntity<>));

        return generic.GenericTypeArguments.First();
    }
}
