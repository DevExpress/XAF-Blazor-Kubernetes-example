using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAFContainerExample.Module.BusinessObjects {
    [DefaultClassOptions]
    public class StickyNote : BaseObject
    {
        public StickyNote(Session session) : base(session) { }

        private string name;
        public string Name {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }

        private string description;
        public string Description { 
            get => description;
            set => SetPropertyValue(nameof(Description), ref description, value);
        }
    }
}
