using DnetIndexedDb;
using Microsoft.JSInterop;

namespace DnetIndexedDbServer.Infrastructure
{
    public class GridColumnDataIndexedDb : IndexedDbInterop
    {
        public GridColumnDataIndexedDb(IJSRuntime jsRuntime, IndexedDbOptions<GridColumnDataIndexedDb> options) : base(jsRuntime, options)
        {
        }
    }
}
