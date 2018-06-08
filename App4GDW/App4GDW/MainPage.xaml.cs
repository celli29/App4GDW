using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App4GDW
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

		    buttonScoreInput.Clicked += (_, e) => Navigation.PushAsync(new ScoreInputPage());
		    buttonHoleMap.Clicked += (_, e) => Navigation.PushAsync(new HoleMapPage());
        }
	}
}
