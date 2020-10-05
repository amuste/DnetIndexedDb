using System;
using System.Diagnostics.CodeAnalysis;
using DnetIndexedDb.Models;

namespace DnetIndexedDb
{
    public class IndexedDbOptionsBuilder<TContext> : IndexedDbOptionsBuilder where TContext : IndexedDbInterop
    {
        public IndexedDbOptionsBuilder([NotNull] IndexedDbOptions<TContext> options) : base(options)
        {
        }

        public new virtual IndexedDbOptions<TContext> Options => (IndexedDbOptions<TContext>)base.Options;

        public new virtual IndexedDbOptionsBuilder<TContext> UseDatabase([NotNull] IndexedDbDatabaseModel indexedDbDatabaseModel) => WithOption(e => e.UseDatabase(indexedDbDatabaseModel));

        public new virtual IndexedDbOptionsBuilder<TContext> UseApplicationServiceProvider(IServiceProvider serviceProvider)
            => (IndexedDbOptionsBuilder<TContext>)base.UseApplicationServiceProvider(serviceProvider);

        private IndexedDbOptionsBuilder<TContext> WithOption(Func<CoreOptionsExtension, CoreOptionsExtension> withFunc)
        {
            ((IIndexedDbOptionsBuilderInfrastructure)this).AddOrUpdateExtension(
                withFunc(Options.FindExtension<CoreOptionsExtension>() ?? new CoreOptionsExtension()));

            return this;
        }

    }
}
