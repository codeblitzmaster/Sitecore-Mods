using SitecoreMods.Foundation.Authorization.Enums;
using System.Collections.Generic;
using System;

namespace SitecoreMods.Foundation.Authorization.Models
{
    public class AuthorizationSettings
    {
        public Dictionary<string, string> AuthProperties { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public AuthorizationTypes AuthorizationType { get; set; }
    }
}