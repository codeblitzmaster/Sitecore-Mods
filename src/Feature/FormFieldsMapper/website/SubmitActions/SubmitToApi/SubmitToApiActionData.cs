using SitecoreMods.Feature.FormFieldsMapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitecoreMods.Feature.FormFieldsMapper.SubmitActions.SubmitToApi
{
    public class SubmitToApiActionData
    {
        public string ApiEndpointId { get; set; }

        public IEnumerable<Field> Mappings { get; set; }
    }
}