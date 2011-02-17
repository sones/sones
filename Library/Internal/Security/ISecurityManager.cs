namespace sones.Security
{
    /// <summary>
    /// The interface for all security managers
    /// Authentication & integrity & encryption
    /// </summary>
    public interface ISecurityManager : IAuthentication, IIntegrity, IEncryption
    {
    }
}