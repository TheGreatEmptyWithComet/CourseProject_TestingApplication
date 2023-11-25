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

    public class TestResultsVM : NotifyPropertyChangedHandler
    {
        #region Properties
        /****************************************************************************************/
        private readonly Context context;

        public ObservableCollection<TestVM> Tests
        {
            get
            {
                if (CurrentTestCategory != null)
                {
                    return new ObservableCollection<TestVM>(context.Tests.Where((t) => t.TestCategory == CurrentTestCategory.Model).Select(i => new TestVM(i)));
                }
                else
                {
                    return new ObservableCollection<TestVM>();
                }
            }
        }
        public ObservableCollection<UserGroupVM> UserGroups
        {
            get
            {
                if (CurrentTestCategory != null)
                {
                    // Reurn user groups only from issued tests
                    return new ObservableCollection<UserGroupVM>(context.IssuedTests.Where((t) => t.Test == CurrentTest.Model).Select(t => new UserGroupVM(t.UserGroup)));
                }
                else
                {
                    return new ObservableCollection<UserGroupVM>();
                }
            }
        }

        public ObservableCollection<ShortResultVM> ShortResults
        {
            get
            {
                if (CurrentUserGroup != null)
                {
                    return new ObservableCollection<ShortResultVM>(context.ShortResults.Where((r) => r.Test == CurrentTest.Model && r.User.UserGroup == CurrentUserGroup.Model).Select(i => new ShortResultVM(i)));
                }
                else
                {
                    return new ObservableCollection<ShortResultVM>();
                }
            }
        }

        private TestCategoryVM currentTestCategory;
        public TestCategoryVM CurrentTestCategory
        {
            get => currentTestCategory;
            set
            {
                if (value != null)
                {
                    currentTestCategory = value;
                    NotifyPropertyChanged(nameof(Tests));
                    // Clear issued tests list:
                    CurrentTest = new TestVM(new Test());
                }
            }
        }

        private TestVM currentTest;
        public TestVM CurrentTest
        {
            get => currentTest;
            set
            {
                if (value != null)
                {
                    currentTest = value;
                    NotifyPropertyChanged(nameof(UserGroups));
                    NotifyPropertyChanged(nameof(CurrentTest));
                }
            }
        }
        private UserGroupVM currentUserGroup;
        public UserGroupVM CurrentUserGroup
        {
            get => currentUserGroup;
            set
            {
                if (value != null)
                {
                    currentUserGroup = value;
                    NotifyPropertyChanged(nameof(CurrentUserGroup));
                    NotifyPropertyChanged(nameof(ShortResults));
                }
            }
        }

        #endregion



        #region Constructor
        /****************************************************************************************/
        public TestResultsVM(Context context)
        {
            this.context = context;

            // Load data from DB
            context.UserGroups.Include((ug) => ug.Users).Load();

        }
        #endregion

    }
}

