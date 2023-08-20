namespace SitecoreMods.Feature.FormFieldsMapper.FieldValueConverters.Abstractions
{
    public interface IFieldValueConverter
    {
        object ConvertFieldValue(object fieldValue);
    }
}