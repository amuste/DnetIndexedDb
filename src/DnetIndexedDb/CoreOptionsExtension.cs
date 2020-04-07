using System;
using System.Diagnostics.CodeAnalysis;
using DnetIndexedDb.Models;

namespace DnetIndexedDb
{
    public class CoreOptionsExtension : IIndexedDbOptionsExtension
    {
        private IServiceProvider _applicationServiceProvider;

        private IndexedDbDatabaseModel _indexedDbDatabaseModel;

        public CoreOptionsExtension()
        {
        }

        protected CoreOptionsExtension([NotNull] CoreOptionsExtension copyFrom)
        {
            _applicationServiceProvider = copyFrom.ApplicationServiceProvider;
        }

        public virtual IndexedDbDatabaseModel IndexedDbDatabaseModel => _indexedDbDatabaseModel;

        protected virtual CoreOptionsExtension Clone() => new CoreOptionsExtension(this);

        public virtual CoreOptionsExtension UseDatabase(IndexedDbDatabaseModel indexedDbDatabaseModel)
        {
            var clone = Clone();

            clone._indexedDbDatabaseModel = indexedDbDatabaseModel;

            return clone;
        }

        public virtual IServiceProvider ApplicationServiceProvider => _applicationServiceProvider;

        public virtual CoreOptionsExtension WithApplicationServiceProvider(IServiceProvider applicationServiceProvider)
        {
            var clone = Clone();

            clone._applicationServiceProvider = applicationServiceProvider;

            return clone;
        }

    }
   
}
