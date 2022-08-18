using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XAFContainerExample.Module.BusinessObjects {
    [DefaultClassOptions]
    public class Position : BaseObject {
        public Position(Session session) : base(session) { }

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
