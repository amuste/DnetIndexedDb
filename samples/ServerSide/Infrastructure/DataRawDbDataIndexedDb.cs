using DnetIndexedDb;
using Microsoft.JSInterop;

namespace DnetIndexedDbServer.Infrastructure
{
    public class DataRawDbDataIndexedDb : IndexedDbInterop
    {
        public DataRawDbDataIndexedDb(IJSRuntime jsRuntime, IndexedDbOptions<DataRawDbDataIndexedDb> options) : base(jsRuntime, options)
        {
        }
    }
}
