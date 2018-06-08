using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.GoogleMaps;

namespace App4GDW
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HoleMapPage : ContentPage
	{
		public HoleMapPage ()
        {
            InitializeComponent();

            var pinMelbourne = new Pin() { Label = "Melbourne", Position = new Position(-37.971237, 144.492697) };
            var pinNewyork = new Pin() { Label = "New york", Position = new Position(40.705311, -74.2581874) };
            var pinLisboa = new Pin() { Label = "Lisboa", Position = new Position(38.7436057, -13.6426275) };
            var pinParis = new Pin() { Label = "Paris", Position = new Position(48.8588377, 2.2775173) };
            var pinTokyo = new Pin() { Label = "Tokyo", Position = new Position(35.7104, 139.8093) };
            map.Pins.Add(pinMelbourne);
            map.Pins.Add(pinNewyork);
            map.Pins.Add(pinLisboa);
            map.Pins.Add(pinParis);
            map.Pins.Add(pinTokyo);

            // CameraChanged is obsolete, please use CameraIdled
            map.CameraChanged += (sender, e) =>
            {
                var p = e.Position;
                var text = $"CameraChanged:Lat={p.Target.Latitude:0.00}, Long={p.Target.Longitude:0.00}, Zoom={p.Zoom:0.00}, Bearing={p.Bearing:0.00}, Tilt={p.Tilt:0.00}";
                labelStatus.Text = text;
                Debug.WriteLine(text);
            };

            map.CameraMoveStarted += (sender, e) =>
            {
                var text = $"CameraMoveStarted:IsGesture={e.IsGesture}";
                labelStatus.Text = text;
                Debug.WriteLine(text);
            };

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

            // MoveToCamera with Position
            buttonMoveToPosition.Clicked += async (sender, e) =>
            {
                await map.MoveCamera(CameraUpdateFactory.NewPosition(
                    pinMelbourne.Position)); // Melbourne
            };

            // MoveToCamera with Position and Zoom
            buttonMoveToPositionZoom.Clicked += async (sender, e) =>
            {
                await map.MoveCamera(CameraUpdateFactory.NewPositionZoom(
                    pinNewyork.Position, 16d)); // New york
            };

            // MoveToCamera with Bounds
            buttonMoveToBounds.Clicked += async (sender, e) =>
            {
                await map.MoveCamera(CameraUpdateFactory.NewBounds(
                    new Bounds(pinLisboa.Position,  // Lisboa
                               pinParis.Position),  // Paris
                   50)); // 50px
            };

            // MoveToCamera with CameraPosition
            buttonMoveToCameraPosition.Clicked += async (sender, e) =>
            {
                await map.MoveCamera(CameraUpdateFactory.NewCameraPosition(
                    new CameraPosition(
                        pinTokyo.Position, // Tokyo
                        17d, // zoom
                        45d, // bearing(rotation)
                        60d // tilt
                        )));
            };

            // #####################################################
            // #####################################################
            // for the test, ME
            Position p1 = new Position(32.873868, -97.084553);
            // for the test, green center at Hole#1
            Position p2 = new Position(32.858834, -97.058594);

            double bearingDegree = CalculateBearingAngle(p1, p2);
            Position midPoint = CalculateMidPoint(p1, p2);
            // need to calculate zoom based on the distance

            // #####################################################
            // #####################################################
            // Kyle Test to rotate google map based on two positions
            // #####################################################
            // #####################################################
            buttonKyleTestCameraPosition.Clicked += async (sender, e) =>
            {
                await map.MoveCamera(CameraUpdateFactory.NewCameraPosition(
                    new CameraPosition(
                        midPoint, // midpoint
                        17d, // zoom
                        bearingDegree, // bearing(rotation)
                        0d // tilt
                    )));
            };

            // AnimateToCamera with Position
            buttonAnimateToPosition.Clicked += async (sender, e) =>
            {
                var animState = await map.AnimateCamera(CameraUpdateFactory.NewPosition(
                    pinMelbourne.Position)); // Melbourne
                Debug.WriteLine($"Animate with Position result = {animState}");
            };

            // AnimateToCamera with Position and Zoom
            buttonAnimateToPositionZoom.Clicked += async (sender, e) =>
            {
                var animState = await map.AnimateCamera(CameraUpdateFactory.NewPositionZoom(
                    pinNewyork.Position, 16d), TimeSpan.FromSeconds(1)); // New york
                Debug.WriteLine($"Animate with Position and Zoom result = {animState}");
            };

            // AnimateToCamera with Bounds
            buttonAnimateToBounds.Clicked += async (sender, e) =>
            {
                var animState = await map.AnimateCamera(CameraUpdateFactory.NewBounds(
                    new Bounds(pinLisboa.Position,  // Lisboa
                               pinParis.Position),  // Paris
                   50), TimeSpan.FromSeconds(3)); // 50px
                Debug.WriteLine($"Animate with Bounds result = {animState}");
            };

            // AnimateToCamera with CameraPosition
            buttonAnimateToCameraPosition.Clicked += async (sender, e) =>
            {
                var animState = await map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(
                    new CameraPosition(
                        pinTokyo.Position, // Tokyo
                        17d, // zoom
                        45d, // bearing(rotation)
                        60d)), // tilt
                    TimeSpan.FromSeconds(5));
                Debug.WriteLine($"Animate with CameraPosition result = {animState}");
            };

            // Padding for Map
            entryPadding.TextChanged += (sender, e) =>
            {
                try
                {
                    var converter = new ThicknessTypeConverter();
                    var padding = (Thickness)converter.ConvertFromInvariantString(entryPadding.Text);
                    map.Padding = padding;
                }
                catch (Exception)
                {
                }
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

	        double distX = 0.0;
	        double distY = 0.0;

	        radians = Math.Atan((T.Latitude - M.Latitude) / (T.Longitude - M.Longitude));
	        angle = radians * (180 / Math.PI);

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