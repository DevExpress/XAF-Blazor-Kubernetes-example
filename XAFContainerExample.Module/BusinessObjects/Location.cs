using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;

namespace XAFContainerExample.Module.BusinessObjects {
    [DefaultClassOptions]
    public class Location : BaseObject {
        public virtual string Name { get; set; }

        public virtual string Country { get; set; }

        public virtual string State { get; set; }

        public virtual string City { get; set; }
    }
}
