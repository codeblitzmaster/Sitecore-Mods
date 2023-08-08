using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitecoreMods.Foundation.Authorization.Enums
{
    public enum AuthorizationTypes
    {
        None,
        ApiKey,
        Basic,
        OAuth2ClientCredentialsGrant,
        OAuth2PasswordCredentialsGrant,
    }
}