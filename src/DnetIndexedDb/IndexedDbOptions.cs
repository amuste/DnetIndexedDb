using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DnetIndexedDb
{
    public abstract class IndexedDbOptions : IIndexedDbOptions
    {

        private readonly IReadOnlyDictionary<Type, IIndexedDbOptionsExtension> _extensions;

        protected IndexedDbOptions([NotNull] IReadOnlyDictionary<Type, IIndexedDbOptionsExtension> extensions)
        {
            //Check.NotNull(extensions, nameof(extensions));

            _extensions = extensions;
        }

        public virtual IEnumerable<IIndexedDbOptionsExtension> Extensions => _extensions.Values;

        public virtual TExtension FindExtension<TExtension>() where TExtension : class, IIndexedDbOptionsExtension
            => _extensions.TryGetValue(typeof(TExtension), out var extension) ? (TExtension)extension : null;

        public virtual TExtension GetExtension<TExtension>() where TExtension : class, IIndexedDbOptionsExtension
        {
            var extension = FindExtension<TExtension>();
            if (extension == null)
            {
                throw new InvalidOperationException($"OptionsExtensionNotFound {typeof(TExtension)}");
            }

            return extension;
        }

        public abstract IndexedDbOptions WithExtension<TExtension>([NotNull] TExtension extension)
            where TExtension : class, IIndexedDbOptionsExtension;

       
    }
}
