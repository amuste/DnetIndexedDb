using System;
using System.Diagnostics.CodeAnalysis;
using DnetIndexedDb.Models;

namespace DnetIndexedDb
{
    public class IndexedDbOptionsBuilder : IIndexedDbOptionsBuilderInfrastructure
    {
        private IndexedDbOptions _options;

        public IndexedDbOptionsBuilder([NotNull] IndexedDbOptions options)
        {
            //Check.NotNull(options, nameof(options));

            _options = options;
        }

        public virtual IndexedDbOptions Options => _options;

        public virtual IndexedDbOptionsBuilder UseDatabase([NotNull] IndexedDbDatabaseModel indexedDbDatabaseModel) => WithOption(e => e.UseDatabase(indexedDbDatabaseModel));

        void IIndexedDbOptionsBuilderInfrastructure.AddOrUpdateExtension<TExtension>(TExtension extension)
        {
            //Check.NotNull(extension, nameof(extension));

            _options = _options.WithExtension(extension);
        }

        private IndexedDbOptionsBuilder WithOption(Func<CoreOptionsExtension, CoreOptionsExtension> withFunc)
        {
            ((IIndexedDbOptionsBuilderInfrastructure)this).AddOrUpdateExtension(withFunc(Options.FindExtension<CoreOptionsExtension>() ?? new CoreOptionsExtension()));

            return this;
        }

        public virtual IndexedDbOptionsBuilder UseApplicationServiceProvider(IServiceProvider serviceProvider)
            => WithOption(e => e.WithApplicationServiceProvider(serviceProvider));

    }
}
