using System.Text.RegularExpressions;
using SitecoreMods.Feature.FormFieldsMapper.FieldValueConverters.Abstractions;

namespace SitecoreMods.Feature.FormFieldsMapper.FieldValueConverters
{
    public class RegularExpressionConverter : FieldValueConverterBase<RegularExpressionParameters>
    {
        public RegularExpressionConverter(string parameters) : base(parameters)
        {
        }

        public override object ConvertFieldValue(object fieldValue)
        {
            var value = Regex.Replace(fieldValue.ToString(), Parameters.RegularExpression, Parameters.Replacement);
            return value;
        }

        
    }
}