
#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.Lib.Frameworks.CLIrony.Compiler.Lalr;

#endregion

namespace sones.GraphDS.Connectors.CLI
{

    public class AcInformation
    {

        #region Data

        private List<String>    _AcLines;
        private ParserReturn    _AcMetadata;
        private String          _ToBeAced;
        private int             _IndexOfLastDelimiter;

        #endregion

        #region Properties

        public List<String>     AcLines                 { get { return _AcLines; } }
        public ParserReturn     AcMetadata              { get { return _AcMetadata; } }
        public String           ToBeAced                { get { return _ToBeAced; } }
        public int              IndexOfLastDelimiter    { get { return _IndexOfLastDelimiter; } }

        #endregion

        #region Constructor

        public AcInformation(List<String> AcLines, ParserReturn AcMetadata, String ToBeAced, int IndexOfLastDelimiter)
        {
            _AcLines                = AcLines;
            _AcMetadata             = AcMetadata;
            _ToBeAced               = ToBeAced;
            _IndexOfLastDelimiter   = IndexOfLastDelimiter;
        }

        #endregion

    }

}
