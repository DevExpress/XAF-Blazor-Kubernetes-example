using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;

namespace XAFContainerExample.Module.BusinessObjects {
    public class Degree : BaseObject {
        public Degree(Session session) : base(session) { }

        private string name;
        public string Name {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }
    }
}
