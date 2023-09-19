using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using System.ComponentModel;

namespace XAFContainerExample.Module.BusinessObjects {
    [DefaultProperty("Room")]
    public class Workplace : BaseObject {

        public virtual string Room { get; set; }

        public virtual string ComputerId { get; set; }
    }
}
