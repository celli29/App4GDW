using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.GoogleMaps;
using Plugin.Geolocator;

namespace App4GDW
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HoleMapPage : ContentPage
	{
        private MapSpan InitRegion { get; set; }
        private int CurrentHole { get; set; }
        private List<SimpleCoordinates> Coordinates { get; set; }
        private List<TeeCommonInfoes> TeeInfo { get; set; }

        private double[] arrLat { get; set; }
        private double[] arrLon { get; set; }
        private Position[] arrCenterGreens { get; set; }

        private int[] arrPar { get; set; }
        private int[] arrHandicap { get; set; }
        private int[] arrDistance { get; set; }

        // #####################################################
        // #####################################################
        // for the test, ME
        Position pMyHouse = new Position(32.873868, -97.084553);
        // for the test, green center at Hole#1
        Position p2 = new Position(32.858834, -97.058594);

        // plantation golf course
        Position pTeeGround = new Position(33.118873, -96.782237);
        Position pMidFairway = new Position(33.118986, -96.784034);
        Position pHole1Center = new Position(33.119057, -96.785694);

        Position myCurrentPosition;

        private async void InitMoveToRegion()
        {
            await Task.Delay(1);
            if(InitRegion != null)
            {
                map.MoveToRegion(InitRegion);
            }
        }

        // from plugin.geolocator, to get the current position
        // switch to fixed value when simulator
        private async Task<Position> GetMyCurrentPosition()
        {
            var timeout = TimeSpan.FromSeconds(1);
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;
            var position = await locator.GetPositionAsync(timeout, null, true);

            return  new Position(position.Latitude, position.Longitude);
        }

        public HoleMapPage (int hole)
        {
            InitializeComponent();
            CurrentHole = hole;
        }

        public HoleMapPage (int hole, List<SimpleCoordinates> coords, List<TeeCommonInfoes> teeInfoes)
        {
            InitializeComponent();
            CurrentHole = hole;
            Coordinates = coords;
            TeeInfo = teeInfoes;

            // to set bolf font partially
            var fs = new FormattedString();
            fs.Spans.Add(new Span { Text = ", Bold", ForegroundColor = Color.Gray, FontSize = 20, FontAttributes = FontAttributes.Bold });

            // to build arrPositions for later use (1-18 holes)
            arrLat = coords.Where(c => c.Theme == "CenterLat").FirstOrDefault().TransposeCoordinates();
            arrLon = coords.Where(c => c.Theme == "CenterLon").FirstOrDefault().TransposeCoordinates();
            arrCenterGreens = new Position[18];
            if (arrLat != null && arrLon != null)
            {
                for (int i = 0; i < 18; i++)
                {
                    arrCenterGreens[i] = new Position(arrLat[i], arrLon[i]);
                }
            }

            arrPar = teeInfoes.Where(t => t.Theme == "Par").FirstOrDefault().TransposeTeeInfoes();
            arrHandicap = teeInfoes.Where(t => t.Theme == "Handicap").FirstOrDefault().TransposeTeeInfoes();
            arrDistance = teeInfoes.Where(t => t.Theme == "Distance").FirstOrDefault().TransposeTeeInfoes();

            labelHole.Text = "Hole # " + CurrentHole;
            labelHole.FontAttributes = FontAttributes.Bold;


            myCurrentPosition = pTeeGround;
            // enable this when AD-HOC
            // because this is not in async, it doesnt' work
            //myCurrentPosition = GetMyCurrentPosition().Result;

            // try to fix back to Rome issue
            MapSpan ms = null;
            try
            {
                ms = MapSpan.FromCenterAndRadius(myCurrentPosition, new Distance(1000));    
            }
            catch(Exception e)
            {
                string mm = e.Message;
            }

            InitRegion = ms;
            InitMoveToRegion();

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


            map.MapClicked += (sender, e) =>
            {
                double distance1; // me to the point
                double distance2; // the point to the green

                Position thePoint = new Position(e.Point.Latitude,e.Point.Longitude);

                distance1 = CalculateDistance(myCurrentPosition, thePoint);
                distance2 = CalculateDistance(thePoint, arrCenterGreens[CurrentHole - 1]);

                labelDistance.Text = "To this : " + distance1.KmToYard().ToString("#.#") + ", To Green : " + distance2.KmToYard().ToString("#.#");

                Pin objectTarget = new Pin()
                {
                    Icon = BitmapDescriptorFactory.DefaultMarker(Color.DodgerBlue),
                    Type = PinType.Place,
                    Label = "To this : " + distance1.KmToYard().ToString("#.#") + ", To Green : " + distance2.KmToYard().ToString("#.#"),
                    Address = "Target",
                    Position = thePoint,
                    Tag = " ",
                    IsDraggable = true
                };
                map.Pins.Add(objectTarget);



            };

            map.PinDragEnd += (sender, e) =>
            {
                double distance3; // me to the point
                double distance4; // the point to the green

                Position thePoint3 = e.Pin.Position;

                distance3 = CalculateDistance(myCurrentPosition, thePoint3);
                distance4 = CalculateDistance(thePoint3, arrCenterGreens[CurrentHole - 1]);

                e.Pin.Label = "To this : " + distance3.KmToYard().ToString("#.#") + ", To Green : " + distance4.KmToYard().ToString("#.#");
                e.Pin.Address = "Target";

                labelDistance.Text = "To this : " + distance3.KmToYard().ToString("#.#") + ", To Green : " + distance3.KmToYard().ToString("#.#");

            };


            buttonHole1.Clicked += async (sender, e) =>
            {
                await EighteenHoles();

            };

            buttonScore.Clicked += (sender, e) => Navigation.PushAsync(new ScoreInputPage(CurrentHole));

           
            map.Padding = new Thickness(0, 0, 0, 0);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await EighteenHoles();
        }

        //protected override async void OnDisappearing()
        //{
        //    base.OnDisappearing();

        //    //await FakeReturn();
        //}

        //private async Task<string> FakeReturn()
        //{
        //    return "fake";
        //}

        private async Task<string> EighteenHoles()
        {
            Position hole1Green = arrCenterGreens[CurrentHole-1];
            //hole1Green = new Position(33.119057, -96.785694);

            labelHole.Text = "Hole # " + CurrentHole + "  Par " + arrPar[CurrentHole - 1] + " H: " + arrHandicap[CurrentHole - 1];

            // enable this when AD-HOC
            //myCurrentPosition = GetMyCurrentPosition().Result;

            var timeout = TimeSpan.FromSeconds(1);
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;
            var position = await locator.GetPositionAsync(timeout, null, true);
            myCurrentPosition = new Position(position.Latitude, position.Longitude);

            //myCurrentPosition = pTeeGround;

            double distanceToHole = CalculateDistance(myCurrentPosition, hole1Green);


            double bearingHole1 = CalculateBearingAngle(myCurrentPosition, hole1Green);
            Position midPointHole1 = CalculateMidPoint(myCurrentPosition, hole1Green);

            //map.MoveToRegion(MapSpan.FromCenterAndRadius(midPointHole1, Distance.FromMiles(0.4)));

            double zoomLevel = distanceToHole.KmToZoomLevel2(midPointHole1.Latitude);

            await map.MoveCamera(CameraUpdateFactory.NewCameraPosition(
                new CameraPosition(
                    midPointHole1, // midpoint
                    zoomLevel, // zoom
                    bearingHole1, // bearing(rotation)
                    0d // tilt
                )));

            map.Pins.Clear();

            Pin pinBottom = new Pin()
            {
                Type = PinType.Generic,
                Label = "Center Pin",
                Address = "My current position",
                Position = myCurrentPosition,
                Tag = " "
            };
            map.Pins.Add(pinBottom);

            Pin pinTarget = new Pin()
            {
                Icon = BitmapDescriptorFactory.DefaultMarker(Color.Green),
                Type = PinType.Place,
                Label = "Hole1 Pin",
                Address = "Hole1 Green",
                Position = hole1Green,
                Tag = " "
            };
            map.Pins.Add(pinTarget);

            labelCurrentDistance.Text = distanceToHole.KmToYard().ToString("#.#") + " yd";
            labelCurrentDistance.FontAttributes = FontAttributes.Bold;
            labelCurrentDistance.TextColor = Color.DodgerBlue;
            //labelHole.Text += " (mi)" + distanceToHole.KmToMile().ToString("#.###");

            return "Success";
        }

	    private Position CalculateMidPoint(Position p1, Position p2)
	    {
	        return new Position((p1.Latitude+p2.Latitude)/2, (p1.Longitude + p2.Longitude) / 2);
	    }

        // input: lat, long
        // middle: convert lat/long to calculate distance
        // output: angle
	    private double CalculateBearingAngle(Position M, Position T) // Me; Me, T;Target
	    {
	        double angle = 0.0;
	        double radians = 0.0;

	        radians = Math.Atan((T.Longitude - M.Longitude) / (T.Latitude - M.Latitude));
	        angle = radians * (180 / Math.PI);
            if(T.Latitude-M.Latitude < 0)
            {
                angle += 180;
            }

	        return angle;
	    }

	    private double DegreesToRadians(double degrees) // static
	    {
	        return degrees * Math.PI / 180.0;
	    }

	    public double CalculateDistance(Position location1, Position location2) // was static
	    {
	        double circumference = 6371 * 2 * Math.PI; // Earth's circumference at the equator in km ?? 6371?? 40000
	        double distance = 0.0;

	        //Calculate radians
	        double latitude1Rad = DegreesToRadians(location1.Latitude);
	        double longitude1Rad = DegreesToRadians(location1.Longitude);
	        double latititude2Rad = DegreesToRadians(location2.Latitude);
	        double longitude2Rad = DegreesToRadians(location2.Longitude);

	        double logitudeDiff = Math.Abs(longitude1Rad - longitude2Rad);

	        if (logitudeDiff > Math.PI)
	        {
	            logitudeDiff = 2.0 * Math.PI - logitudeDiff;
	        }

	        double angleCalculation =
	            Math.Acos(
	                Math.Sin(latititude2Rad) * Math.Sin(latitude1Rad) +
	                Math.Cos(latititude2Rad) * Math.Cos(latitude1Rad) * Math.Cos(logitudeDiff));

	        distance = circumference * angleCalculation / (2.0 * Math.PI);

	        return distance;
	    }
    }
}