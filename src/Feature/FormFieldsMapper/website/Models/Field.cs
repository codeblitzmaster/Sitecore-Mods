using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitecoreMods.Feature.FormFieldsMapper.Models
{
    /// <summary>
    /// Field Model for Mappings
    /// </summary>
    public class Field
    {
        public Guid? ID { get; set; }

        /// <summary>
        /// Destination Field Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// DestinationFieldDisplayName
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Destination Field Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Destination Field Type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Destination Field Is Required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Destination Field Is Primary
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Source Field Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Field Value Converter Type
        /// </summary>
        public string ValueConverterType { get; set; }

        /// <summary>
        /// Field Value Converter Type Params
        /// </summary>
        public string ValueConverterTypeParams { get; set; }
    }
}