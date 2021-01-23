using DnetIndexedDb;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DnetIndexedDbServer.Infrastructure.Entities
{
    public class TableFieldDto
    {
        [IndexDbKey(AutoIncrement = true)]
        public int? TableFieldId { get; set; }
        [IndexDbIndex]
        public string TableName { get; set; }
        [IndexDbIndex]
        public string FieldVisualName { get; set; }
        [IndexDbIndex]
        public string AttachedProperty { get; set; }
        [IndexDbIndex]
        public bool IsLink { get; set; }
        [IndexDbIndex]
        public int MemberOf { get; set; }
        [IndexDbIndex]
        public int Width { get; set; }
        [IndexDbIndex]
        public string TextAlignClass { get; set; }
        [IndexDbIndex]
        public bool Hide { get; set; }
        [IndexDbIndex]
        public string Type { get; set; }
    }
}
