using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XAFContainerExample.Module.BusinessObjects {
    [DefaultClassOptions]
    public class Employee : Person {
        public Employee (Session session) : base (session) { }

        private Employee manager;

        [VisibleInListViewAttribute(false)]
        public Employee Manager {
            get => manager;
            set => SetPropertyValue(nameof(Manager), ref manager, value);
        }
        private Employee tutor;
        [VisibleInListViewAttribute(false)]
        public Employee Tutor {
            get => tutor;
            set => SetPropertyValue(nameof(Tutor), ref tutor, value);
        }

        private string notes;
        [Size(SizeAttribute.Unlimited)]
        public string Notes { 
            get => notes;
            set => SetPropertyValue(nameof(Notes), ref notes, value);
        }

        private Degree degree;
        public Degree Degree {
            get => degree;
            set => SetPropertyValue(nameof(Degree), ref degree, value);
        }

        private string githubProfile;
        public string GitHubProfile { 
            get => githubProfile;
            set => SetPropertyValue(nameof(GitHubProfile), ref githubProfile, value);
        }

        private string stackoverflowProfile;
        public string StackoverflowProfile {
            get => stackoverflowProfile;
            set => SetPropertyValue(nameof(StackoverflowProfile), ref stackoverflowProfile, value);
        }

        private string linkedinProfile;
        public string LinkedinProfile {
            get => linkedinProfile;
            set => SetPropertyValue(nameof(LinkedinProfile), ref linkedinProfile, value);
        }

        private Department department;
        public Department Department {
            get => department;
            set => SetPropertyValue(nameof(Department), ref department, value);
        }
        private Position position;
        public Position Position {
            get => position;
            set => SetPropertyValue(nameof(Position), ref position, value);
        }
        private Location location;
        public Location Location { 
            get => location;
            set => SetPropertyValue(nameof(Location), ref location, value);
        }
        private Workplace workplace;
        public Workplace Workplace {
            get => workplace;
            set => SetPropertyValue(nameof(Workplace), ref workplace, value);
        }
        private Level level;
        public Level Level {
            get => level;
            set => SetPropertyValue(nameof(Level), ref level, value);
        }
        private Project currentProject;
        public Project CurrentProject {
            get => currentProject;
            set => SetPropertyValue(nameof(CurrentProject), ref currentProject, value);
        }
    }
}
