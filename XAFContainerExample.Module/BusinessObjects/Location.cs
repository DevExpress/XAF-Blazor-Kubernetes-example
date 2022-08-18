using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XAFContainerExample.Module.BusinessObjects {
    [DefaultClassOptions]
    public class Location : BaseObject {
        public Location(Session session) : base(session) { }

        private string name;
        public string Name {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }

        private string country;
        public string Country {
            get => country;
            set => SetPropertyValue(nameof(Country), ref country, value);
        }
        private string state;
        public string State {
            get => state;
            set => SetPropertyValue(nameof(State), ref state, value);
        }
        private string city;
        public string City {
            get => city;
            set => SetPropertyValue(nameof(City), ref city, value);
        }
    }
}
