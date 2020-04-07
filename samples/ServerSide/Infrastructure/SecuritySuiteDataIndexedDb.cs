using DnetIndexedDb;
using Microsoft.JSInterop;

namespace DnetIndexedDbServer.Infrastructure
{
    public class SecuritySuiteDataIndexedDb : IndexedDbInterop
    {
        public SecuritySuiteDataIndexedDb(IJSRuntime jsRuntime, IndexedDbOptions<SecuritySuiteDataIndexedDb> options) : base(jsRuntime, options)
        {
        }
    }
}
