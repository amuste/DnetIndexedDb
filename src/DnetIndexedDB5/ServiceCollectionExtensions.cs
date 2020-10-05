using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DnetIndexedDb
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIndexedDbDatabase<TDatabase>(
            this IServiceCollection serviceCollection,
            Action<IndexedDbOptionsBuilder> options,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped
            ) where TDatabase : IndexedDbInterop
            => AddIndexedDbDatabase<TDatabase, TDatabase>(serviceCollection, options, contextLifetime);

        public static IServiceCollection AddIndexedDbDatabase<TContextService, TContextImplementation>(
            [NotNull] this IServiceCollection serviceCollection,
            Action<IndexedDbOptionsBuilder> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TContextImplementation : IndexedDbInterop, TContextService
            => AddIndexedDbDatabase<TContextService, TContextImplementation>(
                serviceCollection,
                optionsAction == null
                    ? (Action<IServiceProvider, IndexedDbOptionsBuilder>)null
                    : (p, b) => optionsAction.Invoke(b), contextLifetime, optionsLifetime);

        public static IServiceCollection AddIndexedDbDatabase<TContextService, TContextImplementation>(
            [NotNull] this IServiceCollection serviceCollection,
            Action<IServiceProvider, IndexedDbOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TContextImplementation : IndexedDbInterop, TContextService
        {
            //Check.NotNull(serviceCollection, nameof(serviceCollection));

            if (contextLifetime == ServiceLifetime.Singleton)
            {
                optionsLifetime = ServiceLifetime.Singleton;
            }

            if (optionsAction != null)
            {
                CheckContextConstructors<TContextImplementation>();
            }

            AddCoreServices<TContextImplementation>(serviceCollection, optionsAction, optionsLifetime);

            var serviceDescriptor = new ServiceDescriptor(typeof(TContextService), typeof(TContextImplementation), contextLifetime);

            serviceCollection.TryAdd(serviceDescriptor);

            return serviceCollection;
        }

        private static void AddCoreServices<TContextImplementation>(
            IServiceCollection serviceCollection,
            Action<IServiceProvider, IndexedDbOptionsBuilder> optionsAction,
            ServiceLifetime optionsLifetime)
            where TContextImplementation : IndexedDbInterop
        {
            var serviceDescriptor = new ServiceDescriptor(typeof(IndexedDbOptions<TContextImplementation>),
                p => CreateDbContextOptions<TContextImplementation>(p, optionsAction),
                optionsLifetime);

            serviceCollection.TryAdd(serviceDescriptor);
        }

        private static void CheckContextConstructors<TContext>()
            where TContext : IndexedDbInterop
        {
            var declaredConstructors = typeof(TContext).GetTypeInfo().DeclaredConstructors.ToList();
            if (declaredConstructors.Count == 1 && declaredConstructors[0].GetParameters().Length == 0)
            {
                throw new ArgumentException($"DbContextMissingConstructor{typeof(TContext)}");
            }
        }

        private static IndexedDbOptions<TContext> CreateDbContextOptions<TContext>(
            IServiceProvider applicationServiceProvider,
            Action<IServiceProvider, IndexedDbOptionsBuilder> optionsAction)
            where TContext : IndexedDbInterop
        {
            var builder = new IndexedDbOptionsBuilder<TContext>(
                  new IndexedDbOptions<TContext>(new Dictionary<Type, IIndexedDbOptionsExtension>()));

            builder.UseApplicationServiceProvider(applicationServiceProvider);

            optionsAction?.Invoke(applicationServiceProvider, builder);

            return builder.Options;
        }

    }
}
