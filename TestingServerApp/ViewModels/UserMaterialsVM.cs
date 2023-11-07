using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using TestingServerApp.Viewes;

namespace TestingServerApp
{
    public class UserMaterialsVM : NotifyPropertyChangeHandler
    {
        #region Properties
        /****************************************************************************************/
        private readonly Context context;
        private UserDataWindow UserDataWindow;

        // "Administrator" group isn't shown in the app. It only separates users:
        public ObservableCollection<UserGroupVM> UserGroups
        {
            get => new ObservableCollection<UserGroupVM>(context.UserGroups.Where((u) => u.Name != "Administrator").Select(i => new UserGroupVM(i)));
        }
        // Administrators - all users from according group:
        public ObservableCollection<UserVM> Administrators
        {
            get => new ObservableCollection<UserVM>(context.Users.Where((u) => u.UserGroup.Name == "Administrator").Select(i => new UserVM(i)));
        }
        // As far as "Administrators" group isn't shown in the application, it can't be selected and admin users can't be shown as Students:
        private List<User> allStudents;
        public ObservableCollection<UserVM> Students
        {
            get => new ObservableCollection<UserVM>(allStudents.Select(i => new UserVM(i)));
        }

        // Entities for data binding
        public UserVM CurrentUser { get; set; }
        public UserGroupVM CurrentUserGroup { get; set; }

        // Error message for data window
        private string errorMessage;
        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                NotifyPropertyChanged(nameof(ErrorMessage));
            }
        }
        #endregion

        #region Commands
        /****************************************************************************************/
        public ICommand AddAdminUserCommand { get; private set; }
        public ICommand AddOtherUserCommand { get; private set; }
        public ICommand EditUserCommand { get; private set; }
        public ICommand DeleteUserCommand { get; private set; }
        public ICommand SaveUserCommand { get; private set; }

        public ICommand AddGroupCommand { get; private set; }
        public ICommand EditGroupCommand { get; private set; }
        public ICommand DeleteGroupCommand { get; private set; }
        public ICommand SaveGroupCommand { get; private set; }

        #endregion


        #region Constructor
        /****************************************************************************************/
        public UserMaterialsVM(Context context)
        {
            this.context = context;

            InitCommands();

            // Load data from DB
            context.UserGroups.Include((ug) => ug.Users).Load();
            allStudents = context.Users.Where((u) => u.UserGroup.Name != "Administrator").ToList();
        }
        #endregion


        #region Methods
        /****************************************************************************************/
        private void InitCommands()
        {
            AddAdminUserCommand = new RelayCommand(AddAdminUser);
            AddOtherUserCommand = new RelayCommand(AddUser);
            EditUserCommand = new RelayCommand(EditUser);
            DeleteUserCommand = new RelayCommand(DeleteUser);
            SaveUserCommand = new RelayCommand(CheckUserData);
        }

        private void loadDataFromDb()
        {
            context.UserGroups.Include((ug) => ug.Users).Load();
        }

        private void SetCurrentUserGroupAsAdmin()
        {
            CurrentUserGroup = new UserGroupVM(context.UserGroups.Where((ug) => ug.Name == "Administrator").FirstOrDefault()!);
        }

        private void AddAdminUser()
        {
            SetCurrentUserGroupAsAdmin();
            AddUser();
        }
        private void AddUser()
        {
            // Create new entity
            User newUser = new User()
            {
                UserGroup = CurrentUserGroup.Model
            };
            CurrentUser = new UserVM(newUser);
            ErrorMessage = string.Empty;

            //Create and show data window
            UserDataWindow = new UserDataWindow();
            UserDataWindow.Owner = Application.Current.MainWindow;

            if (UserDataWindow.ShowDialog() == true)
            {
                context.Add(newUser);
                SaveChanges();
            }
        }

        private void EditUser()
        {
            ErrorMessage = string.Empty;

            //Create and show window
            UserDataWindow = new UserDataWindow();
            UserDataWindow.Owner = Application.Current.MainWindow;

            if (UserDataWindow.ShowDialog() == true)
            {
                SaveChanges();
            }
            else
            {
                // Discard changes
                Context.DiscardChanges<User>(context);
            }
        }

        private void DeleteUser()
        {
            var result = MessageBox.Show("Delete test ctaegory?\r\nAll related tests will be deleted!\r\rData recovery will be impossible", "Attention!", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {

                context.Remove(CurrentUser.Model);

                SaveChanges();
            }
        }

        private void CheckUserData()
        {
            // Login must not be empty
            if (string.IsNullOrEmpty(CurrentUser.Login))
            {
                ErrorMessage = "Error: login must not be empty";
                return;
            }
            // Login must be unique
            else if (context.Users.Any((user) => user.Login == CurrentUser.Login && user.Id != CurrentUser.Id))
            {
                ErrorMessage = "Error: login must be unique";
                return;
            }
            // Email must not be empty
            if (string.IsNullOrEmpty(CurrentUser.Email))
            {
                ErrorMessage = "Error: email must not be empty";
                return;
            }
            // Pussword must not be empty
            if (string.IsNullOrEmpty(CurrentUser.PasswordHash))
            {
                ErrorMessage = "Error: pussword must not be empty";
                return;
            }

            UserDataWindow.DialogResult = true;
            UserDataWindow.Close();
        }

        private void SaveChanges()
        {
            try
            {
                context.SaveChanges();
                NotifyPropertyChanged(nameof(Administrators));
            }
            catch (Exception ex)
            {
                string innerMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                MessageBox.Show(ex.Message + "\n" + innerMessage, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion


    }
}
