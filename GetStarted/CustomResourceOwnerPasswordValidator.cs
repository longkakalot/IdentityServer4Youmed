using IdentityModel;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetStarted
{
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            //Validate user's username and password. Insert your logic here.
            if (context.UserName == "tmhtp-youmed" && context.Password == "tmhtp-youmed2021")
                context.Result = new GrantValidationResult("123", OidcConstants.AuthenticationMethods.Password);

            return Task.FromResult(0);
        }
    }
}
