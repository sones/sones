using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Result;

namespace sones.GraphDB.Interfaces
{
    public interface IQueryableLanguage
    {
        QueryResult Query(String myQueryScript, IGraphDBSession myGraphDBSession);
    }
}
