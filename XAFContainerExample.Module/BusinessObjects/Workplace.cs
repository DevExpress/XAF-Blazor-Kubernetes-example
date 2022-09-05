using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System.ComponentModel;

namespace XAFContainerExample.Module.BusinessObjects {
    [DefaultProperty("Room")]
    public class Workplace : BaseObject {
        public Workplace(Session session) : base(session) { }

        private string room;

        public string Room {
            get => room;
            set => SetPropertyValue(nameof(Room), ref room, value);
        }
        private string computerId;
        public string ComputerId {
            get => computerId;
            set => SetPropertyValue(nameof(ComputerId), ref computerId, value);
        }
    }
}
