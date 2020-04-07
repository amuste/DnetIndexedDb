using System.Collections.Generic;

namespace DnetIndexedDb
{
    public interface IIndexedDbOptions
    {
        IEnumerable<IIndexedDbOptionsExtension> Extensions { get; }

        TExtension FindExtension<TExtension>() where TExtension : class, IIndexedDbOptionsExtension;
    }
}
