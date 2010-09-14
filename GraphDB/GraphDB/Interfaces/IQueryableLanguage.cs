using System;
using sones.GraphDB.Result;

namespace sones.GraphDB.Interfaces
{
    public interface IQueryableLanguage
    {
        QueryResult Query(String myQueryScript, IGraphDBSession myGraphDBSession);
    }
}
