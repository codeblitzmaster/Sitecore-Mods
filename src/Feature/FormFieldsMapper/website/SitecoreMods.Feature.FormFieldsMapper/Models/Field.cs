using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitecoreMods.Feature.FormFieldsMapper.Models
{
    public class Field
    {
        public Guid? ID { get; set; }
        public string SourceFieldValue { get; set; }
        public string DestinationFieldName { get; set; }
        public string DestinationFieldDisplayName { get; set; }
        public string DestinationFieldDescription { get; set; }
        public string DestinationFieldType { get; set; }
        public bool DestinationFieldIsRequired { get; set; }
        public bool DestinationFieldIsPrimary { get; set; }
    }
}