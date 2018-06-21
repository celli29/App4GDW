using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App4GDW
{
	public partial class App : Application
	{
        public static DatabaseManager DatabaseManager { get; private set; }
        public static LocalDB localDB;

		public App ()
		{
			InitializeComponent();

            DatabaseManager = new DatabaseManager(new RestService());
            MainPage = new NavigationPage(new MainPage());
		}

        public static LocalDB LocalDB
        {
            get
            {
                if (localDB == null)
                {
                    localDB = new LocalDB(DependencyService.Get<IFileHelper>().GetLocalFilePath("LocalGdw.db3"));
                }
                return LocalDB;
            }
        }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
