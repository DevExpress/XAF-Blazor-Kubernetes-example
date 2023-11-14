
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using System.Text.Json.Serialization;

namespace XAFContainerExample.Module.BusinessObjects {
    [DefaultProperty(nameof(FullName))]
    [ImageName("BO_Person")]
    public class Person : BaseObject {

        public virtual String FirstName { get; set; }

        public virtual String LastName { get; set; }

        public virtual String MiddleName { get; set; }

        public virtual DateTime? Birthday { get; set; }

        [FieldSize(255)]
        public virtual String Email { get; set; }

        [SearchMemberOptions(SearchMemberMode.Exclude)]
        [JsonIgnore]
        public String FullName {
            get { return ObjectFormatter.Format(FullNameFormat, this, EmptyEntriesMode.RemoveDelimiterWhenEntryIsEmpty); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonIgnore]
        public virtual String DisplayName {
            get { return FullName; }
        }

        public static String FullNameFormat = "{FirstName} {MiddleName} {LastName}";
    }
}