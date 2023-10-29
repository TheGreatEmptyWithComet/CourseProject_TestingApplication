using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestingServerApp
{
    public class BaseVM: NotifyPropertyChangeHandler
    {
        #region Properties
        /****************************************************************************************/
        private readonly Context context;

        private string currentPage;
        public string CurrentPage
        {
            get { return currentPage; }
            set
            {
                currentPage = value;
                NotifyPropertyChanged(nameof(CurrentPage));
            }
        }
        #endregion


        #region Inner view models
        /****************************************************************************************/
        public TestCategoriesVM TestCategoriesVM { get; private set; }
        public TestMaterialsVM TestMaterialsVM { get; private set; }
        #endregion


        #region Commands
        /****************************************************************************************/
        public ICommand PageNavigationCommand { get; private set; }

        #endregion


        #region Constructor
        /****************************************************************************************/
        public BaseVM()
        {
            context = new Context();
            
            // Set the start page
            CurrentPage = "UsersMenuPage.xaml";

            // Init inner view models
            TestCategoriesVM = new TestCategoriesVM(context);
            TestMaterialsVM = new TestMaterialsVM(context);
            TestMaterialsVM.OnCurrentPageChanged += (page) => CurrentPage = page;

            InitCommands();

        }
        #endregion


        #region Methods
        /****************************************************************************************/
        private void InitCommands()
        {
            PageNavigationCommand = new RelayCommand<string>(p => CurrentPage = p);
        }



        #endregion

    }
}
