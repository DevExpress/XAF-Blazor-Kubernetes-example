using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using XAFContainerExample.Module.BusinessObjects;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.ExpressApp.SystemModule;

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

#if !RELEASE
        ApplicationUser sampleUser = ObjectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == "User");
        if(sampleUser == null) {
            sampleUser = ObjectSpace.CreateObject<ApplicationUser>();
            sampleUser.UserName = "User";
            // Set a password if the standard authentication type is used
            sampleUser.SetPassword("");

            // The UserLoginInfo object requires a user object Id (Oid).
            // Commit the user object to the database before you create a UserLoginInfo object. This will correctly initialize the user key property.
            ObjectSpace.CommitChanges(); //This line persists created object(s).
            ((ISecurityUserWithLoginInfo)sampleUser).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(sampleUser));
        }
        PermissionPolicyRole defaultRole = CreateDefaultRole();
        sampleUser.Roles.Add(defaultRole);

        ApplicationUser userAdmin = ObjectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == "Admin");
        if(userAdmin == null) {
            userAdmin = ObjectSpace.CreateObject<ApplicationUser>();
            userAdmin.UserName = "Admin";
            // Set a password if the standard authentication type is used
            userAdmin.SetPassword("");

            // The UserLoginInfo object requires a user object Id (Oid).
            // Commit the user object to the database before you create a UserLoginInfo object. This will correctly initialize the user key property.
            ObjectSpace.CommitChanges(); //This line persists created object(s).
            ((ISecurityUserWithLoginInfo)userAdmin).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(userAdmin));
        }
        // If a role with the Administrators name doesn't exist in the database, create this role
        PermissionPolicyRole adminRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == "Administrators");
        if(adminRole == null) {
            adminRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
            adminRole.Name = "Administrators";
        }
        adminRole.IsAdministrative = true;
        userAdmin.Roles.Add(adminRole);        
#endif

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
    private PermissionPolicyRole CreateDefaultRole() {
        PermissionPolicyRole defaultRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == "Default");
        if(defaultRole == null) {
            defaultRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
            defaultRole.Name = "Default";

            defaultRole.AddObjectPermissionFromLambda<ApplicationUser>(SecurityOperations.Read, cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            defaultRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
            defaultRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            defaultRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "StoredPassword", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Deny);
            defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);
        }
        return defaultRole;
    }
}
