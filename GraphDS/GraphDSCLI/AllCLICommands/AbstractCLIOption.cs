/* Graph CLI
 * (c) Henning Rauch, 2009
 * 
 * Datatype for GraphCLI options
 * 
 * Lead programmer:
 *      Henning Rauch
 * 
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.GraphDS.Connectors.CLI
{

    public class AbstractCLIOption
    {

        #region Data

        private String _Option = "";
        private List<String> _Parameters = new List<string>();

        #endregion

        #region Constructors

        public AbstractCLIOption(String _OptionName)
        {
            _Option = _OptionName;
        }

        #endregion

        #region Public methods

        public String Option
        {
            get
            {
                return _Option;
            }
        }

        public List<String> Parameters
        {
            get
            {
                return _Parameters;
            }
        }

        public void AddParameter(string _Parameter)
        {
            _Parameters.Add(_Parameter);
        }

        public int Pos { get; set; }

        public int EndPos { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        #endregion

    }

}
