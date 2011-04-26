using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace sones.Networking.HTTP
{

    public class HTTPSecurity
    {

        private HttpClientCredentialType _CredentialType;
        public HttpClientCredentialType CredentialType
        {
            get { return _CredentialType; }
            set { _CredentialType = value; }
        }

        private UserNamePasswordValidator _UserNamePasswordValidator;
        public UserNamePasswordValidator UserNamePasswordValidator
        {
            get { return _UserNamePasswordValidator; }
            set { _UserNamePasswordValidator = value; }
        }

        #region SSL

        public X509Certificate2 ServerCertificate { get; set; }
        public RemoteCertificateValidationCallback RemoteCertificateValidationCallback { get; set; }
        public LocalCertificateSelectionCallback LocalCertificateSelectionCallback { get; set; }
        public EncryptionPolicy EncryptionPolicy { get; set; }

        private Boolean _UseClientCertificate = false;
        public Boolean UseClientCertificate 
        {
            get { return _UseClientCertificate; }
            set { _UseClientCertificate = value; }
        }

        //public Boolean RemoteCertificateValidationEvent(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        //{
        //    if (OnRemoteCertificateValidation != null)
        //    {
        //        return OnRemoteCertificateValidation(sender, certificate, chain, sslPolicyErrors);
        //    }
        //    return false;
        //}


        #endregion

    }

    //public delegate Boolean RemoteCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors);

    //public delegate void RemoteCertificateValidation(object sender, RemoteCertificateValidationEventArgs eventArgs);
    public class RemoteCertificateValidationEventArgs
    {
        public X509Certificate2 certificate { get; set; }
        public X509Chain chain { get; set; }
        public SslPolicyErrors sslPolicyErrors { get; set; }
    }

}
