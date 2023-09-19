using DevExpress.Persistent.BaseImpl.EF;
using System;

namespace XAFContainerExample.Module.BusinessObjects {
    public class Degree : BaseObject {
        public virtual string Name { get; set; }
    }
}
