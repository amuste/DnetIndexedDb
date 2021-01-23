using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnetIndexedDb
{
    public class IndexDbKeyAttribute : Attribute
    {
        public string Name { get; set; }
        public bool AutoIncrement { get; set; } = false;
        public bool Unique { get; set; } = true;
    }
}
