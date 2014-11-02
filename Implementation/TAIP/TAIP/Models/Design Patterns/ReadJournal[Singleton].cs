using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAIP.Models.Design_Patterns
{
    public class ReadJournal_Singleton_
    {
        public class ReadJournal
        {
            private static ReadJournal self;
            private string name;

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            //hide the default constructor
            private ReadJournal() { }

            public static ReadJournal Instance()
            {
                //create instance only if we don't have one
                if (self == null)
                    self = new ReadJournal();
                return self;
            }
        }
    }
}