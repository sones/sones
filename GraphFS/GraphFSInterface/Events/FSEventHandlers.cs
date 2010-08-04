using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphFS.Events
{

    public class FSEventHandlers
    {

        public delegate void OnLoadEventHandler    (Object mySender, EventArgs myEventArgs);
        public delegate void OnLoadedEventHandler  (Object mySender, EventArgs myEventArgs);
        public delegate void OnSaveEventHandler    (Object mySender, EventArgs myEventArgs);
        public delegate void OnSavedEventHandler   (Object mySender, EventArgs myEventArgs);
        public delegate void OnRemoveEventHandler  (Object mySender, EventArgs myEventArgs);
        public delegate void OnRemovedEventHandler (Object mySender, EventArgs myEventArgs);

    }

}
