using System.Diagnostics.CodeAnalysis;

namespace DnetIndexedDb
{
    public interface IIndexedDbOptionsBuilderInfrastructure
    {
        void AddOrUpdateExtension<TExtension>([NotNull] TExtension extension) where TExtension : class, IIndexedDbOptionsExtension;
    }
}
