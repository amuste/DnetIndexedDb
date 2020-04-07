using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DnetIndexedDb
{
    public class IndexedDbOptions<TContext> : IndexedDbOptions where TContext : IndexedDbInterop
    {

        public IndexedDbOptions() : base(new Dictionary<Type, IIndexedDbOptionsExtension>())
        {
        }

        public IndexedDbOptions([NotNull] IReadOnlyDictionary<Type, IIndexedDbOptionsExtension> extensions) : base(extensions)
        {
        }

        public override IndexedDbOptions WithExtension<TExtension>(TExtension extension)
        {
            //Check.NotNull(extension, nameof(extension));

            var extensions = Extensions.ToDictionary(p => p.GetType(), p => p);
            extensions[typeof(TExtension)] = extension;

            return new IndexedDbOptions<TContext>(extensions);
        }
    }
}
