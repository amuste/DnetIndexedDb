using DnetIndexedDb;
using Microsoft.JSInterop;

namespace DnetIndexedDbServer.Infrastructure
{
    public class GridColumnDataIndexedDb2 : IndexedDbInterop
    {
        public GridColumnDataIndexedDb2(IJSRuntime jsRuntime, IndexedDbOptions<GridColumnDataIndexedDb2> options) : base(jsRuntime, options)
        {
        }
    }
}
