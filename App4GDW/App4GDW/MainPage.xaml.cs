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
        public Dictionary<string, int> dictCourses { get; set; }
        public string TeeName { get; set; }
        private List<SimpleCoordinates> Coords { get; set; }
        private List<TeeCommonInfoes> teeCommonInfoes { get; set; }
        private List<SimpleTee> candidateTees { get; set; }

        private int _gcid { get; set; }
        private string _teeName { get; set; }
        private string _gender { get; set; }

		public MainPage()
		{
			InitializeComponent();

            buttonScoreInput.BackgroundColor = Color.DodgerBlue;
            buttonHoleMap.BackgroundColor = Color.DodgerBlue;
            buttonTest.BackgroundColor = Color.LightGray;

		    //buttonScoreInput.Clicked += (_, e) => Navigation.PushAsync(new ScoreInputPage());
            buttonHoleMap.Clicked += (_, e) => Navigation.PushAsync(new HoleMapPage(1, Coords, teeCommonInfoes));
            buttonTest.Clicked += (_, e) => Navigation.PushAsync(new TestPage());    
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            List<SimpleCourse> temp = await App.DatabaseManager.GetCourseTasksAsync();

            // make a dictionary for the list of simplecourse, cause xamarin picker items doesn't support item.value etc
            dictCourses = new Dictionary<string, int>();
            foreach(var b in temp)
            {
                dictCourses.Add(b.Name, b.GCID); // name:key, gcid:value
            }

            List<string> newItems = new List<string>();

            foreach(var a in temp)
            {
                newItems.Add(a.Name);   
            }

            coursePicker.ItemsSource = newItems;
        }

        protected async void OnCourseSelected(object sender, EventArgs args)
        {
            Picker cPicker = (Picker)sender;

            var course = cPicker.SelectedItem;
            teePicker.Items.Clear();

            // to be replaced by data source
            _gcid = dictCourses[cPicker.SelectedItem.ToString()];

            // hack
            // until fixing the issue of picker
            //GCID = 1060; // plantation golf course

            List<SimpleTee> temp = await App.DatabaseManager.GetTeeTasksAsync(_gcid);
            candidateTees = temp;
            teePicker.IsEnabled = true;

            List<string> newItems = new List<string>();

            foreach (var a in temp)
            {
                newItems.Add(a.Name + " : " + a.Gender);
            }

            // now to get coordinates information
            Coords = await App.DatabaseManager.GetCoordinatesTasksAsync(_gcid);

            // save coordinates into local database
            double[] arrLat = Coords.Where(c => c.Theme == "CenterLat").FirstOrDefault().TransposeCoordinates();
            double[] arrLon = Coords.Where(c => c.Theme == "CenterLon").FirstOrDefault().TransposeCoordinates();

            for (int i = 0; i < 18; i++)
            {
                await App.LocalDB.SaveCoordAsync(new LocalCoordinates() { Hole = i + 1, CenterLat = arrLat[i], CenterLon = arrLon[i] });
            }
                

            teePicker.ItemsSource = newItems;
        }

        protected async void OnTeeSelected(object sender, EventArgs args)
        {
            Picker tPicker = (Picker)sender;

            TeeName = tPicker.SelectedItem.ToString();

            string[] temp = TeeName.Split(':');

            teeCommonInfoes = await App.DatabaseManager.GetTeeInfoTasksAsync(_gcid, temp[0].Trim(), temp[1].Trim());

            // save teecommoninfoes into local database
            int[] arrPar = teeCommonInfoes.Where(t => t.Theme == "Par").FirstOrDefault().TransposeTeeInfoes();
            int[] arrHandicap = teeCommonInfoes.Where(t => t.Theme == "Handicap").FirstOrDefault().TransposeTeeInfoes();
            int[] arrDistance = teeCommonInfoes.Where(t => t.Theme == "Distance").FirstOrDefault().TransposeTeeInfoes();

            for (int i = 0; i < 18; i++)
            {
                await App.LocalDB.SaveLocalTeeInfoAsync(new LocalTeeInfo() { Hole = i + 1, Par = arrPar[i], Handicap = arrHandicap[i], Distance = arrDistance[i] });
            }

        }
	}
}
