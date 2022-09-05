using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using XAFContainerExample.Module.BusinessObjects;

namespace XAFContainerExample.Module.DatabaseUpdate;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
public class Updater : ModuleUpdater {
    private const int notesCount = 2000;
    private const int employeesCount = 20;

    public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
        base(objectSpace, currentDBVersion) {
    }
    public override void UpdateDatabaseAfterUpdateSchema() {
        base.UpdateDatabaseAfterUpdateSchema();

        Degree degree = ObjectSpace.FirstOrDefault<Degree>(d => d.Name == "Bachelor");

        if (degree == null) {
            degree = ObjectSpace.CreateObject<Degree>();
            degree.Name = "Bachelor";
        }

        for (int i = 0; i < employeesCount; i++) {
            string employeeName = string.Format("Employee {0}#", i);

            Employee employee = ObjectSpace.FirstOrDefault<Employee>(e => e.FirstName == employeeName);

            if (employee == null) {
                employee = ObjectSpace.CreateObject<Employee>();
                employee.FirstName = employeeName;

                if (i > 0)
                    employee.Manager = ObjectSpace.FirstOrDefault<Employee>(e => e.FirstName == string.Format("Employee {0}#", i - 1));

                employee.Tutor = employee.Manager;

                employee.Department = ObjectSpace.CreateObject<Department>();
                employee.Department.Name = String.Format("Department {0}", i);

                employee.Level = ObjectSpace.CreateObject<Level>();
                employee.Level.Name = String.Format("Level {0}", i);

                employee.Location = ObjectSpace.CreateObject<Location>();
                employee.Location.Name = String.Format("Location {0}", i);

                employee.Position = ObjectSpace.CreateObject<Position>();
                employee.Position.Name = String.Format("Position {0}", i);

                employee.Workplace = ObjectSpace.CreateObject<Workplace>();
                employee.Workplace.Room = String.Format("Room {0}", i);

                employee.CurrentProject = ObjectSpace.CreateObject<Project>();
                employee.CurrentProject.Name = String.Format("Project {0}", i);

                employee.Degree = degree;
            }
        }

        for (int i = 0; i < notesCount; i++) {
            string noteName = string.Format("Note {0}", i);
            string noteDescription = string.Format("This is sticky note {0}", i);

            StickyNote note = ObjectSpace.FirstOrDefault<StickyNote>(note => note.Name == noteName);

            if (note == null) {
                note = ObjectSpace.CreateObject<StickyNote>();
                note.Name = noteName;
                note.Description = noteDescription;
            }
        }

        ObjectSpace.CommitChanges();
    }
    public override void UpdateDatabaseBeforeUpdateSchema() {
        base.UpdateDatabaseBeforeUpdateSchema();
        //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
        //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
        //}
    }
}
