/*
 * RESTCLI_StartREST
 * Achim Friedland, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphFS.Session;
using sones.GraphFS.Connectors.GraphDSCLI;
using sones.Networking.HTTP;
using sones.GraphDS.Connectors.REST;
using sones.Lib.ErrorHandling;
using sones.GraphFS.Errors;
using sones.GraphDS.API.CSharp;
using System.ServiceModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

#endregion

namespace sones.GraphDS.Connectors.CLI
{

    /// <summary>
    /// Information on this GraphDS instance
    /// </summary>
    public class RESTCLI_StartREST : AllGraphDSRESTCLICommands
    {

        #region PassValidator

        // Include: System.IdentityModel
        public class PassValidator : UserNamePasswordValidator
        {
            public override void Validate(String myUserName, String myPassword)
            {
                if (!(myUserName == "test" && myPassword == "test") && !(myUserName == "test2" && myPassword == "test2"))
                {
                    throw new SecurityTokenException("Unknown Username or Password");
                }
            }
        }

        #endregion


        #region Constructor

        public RESTCLI_StartREST()
        {

            // Command name and description
            InitCommand("STARTREST",
                        "Start the GraphDSREST interface.",
                        "Start the GraphDSREST interface.");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal);

        }

        #endregion

        #region Execute Command

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));


            var _HttpSecurity = new HTTPSecurity()
            {
                // Include: System.ServiceModel
                CredentialType            = HttpClientCredentialType.Basic,
                UserNamePasswordValidator = new PassValidator()
            };

            // Initialize REST service
            var _HttpWebServer = new HTTPServer<GraphDSREST_Service>(
                9975,
                new GraphDSREST_Service(myAGraphDSSharp),
                myAutoStart: true)
            {
                HTTPSecurity = _HttpSecurity,
            };


            // Register the REST service within the list of services
            // to stop before shutting down the GraphDSSharp instance
            myAGraphDSSharp.ShutdownEvent += new GraphDSSharp.ShutdownEventHandler((o, e) =>
            {
                _HttpWebServer.StopAndWait();
            });

            return Exceptional.OK;

        }

        #endregion

    }

}
