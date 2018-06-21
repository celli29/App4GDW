using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.GoogleMaps;

namespace App4GDW
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScoreInputPage : ContentPage
	{
        private int CurrentHole { get; set; }

        private List<ScoreCard> scoreCards { get; set; }
        private List<TeeCommonInfoes> teeCommonInfoes { get; set; }

        public ScoreInputPage (int hole)
        {
            InitializeComponent();
            CurrentHole = hole;

            labelHeader.Text = "Hole # " + CurrentHole;

            buttonNext.Clicked += async (sender, e) =>
            {
                CurrentHole = CurrentHole % 18 + 1;
                await EighteenHoles();
            };

            buttonPrev.Clicked += async (sender, e) =>
            {
                CurrentHole = CurrentHole == 1 ? 18 : CurrentHole == 18 ? 17 : CurrentHole - 1;
                await EighteenHoles();
            };

            buttonMap.Clicked += (sender, e) => Navigation.PushAsync(new HoleMapPage(CurrentHole));

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await EighteenHoles();
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            await SavingData();

            // before disappearing, save all the data
        }

        protected async Task<string> EighteenHoles()
        {
            // data binding and retrieving/updating data
            labelHeader.Text = "Hole # " + CurrentHole;

            return "success";
        }

        protected async Task<string> SavingData()
        {
            return "data successfully saved";
        }
    }
}