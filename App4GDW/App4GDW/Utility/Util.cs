using System;
namespace App4GDW
{
    public static class Util
    {

        public static double KmToMile(this double km)
        {
            return km * 0.62137119;
        }

        public static double MileToKm(this double mile)
        {
            return mile * 1.609344;
        }

        public static double KmToYard(this double km)
        {
            return km * 1093.6132983377;
        }

        public static double KmToZoomLevel(this double km)
        {
            return Math.Log(80000 / km, 2);
        }

        // need to have latitude
        public static double KmToZoomLevel2(this double km, double lat)
        {
            double pixelHeight = 750; // iphone6 ; 1334, but for the test, use half (750 x 1334)

            return Math.Log(156543.03392 * Math.Cos(lat * Math.PI/180) / (km*1000/pixelHeight), 2)-1;
        }

        public static double[] TransposeCoordinates(this SimpleCoordinates sc)
        {
            double[] arr = new double[18];

            if( sc == null)
            {
                return arr;
            }

            arr[0] = sc.H01;
            arr[1] = sc.H02;
            arr[2] = sc.H03;
            arr[3] = sc.H04;
            arr[4] = sc.H05;
            arr[5] = sc.H06;

            arr[6] = sc.H07;
            arr[7] = sc.H08;
            arr[8] = sc.H09;
            arr[9] = sc.H10;
            arr[10] = sc.H11;
            arr[11] = sc.H12;

            arr[12] = sc.H13;
            arr[13] = sc.H14;
            arr[14] = sc.H15;
            arr[15] = sc.H16;
            arr[16] = sc.H17;
            arr[17] = sc.H18;

            return arr;
        }

        public static int[] TransposeTeeInfoes(this TeeCommonInfoes tci)
        {
            int[] arr = new int[18];

            if (tci == null)
            {
                return arr;
            }

            arr[0] = tci.H01 ?? 0;
            arr[1] = tci.H02 ?? 0;
            arr[2] = tci.H03 ?? 0;
            arr[3] = tci.H04 ?? 0;
            arr[4] = tci.H05 ?? 0;
            arr[5] = tci.H06 ?? 0;

            arr[6] = tci.H07 ?? 0;
            arr[7] = tci.H08 ?? 0;
            arr[8] = tci.H09 ?? 0;
            arr[9] = tci.H10 ?? 0;
            arr[10] = tci.H11 ?? 0;
            arr[11] = tci.H12 ?? 0;

            arr[12] = tci.H13 ?? 0;
            arr[13] = tci.H14 ?? 0;
            arr[14] = tci.H15 ?? 0;
            arr[15] = tci.H16 ?? 0;
            arr[16] = tci.H17 ?? 0;
            arr[17] = tci.H18 ?? 0;

            return arr;
        }
    }
}
