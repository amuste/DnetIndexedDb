using System.Collections.Generic;
using DnetIndexedDbServer.Infrastructure.Entities;

namespace DnetIndexedDbServer.Pages
{
    public class TableFieldService
    {
        public List<TableFieldDto> GetTableFields()
        {
            var items = new List<TableFieldDto>
            {
                new TableFieldDto
                {
                    TableFieldId = 11,
                    TableName = "Person",
                    FieldVisualName = "Id",
                    AttachedProperty = "Id",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = 12,
                    TableName = "Person",
                    FieldVisualName = "Index",
                    AttachedProperty = "Index",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = 13,
                    TableName = "Person",
                    FieldVisualName = "IsActive",
                    AttachedProperty = "IsActive",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = 14,
                    TableName = "Person",
                    FieldVisualName = "Balance",
                    AttachedProperty = "Balance",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = 15,
                    TableName = "Person",
                    FieldVisualName = "Picture",
                    AttachedProperty = "Picture",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = 16,
                    TableName = "Person",
                    FieldVisualName = "Age",
                    AttachedProperty = "Age",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                 new TableFieldDto
                {
                    TableFieldId = 17,
                    TableName = "Person",
                    FieldVisualName = "EyeColor",
                    AttachedProperty = "EyeColor",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = 18,
                    TableName = "Person",
                    FieldVisualName = "Name",
                    AttachedProperty = "Name",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = 19,
                    TableName = "Person",
                    FieldVisualName = "Gender",
                    AttachedProperty = "Gender",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = 20,
                    TableName = "Person",
                    FieldVisualName = "Company",
                    AttachedProperty = "Company",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = 21,
                    TableName = "Person",
                    FieldVisualName = "Email",
                    AttachedProperty = "Email",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = 22,
                    TableName = "Person",
                    FieldVisualName = "Phone",
                    AttachedProperty = "Phone",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                 new TableFieldDto
                {
                    TableFieldId = 23,
                    TableName = "Person",
                    FieldVisualName = "Address",
                    AttachedProperty = "Address",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                 new TableFieldDto
                {
                    TableFieldId = 24,
                    TableName = "Person",
                    FieldVisualName = "Registered",
                    AttachedProperty = "Registered",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = 25,
                    TableName = "Person",
                    FieldVisualName = "Latitude",
                    AttachedProperty = "Latitude",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = 26,
                    TableName = "Person",
                    FieldVisualName = "Longitude",
                    AttachedProperty = "Longitude",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = 27,
                    TableName = "Person",
                    FieldVisualName = "Greeting",
                    AttachedProperty = "Greeting",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = 28,
                    TableName = "Person",
                    FieldVisualName = "FavoriteFruit",
                    AttachedProperty = "FavoriteFruit",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                }
            };

            return items;
        }

        public List<TableFieldDto> GetNullIdTableFields()
        {
            var items = new List<TableFieldDto>
            {
                new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "Id",
                    AttachedProperty = "Id",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "Index",
                    AttachedProperty = "Index",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "IsActive",
                    AttachedProperty = "IsActive",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "Balance",
                    AttachedProperty = "Balance",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "Picture",
                    AttachedProperty = "Picture",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "Age",
                    AttachedProperty = "Age",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                 new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "EyeColor",
                    AttachedProperty = "EyeColor",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "Name",
                    AttachedProperty = "Name",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "Gender",
                    AttachedProperty = "Gender",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "Company",
                    AttachedProperty = "Company",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "Email",
                    AttachedProperty = "Email",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "Phone",
                    AttachedProperty = "Phone",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                 new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "Address",
                    AttachedProperty = "Address",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                 new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "Registered",
                    AttachedProperty = "Registered",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "Latitude",
                    AttachedProperty = "Latitude",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "Longitude",
                    AttachedProperty = "Longitude",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "Greeting",
                    AttachedProperty = "Greeting",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                },
                new TableFieldDto
                {
                    TableFieldId = null,
                    TableName = "Person",
                    FieldVisualName = "FavoriteFruit",
                    AttachedProperty = "FavoriteFruit",
                    IsLink = false,
                    MemberOf = 10,
                    Width = 200,
                    TextAlignClass = "",
                    Hide = false,
                    Type = ""
                }
            };

            return items;
        }


    }
}
