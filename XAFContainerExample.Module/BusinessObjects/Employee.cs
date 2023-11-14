using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using System.ComponentModel.DataAnnotations;

namespace XAFContainerExample.Module.BusinessObjects {
    [DefaultClassOptions]
    public class Employee : Person {
        [VisibleInListViewAttribute(false)]
        public virtual Employee Manager { get; set; }

        [StringLength(4096)]
        public virtual string Notes { get; set; }

        public virtual Degree Degree { get; set; }

        public virtual string GitHubProfile { get; set; }

        public virtual string StackoverflowProfile { get; set; }

        public virtual string LinkedinProfile { get; set; }

        public virtual Department Department { get; set; }

        public virtual Position Position { get; set; }

        public virtual Location Location { get; set; }

        public virtual Workplace Workplace { get; set; }

        public virtual Level Level { get; set; }

        public virtual Project CurrentProject { get; set; }
    }
}
