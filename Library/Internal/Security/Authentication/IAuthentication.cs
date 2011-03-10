namespace sones.Library.Security
{
    /// <summary>
    /// Authentication interface
    /// User and GraphElement authentication
    /// </summary>
    public interface IAuthentication : IUserAuthentication, IGraphElementAuthentication
    {
    }
}