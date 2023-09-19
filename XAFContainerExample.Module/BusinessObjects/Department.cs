using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;

namespace XAFContainerExample.Module.BusinessObjects {
    [DefaultClassOptions]
    public class Department : BaseObject {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }
    }
}
