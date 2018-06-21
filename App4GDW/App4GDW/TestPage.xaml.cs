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
	public partial class TestPage : ContentPage
	{
        private MapSpan InitRegion { get; set; }

        private async void InitMoveToRegion()
        {
            await Task.Delay(2000);
            if(InitRegion != null)
            {
                map.MoveToRegion(InitRegion);
            }
        }

		public TestPage ()
        {
            InitializeComponent();

            var pinMelbourne = new Pin() { Label = "Melbourne", Position = new Position(-37.971237, 144.492697) };


            Position pCenter = new Position(32.873868, -97.084553);
            Position pQ1 = new Position(32.889995, -97.058218);
            Position pQ2 = new Position(32.889995, -97.117115);
            Position pQ3 = new Position(32.859097, -97.117115);
            Position pQ4 = new Position(32.859097, -97.058218);

            //Position pTarget = pQ2;
            // try to fix back to Rome issue
            MapSpan ms = null;
            try
            {
                ms = MapSpan.FromCenterAndRadius(new Position(33, -97), new Distance(1000));    
            }
            catch(Exception e)
            {
                string mm = e.Message;
            }

            InitRegion = ms;
            InitMoveToRegion();

            /*
            map.Pins.Clear();

            map.InitialCameraUpdate = CameraUpdateFactory.NewCameraPosition(new CameraPosition(
                pCenter,
                14d,
                0d,
                0d));
*/
            // MapTypes
            var mapTypeValues = new List<MapType>();
            foreach (var mapType in Enum.GetValues(typeof(MapType)))
            {
                mapTypeValues.Add((MapType)mapType);
                pickerMapType.Items.Add(Enum.GetName(typeof(MapType), mapType));
            }

            pickerMapType.SelectedIndexChanged += (sender, e) =>
            {
                map.MapType = mapTypeValues[pickerMapType.SelectedIndex];
            };
            pickerMapType.SelectedIndex = 2;

            // CameraChanged is obsolete, please use CameraIdled

            /*
            map.CameraChanged += (sender, e) =>
            {
                var p = e.Position;
                var text = $"CameraChanged:Lat={p.Target.Latitude:0.00}, Long={p.Target.Longitude:0.00}, Zoom={p.Zoom:0.00}, Bearing={p.Bearing:0.00}, Tilt={p.Tilt:0.00}";
                labelStatus.Text = text;
                Debug.WriteLine(text);
            };
            */

            /*
            map.CameraMoveStarted += (sender, e) =>
            {
                var text = $"CameraMoveStarted:IsGesture={e.IsGesture}";
                labelStatus.Text = text;
                Debug.WriteLine(text);
            };
            */

            map.CameraMoving += (sender, e) =>
            {
                var p = e.Position;
                var text = $"CameraMoving:Lat={p.Target.Latitude:0.00}, Long={p.Target.Longitude:0.00}, Zoom={p.Zoom:0.00}, Bearing={p.Bearing:0.00}, Tilt={p.Tilt:0.00}";
                labelStatus.Text = text;
                Debug.WriteLine(text);
            };


            map.CameraIdled += (sender, e) =>
            {
                var p = e.Position;
                var text = $"CameraIdled:Lat={p.Target.Latitude:0.00}, Long={p.Target.Longitude:0.00}, Zoom={p.Zoom:0.00}, Bearing={p.Bearing:0.00}, Tilt={p.Tilt:0.00}";
                labelStatus.Text = text;
                Debug.WriteLine(text);
            };



            // #####################################################
            // #####################################################
            // for the test, ME
            Position p1 = new Position(32.873868, -97.084553);
            // for the test, green center at Hole#1
            Position p2 = new Position(32.858834, -97.058594);

            // first test pCenter / pQ1 for quadrant 1
            double bearingDegree1 = CalculateBearingAngle(pCenter, pQ1);
            double bearingDegree2 = CalculateBearingAngle(pCenter, pQ2);
            double bearingDegree3 = CalculateBearingAngle(pCenter, pQ3);
            double bearingDegree4 = CalculateBearingAngle(pCenter, pQ4);
            //bearingDegree = 315d;
            Position midPoint1 = CalculateMidPoint(pCenter, pQ1);
            Position midPoint2 = CalculateMidPoint(pCenter, pQ2);
            Position midPoint3 = CalculateMidPoint(pCenter, pQ3);
            Position midPoint4 = CalculateMidPoint(pCenter, pQ4);
            // need to calculate zoom based on the distance

            // #####################################################
            // #####################################################
            // Kyle Test to rotate google map based on two positions
            // #####################################################
            // #####################################################
            buttonKyleTest1CameraPosition.Clicked += async (sender, e) =>
            {
                //map.MoveToRegion(MapSpan.FromCenterAndRadius(midPoint1, Distance.FromMiles(4)));

                await map.MoveCamera(CameraUpdateFactory.NewCameraPosition(
                    new CameraPosition(
                        midPoint1, // midpoint
                        13d, // zoom
                        bearingDegree1, // bearing(rotation)
                        0d // tilt
                    )));

                map.Pins.Clear();

                Pin pinBottom = new Pin()
                {
                    Type = PinType.Generic,
                    Label = "Center Pin",
                    Address = "My Home",
                    Position = pCenter,
                    Tag = " "
                };
                map.Pins.Add(pinBottom);

                Pin pinTarget = new Pin()
                {
                    Type = PinType.Place,
                    Label = "Quad1 Pin",
                    Address = "My Quad1",
                    Position = pQ1,
                    Tag = " "
                };
                map.Pins.Add(pinTarget);

                //map.MoveToRegion(MapSpan.FromCenterAndRadius(midPoint1, Distance.FromMiles(4)));

            };

            buttonKyleTest2CameraPosition.Clicked += async (sender, e) =>
            {
                //map.MoveToRegion(MapSpan.FromCenterAndRadius(midPoint2, Distance.FromMiles(4)));

                await map.MoveCamera(CameraUpdateFactory.NewCameraPosition(
                    new CameraPosition(
                        midPoint2, // midpoint
                        13d, // zoom
                        bearingDegree2, // bearing(rotation)
                        0d // tilt
                    )));

                map.Pins.Clear();

                Pin pinBottom = new Pin()
                {
                    Type = PinType.Generic,
                    Label = "Center Pin",
                    Address = "My Home",
                    Position = pCenter,
                    Tag = " "
                };
                map.Pins.Add(pinBottom);

                Pin pinTarget = new Pin()
                {
                    Type = PinType.Place,
                    Label = "Quad2 Pin",
                    Address = "My Quad2",
                    Position = pQ2,
                    Tag = " "
                };
                map.Pins.Add(pinTarget);

            };

            buttonKyleTest3CameraPosition.Clicked += async (sender, e) =>
            {
                map.MoveToRegion(MapSpan.FromCenterAndRadius(midPoint3, Distance.FromMiles(4)));

                await map.MoveCamera(CameraUpdateFactory.NewCameraPosition(
                    new CameraPosition(
                        midPoint3, // midpoint
                        13d, // zoom
                        bearingDegree3, // bearing(rotation)
                        0d // tilt
                    )));

                map.Pins.Clear();

                Pin pinBottom = new Pin()
                {
                    Type = PinType.Generic,
                    Label = "Center Pin",
                    Address = "My Home",
                    Position = pCenter,
                    Tag = " "
                };
                map.Pins.Add(pinBottom);

                Pin pinTarget = new Pin()
                {
                    Type = PinType.Place,
                    Label = "Quad3 Pin",
                    Address = "My Quad3",
                    Position = pQ3,
                    Tag = " "
                };
                map.Pins.Add(pinTarget);

            };

            buttonKyleTest4CameraPosition.Clicked += async (sender, e) =>
            {
                map.MoveToRegion(MapSpan.FromCenterAndRadius(midPoint4, Distance.FromMiles(4)));

                await map.MoveCamera(CameraUpdateFactory.NewCameraPosition(
                    new CameraPosition(
                        midPoint4, // midpoint
                        13d, // zoom
                        bearingDegree4, // bearing(rotation)
                        0d // tilt
                    )));

                map.Pins.Clear();

                Pin pinBottom = new Pin()
                {
                    Type = PinType.Generic,
                    Label = "Center Pin",
                    Address = "My Home",
                    Position = pCenter,
                    Tag = " "
                };
                map.Pins.Add(pinBottom);

                Pin pinTarget = new Pin()
                {
                    Type = PinType.Place,
                    Label = "Quad4 Pin",
                    Address = "My Quad4",
                    Position = pQ4,
                    Tag = " "
                };
                map.Pins.Add(pinTarget);

            };

            buttonHole1.Clicked += async (sender, e) =>
            {
                Position myPosition;
                Position hole1Green = new Position(32.553649, -97.076853);

                // from plugin.geolocator, to get the current position
                // switch to fixed value when simulator
                /*
                var timeout = TimeSpan.FromSeconds(1);
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;
                var position = await locator.GetPositionAsync(timeout, null, true);
                myPosition = new Position(position.Latitude,position.Longitude);
                */

                myPosition = p1; // my house

                holeDistance.Text = myPosition.Latitude + "," + myPosition.Longitude;

                double distanceToHole = CalculateDistance(myPosition, hole1Green);


                double bearingHole1 = CalculateBearingAngle(myPosition, hole1Green);
                Position midPointHole1 = CalculateMidPoint(myPosition, hole1Green);

                //map.MoveToRegion(MapSpan.FromCenterAndRadius(midPointHole1, Distance.FromMiles(0.4)));

                await map.MoveCamera(CameraUpdateFactory.NewCameraPosition(
                    new CameraPosition(
                        midPointHole1, // midpoint
                        distanceToHole.KmToZoomLevel(), // zoom
                        bearingHole1, // bearing(rotation)
                        0d // tilt
                    )));

                map.Pins.Clear();

                Pin pinBottom = new Pin()
                {
                    Type = PinType.Generic,
                    Label = "Center Pin",
                    Address = "My current position",
                    Position = myPosition,
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

                holeDistance.Text = "distance to hole (yd)" + distanceToHole.KmToYard();
                holeDistance.Text = "distance to hole (mi)" + distanceToHole.KmToMile();

            };

            sliderZoom.ValueChanged += async (sender, e) => 
            {
                float aa = (float)e.NewValue;

                await map.MoveCamera(CameraUpdateFactory.NewCameraPosition(
                    new CameraPosition(
                        pCenter, // midpoint
                        aa, // zoom
                        0d, // bearing(rotation)
                        0d // tilt
                    )));
            };

           
            map.Padding = new Thickness(0, 0, 0, 0);
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