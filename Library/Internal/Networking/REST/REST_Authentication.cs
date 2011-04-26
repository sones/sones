
#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

#endregion

namespace sones.Networking.REST
{

    public class REST_Authentication : UserNamePasswordValidator
    {

        public override void Validate(String myUserName, String myPassword)
        {

            Console.WriteLine("Implement authentication for " + myUserName);

            if (String.IsNullOrEmpty(myUserName) || String.IsNullOrEmpty(myPassword))
                throw new SecurityTokenException("Validation Failed!");

        }

    }

}
