using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Manager.TypeManagement;
using sones.GraphDB.Request;
using sones.Library.LanguageExtensions;
using System.Collections;

namespace sones.GraphDB.TypeManagement.Command
{
    
    /// <summary>
    /// A class that represents an add vertex command.
    /// </summary>
    public sealed class AddVertexTypeCommand: ATypeManagerCommand
    {
        public AddVertexTypeCommand(VertexTypeDefinition myVertexTypeDefinition):this(myVertexTypeDefinition.SingleEnumerable())
        {

        }

        public AddVertexTypeCommand(IEnumerable<VertexTypeDefinition> myVertexTypeDefinition)
        {

        }


    }
}
