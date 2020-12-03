using System.ComponentModel.DataAnnotations;

namespace DnetIndexedDbServer.Infrastructure.Entities
{
    public class TableFieldDto
    {
        [Key]
        public int? TableFieldId { get; set; }

        public string TableName { get; set; }

        public string FieldVisualName { get; set; }

        public string AttachedProperty { get; set; }

        public bool IsLink { get; set; }

        public int MemberOf { get; set; }

        public int Width { get; set; }

        public string TextAlignClass { get; set; }

        public bool Hide { get; set; }

        public string Type { get; set; }
    }
}
