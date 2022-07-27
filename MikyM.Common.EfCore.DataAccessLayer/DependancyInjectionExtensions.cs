using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MikyM.Autofac.Extensions;
using MikyM.Common.DataAccessLayer;
using MikyM.Common.EfCore.DataAccessLayer.Pagination;
using MikyM.Common.EfCore.DataAccessLayer.Repositories;
using MikyM.Common.EfCore.DataAccessLayer.Specifications;
using MikyM.Common.EfCore.DataAccessLayer.Specifications.Evaluators;
using MikyM.Common.EfCore.DataAccessLayer.Specifications.Validators;
using MikyM.Common.EfCore.DataAccessLayer.UnitOfWork;

namespace MikyM.Common.EfCore.DataAccessLayer;

/// <summary>
/// DI extensions for <see cref="ContainerBuilder"/>.
/// </summary>
[PublicAPI]
public static class DependancyInjectionExtensions
{
    /// <summary>
    /// Adds Data Access Layer to the application.
    /// </summary>
    /// <remarks>
    /// Automatically registers <see cref="IRepository{TEntity}"/>, <see cref="IReadOnlyRepository{TEntity}"/>, all base <see cref="IEvaluator"/> types, <see cref="ISpecificationValidator"/>, <see cref="IInMemorySpecificationEvaluator"/>, <see cref="ISpecificationEvaluator"/>, <see cref="IUnitOfWork{TContext}"/> with <see cref="ContainerBuilder"/>.
    /// </remarks>
    /// <param name="configuration">Current instance of <see cref="DataAccessConfiguration"/></param>
    /// <param name="options"><see cref="Action"/> that configures DAL.</param>
    public static DataAccessConfiguration AddEfCoreDataAccessLayer(this DataAccessConfiguration configuration, Action<EfCoreDataAccessConfiguration>? options = null)
    {
        if (configuration.GetType().GetField("Builder", BindingFlags.Instance |
                                                        BindingFlags.NonPublic |
                                                        BindingFlags.Public)
                ?.GetValue(configuration) is not ContainerBuilder builder)
            throw new InvalidOperationException();
        
        var config = new EfCoreDataAccessConfiguration(builder);
        options?.Invoke(config);

        var ctorFinder = new AllConstructorsFinder();

        builder.Register(x => config).As<IOptions<EfCoreDataAccessConfiguration>>().SingleInstance();

        builder.RegisterGeneric(typeof(ReadOnlyRepository<>))
            .As(typeof(IReadOnlyRepository<>))
            .FindConstructorsWith(ctorFinder)
            .InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(Repository<>))
            .As(typeof(IRepository<>))
            .FindConstructorsWith(ctorFinder)
            .InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(UnitOfWork<>)).As(typeof(IUnitOfWork<>)).InstancePerLifetimeScope();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            builder.RegisterAssemblyTypes(assembly)
                .Where(x => x.GetInterface(nameof(IEvaluator)) is not null && x != typeof(IncludeEvaluator))
                .As<IEvaluator>()
                .FindConstructorsWith(ctorFinder)
                .SingleInstance();
        }

        builder.RegisterType<IncludeEvaluator>()
            .As<IEvaluator>()
            .UsingConstructor(typeof(bool))
            .FindConstructorsWith(ctorFinder)
            .WithParameter(new TypedParameter(typeof(bool), config.EnableIncludeCache))
            .SingleInstance();

        builder.RegisterType<ProjectionEvaluator>()
            .As<IProjectionEvaluator>()
            .FindConstructorsWith(ctorFinder)
            .SingleInstance();

        builder.RegisterType<SpecificationEvaluator>()
            .As<ISpecificationEvaluator>()
            .UsingConstructor(typeof(IEnumerable<IEvaluator>), typeof(IProjectionEvaluator))
            .FindConstructorsWith(ctorFinder)
            .SingleInstance();

        builder.RegisterType<SpecificationValidator>()
            .As<ISpecificationValidator>()
            .UsingConstructor(typeof(IEnumerable<IValidator>))
            .FindConstructorsWith(ctorFinder)
            .SingleInstance();

        builder.RegisterType<InMemorySpecificationEvaluator>()
            .As<IInMemorySpecificationEvaluator>()
            .UsingConstructor(typeof(IEnumerable<IInMemoryEvaluator>))
            .FindConstructorsWith(ctorFinder)
            .SingleInstance();

        return configuration;
    }

    /// <summary>
    /// Registers services required for pagination with the <see cref="ContainerBuilder"/>.
    /// </summary>
    /// <param name="options">Data access configuration.</param>
    /// <returns>Current instance of the <see cref="EfCoreDataAccessConfiguration"/>.</returns>
    public static EfCoreDataAccessConfiguration AddPaginationServices(this EfCoreDataAccessConfiguration options)
    {
        options.Builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
        options.Builder.Register(x =>
            {
                var accessor = x.Resolve<IHttpContextAccessor>();
                var request = accessor.HttpContext?.Request;
                var uri = string.Concat(request?.Scheme, "://", request?.Host.ToUriComponent());
                return new UriService(uri);
            })
            .As<IUriService>()
            .SingleInstance();
        
        return options;
    }
    
    /// <summary>
    /// Adds a given custom evaluator that implements <see cref="IEvaluator"/> interface.
    /// </summary>
    /// <typeparam name="TEvaluator">Type to register.</typeparam>
    /// <returns>Current <see cref="EfCoreDataAccessConfiguration"/> instance.</returns>
    public static EfCoreDataAccessConfiguration AddEvaluator<TEvaluator>(this EfCoreDataAccessConfiguration efCoreDataAccessOptions) where TEvaluator : class, IEvaluator
    {
        efCoreDataAccessOptions.Builder.RegisterType(typeof(TEvaluator))
            .As<IEvaluator>()
            .FindConstructorsWith(new AllConstructorsFinder())
            .SingleInstance();

        return efCoreDataAccessOptions;
    }

    /// <summary>
    /// Adds a given custom evaluator that implements <see cref="IEvaluator"/> interface.
    /// </summary>
    /// <param name="efCoreDataAccessOptions">.</param>
    /// <param name="evaluator">Type of the custom evaluator.</param>
    /// <returns>Current <see cref="EfCoreDataAccessConfiguration"/> instance.</returns>
    public static EfCoreDataAccessConfiguration AddEvaluator(this EfCoreDataAccessConfiguration efCoreDataAccessOptions, Type evaluator)
    {
        if (evaluator.GetInterface(nameof(IEvaluator)) is null)
            throw new NotSupportedException("Registered evaluator did not implement IEvaluator interface");

        efCoreDataAccessOptions.Builder.RegisterType(evaluator)
            .As<IEvaluator>()
            .FindConstructorsWith(new AllConstructorsFinder())
            .SingleInstance();

        return efCoreDataAccessOptions;
    }

    /// <summary>
    /// Adds all evaluators that implement <see cref="IInMemoryEvaluator"/> from all assemblies.
    /// </summary>
    /// <returns>Current <see cref="EfCoreDataAccessConfiguration"/> instance.</returns>
    public static EfCoreDataAccessConfiguration AddInMemoryEvaluators(this EfCoreDataAccessConfiguration efCoreDataAccessOptions)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            efCoreDataAccessOptions.Builder.RegisterAssemblyTypes(assembly)
                .Where(x => x.GetInterface(nameof(IInMemoryEvaluator)) is not null)
                .As<IInMemoryEvaluator>()
                .FindConstructorsWith(new AllConstructorsFinder())
                .SingleInstance();
        }

        return efCoreDataAccessOptions;
    }

    /// <summary>
    /// Adds all validators that implement <see cref="IValidator"/> from all assemblies.
    /// </summary>
    /// <returns>Current <see cref="EfCoreDataAccessConfiguration"/> instance.</returns>
    public static EfCoreDataAccessConfiguration AddValidators(this EfCoreDataAccessConfiguration efCoreDataAccessOptions)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            efCoreDataAccessOptions.Builder.RegisterAssemblyTypes(assembly)
                .Where(x => x.GetInterface(nameof(IValidator)) is not null)
                .As<IValidator>()
                .FindConstructorsWith(new AllConstructorsFinder())
                .SingleInstance();
        }

        return efCoreDataAccessOptions;
    }

    /// <summary>
    /// Adds all evaluators that implement <see cref="IEvaluator"/> from all assemblies.
    /// </summary>
    /// <returns>Current <see cref="EfCoreDataAccessConfiguration"/> instance.</returns>
    public static EfCoreDataAccessConfiguration AddEvaluators(this EfCoreDataAccessConfiguration efCoreDataAccessOptions)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly == typeof(IncludeEvaluator).Assembly)
                continue;

            efCoreDataAccessOptions.Builder.RegisterAssemblyTypes(assembly)
                .Where(x => x.GetInterface(nameof(IEvaluator)) is not null)
                .As<IEvaluator>()
                .FindConstructorsWith(new AllConstructorsFinder())
                .SingleInstance();
        }

        return efCoreDataAccessOptions;
    }
}
