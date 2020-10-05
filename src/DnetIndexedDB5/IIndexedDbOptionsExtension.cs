using DnetIndexedDb.Models;

namespace DnetIndexedDb
{
    public interface IIndexedDbOptionsExtension
    {
        IndexedDbDatabaseModel IndexedDbDatabaseModel { get; }
    }
}
