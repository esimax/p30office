using System;


namespace POL.Lib.Utils
{
    public class PrayTime
    {

        public static int Jafari = 0; 
        public static int Karachi = 1; 
        public static int ISNA = 2; 
        public static int MWL = 3; 
        public static int Makkah = 4; 
        public static int Egypt = 5; 
        public static int Custom = 6; 
        public static int Tehran = 7; 

        public static int Shafii = 0; 
        public static int Hanafi = 1; 

        public static int None = 0; 
        public static int MidNight = 1; 
        public static int OneSeventh = 2; 
        public static int AngleBased = 3; 


        public static int Time24 = 0; 
        public static int Time12 = 1; 
        public static int Time12NS = 2; 
        public static int Floating = 3; 

        public static string[] timeNames = {"Fajr", "Sunrise", "Dhuhr", "Asr", "Sunset", "Maghrib", "Isha"};
        private static readonly string InvalidTime = "----"; 
        private int adjustHighLats = 1; 
        private int asrJuristic; 




        private int calcMethod = 7; 
        private int dhuhrMinutes; 
        private double JDate; 

        private double lat; 
        private double lng; 



        private readonly double[][] methodParams;




        private readonly int numIterations = 1; 

        private int timeFormat; 

        private int[] times;
        private double timeZone; 

        public PrayTime()
        {
            times = new int[7];
            methodParams = new double[8][];
            methodParams[Jafari] = new double[] {16, 0, 4, 0, 14};
            methodParams[Karachi] = new double[] {18, 1, 0, 0, 18};
            methodParams[ISNA] = new double[] {15, 1, 0, 0, 15};
            methodParams[MWL] = new double[] {18, 1, 0, 0, 17};
            methodParams[Makkah] = new[] {18.5, 1, 0, 1, 90};
            methodParams[Egypt] = new[] {19.5, 1, 0, 0, 17.5};
            methodParams[Tehran] = new[] {17.7, 0, 4.5, 0, 14};
            methodParams[Custom] = new double[] {18, 1, 0, 0, 17};
        }


        public string[] getPrayerTimes(int year, int month, int day, double latitude, double longitude, int timeZone)
        {
            return getDatePrayerTimes(year, month + 1, day, latitude, longitude, timeZone);
        }

        public void setCalcMethod(int methodID)
        {
            calcMethod = methodID;
        }

        public void setAsrMethod(int methodID)
        {
            if (methodID < 0 || methodID > 1)
                return;
            asrJuristic = methodID;
        }

        public void setFajrAngle(double angle)
        {
            setCustomParams(new[] {(int) angle, -1, -1, -1, -1});
        }

        public void setMaghribAngle(double angle)
        {
            setCustomParams(new[] {-1, 0, (int) angle, -1, -1});
        }

        public void setIshaAngle(double angle)
        {
            setCustomParams(new[] {-1, -1, -1, 0, (int) angle});
        }

        public void setDhuhrMinutes(int minutes)
        {
            dhuhrMinutes = minutes;
        }

        public void setMaghribMinutes(int minutes)
        {
            setCustomParams(new[] {-1, 1, minutes, -1, -1});
        }

        public void setIshaMinutes(int minutes)
        {
            setCustomParams(new[] {-1, -1, -1, 1, minutes});
        }

        public void setCustomParams(int[] param)
        {
            for (var i = 0; i < 5; i++)
            {
                if (param[i] == -1)
                    methodParams[Custom][i] = methodParams[calcMethod][i];
                else
                    methodParams[Custom][i] = param[i];
            }
            calcMethod = Custom;
        }

        public void setHighLatsMethod(int methodID)
        {
            adjustHighLats = methodID;
        }

        public void setTimeFormat(int timeFormat)
        {
            this.timeFormat = timeFormat;
        }

        public string floatToTime24(double time)
        {
            if (time < 0)
                return InvalidTime;
            time = FixHour(time + 0.5/60); 
            var hours = Math.Floor(time);
            var minutes = Math.Floor((time - hours)*60);
            return string.Format("{0}:{1}", twoDigitsFormat((int) hours), twoDigitsFormat((int) minutes));
        }

        public string floatToTime12(double time, bool noSuffix)
        {
            if (time < 0)
                return InvalidTime;
            time = FixHour(time + 0.5/60); 
            var hours = Math.Floor(time);
            var minutes = Math.Floor((time - hours)*60);
            var suffix = hours >= 12 ? " pm" : " am";
            hours = (hours + 12 - 1)%12 + 1;
            return (int) hours + ":" + twoDigitsFormat((int) minutes) + (noSuffix ? "" : suffix);
        }

        public string floatToTime12NS(double time)
        {
            return floatToTime12(time, true);
        }



        public string[] getDatePrayerTimes(int year, int month, int day, double latitude, double longitude,
            double timeZone)
        {
            lat = latitude;
            lng = longitude;
            this.timeZone = timeZone;
            JDate = JulianDate(year, month, day) - longitude/(15*24);

            return computeDayTimes();
        }

        public double[] sunPosition(double jd)
        {
            var D = jd - 2451545.0;
            var g = FixAngle(357.529 + 0.98560028*D);
            var q = FixAngle(280.459 + 0.98564736*D);
            var L = FixAngle(q + 1.915*dsin(g) + 0.020*dsin(2*g));

            var e = 23.439 - 0.00000036*D;

            var d = darcsin(dsin(e)*dsin(L));
            var RA = darctan2(dcos(e)*dsin(L), dcos(L))/15;
            RA = FixHour(RA);
            var EqT = q/15 - RA;

            return new[] {d, EqT};
        }

        public double equationOfTime(double jd)
        {
            return sunPosition(jd)[1];
        }

        public double sunDeclination(double jd)
        {
            return sunPosition(jd)[0];
        }

        public double computeMidDay(double t)
        {
            var T = equationOfTime(JDate + t);
            var Z = FixHour(12 - T);
            return Z;
        }

        public double computeTime(double G, double t)
        {

            var D = sunDeclination(JDate + t);
            var Z = computeMidDay(t);
            var V = (double) 1/15*darccos((-dsin(G) - dsin(D)*dsin(lat))/(dcos(D)*dcos(lat)));
            return Z + (G > 90 ? -V : V);
        }

        public double computeAsr(int step, double t) 
        {
            var D = sunDeclination(JDate + t);
            var G = -darccot(step + dtan(Math.Abs(lat - D)));
            return computeTime(G, t);
        }


        public double[] computeTimes(double[] times)
        {
            var t = dayPortion(times);


            var Fajr = computeTime(180 - methodParams[calcMethod][0], t[0]);
            var Sunrise = computeTime(180 - 0.833, t[1]);
            var Dhuhr = computeMidDay(t[2]);
            var Asr = computeAsr(1 + asrJuristic, t[3]);
            var Sunset = computeTime(0.833, t[4]);
            var Maghrib = computeTime(methodParams[calcMethod][2], t[5]);
            var Isha = computeTime(methodParams[calcMethod][4], t[6]);

            return new[] {Fajr, Sunrise, Dhuhr, Asr, Sunset, Maghrib, Isha};
        }

        public double[] adjustHighLatTimes(double[] times)
        {
            var nightTime = GetTimeDifference(times[4], times[1]); 

            var FajrDiff = nightPortion(methodParams[calcMethod][0])*nightTime;
            if (GetTimeDifference(times[0], times[1]) > FajrDiff)
                times[0] = times[1] - FajrDiff;

            var IshaAngle = Math.Abs(methodParams[calcMethod][3]) < 0.01
                ? methodParams
                    [calcMethod][4]
                : 18;
            var IshaDiff = nightPortion(IshaAngle)*nightTime;
            if (GetTimeDifference(times[4], times[6]) > IshaDiff)
                times[6] = times[4] + IshaDiff;

            var MaghribAngle = Math.Abs(methodParams[calcMethod][1]) < 0.01
                ? methodParams
                    [calcMethod][2]
                : 4;
            var MaghribDiff = nightPortion(MaghribAngle)*nightTime;
            if (GetTimeDifference(times[4], times[5]) > MaghribDiff)
                times[5] = times[4] + MaghribDiff;

            return times;
        }

        public double nightPortion(double angle)
        {
            double val = 0;
            if (adjustHighLats == AngleBased)
                val = 1.0/60.0*angle;
            if (adjustHighLats == MidNight)
                val = 1.0/2.0;
            if (adjustHighLats == OneSeventh)
                val = 1.0/7.0;

            return val;
        }

        public double[] dayPortion(double[] times)
        {
            for (var i = 0; i < times.Length; i++)
            {
                times[i] /= 24;
            }
            return times;
        }

        public string[] computeDayTimes()
        {
            double[] times = {5, 6, 12, 13, 18, 18, 18}; 

            for (var i = 0; i < numIterations; i++)
            {
                times = computeTimes(times);
            }

            times = adjustTimes(times);
            return adjustTimesFormat(times);
        }


        public double[] adjustTimes(double[] times)
        {
            for (var i = 0; i < 7; i++)
            {
                times[i] += timeZone - lng/15;
            }
            times[2] += dhuhrMinutes/60; 
            if (Math.Abs(methodParams[calcMethod][1] - 1) < 0.01) 
                times[5] = times[4] + methodParams[calcMethod][2]/60.0;
            if (Math.Abs(methodParams[calcMethod][3] - 1) < 0.01) 
                times[6] = times[5] + methodParams[calcMethod][4]/60.0;

            if (adjustHighLats != None)
            {
                times = adjustHighLatTimes(times);
            }

            return times;
        }

        public string[] adjustTimesFormat(double[] times)
        {
            var formatted = new string[times.Length];

            if (timeFormat == Floating)
            {
                for (var i = 0; i < times.Length; ++i)
                {
                    formatted[i] = times[i] + "";
                }
                return formatted;
            }
            for (var i = 0; i < 7; i++)
            {
                if (timeFormat == Time12)
                    formatted[i] = floatToTime12(times[i], true);
                else if (timeFormat == Time12NS)
                    formatted[i] = floatToTime12NS(times[i]);
                else
                    formatted[i] = floatToTime24(times[i]);
            }
            return formatted;
        }


        public double GetTimeDifference(double c1, double c2)
        {
            var diff = FixHour(c2 - c1);
            return diff;
        }

        public string twoDigitsFormat(int num)
        {
            return num < 10 ? "0" + num : num + "";
        }


        public double JulianDate(int year, int month, int day)
        {
            if (month <= 2)
            {
                year -= 1;
                month += 12;
            }
            var A = Math.Floor(year/100.0);
            var B = 2 - A + Math.Floor(A/4);

            var JD = Math.Floor(365.25*(year + 4716)) + Math.Floor(30.6001*(month + 1)) + day + B - 1524.5;
            return JD;
        }




        public bool UseDayLightaving(int year, int month, int day)
        {
            return TimeZone.CurrentTimeZone.IsDaylightSavingTime(new DateTime(year, month, day));
        }


        public double dsin(double d)
        {
            return Math.Sin(DegreeToRadian(d));
        }

        public double dcos(double d)
        {
            return Math.Cos(DegreeToRadian(d));
        }

        public double dtan(double d)
        {
            return Math.Tan(DegreeToRadian(d));
        }

        public double darcsin(double x)
        {
            return RadianToDegree(Math.Asin(x));
        }

        public double darccos(double x)
        {
            return RadianToDegree(Math.Acos(x));
        }

        public double darctan(double x)
        {
            return RadianToDegree(Math.Atan(x));
        }

        public double darctan2(double y, double x)
        {
            return RadianToDegree(Math.Atan2(y, x));
        }

        public double darccot(double x)
        {
            return RadianToDegree(Math.Atan(1/x));
        }


        public double RadianToDegree(double radian)
        {
            return radian*180.0/Math.PI;
        }

        public double DegreeToRadian(double degree)
        {
            return degree*Math.PI/180.0;
        }

        public double FixAngle(double angel)
        {
            angel = angel - 360.0*Math.Floor(angel/360.0);
            angel = angel < 0 ? angel + 360.0 : angel;
            return angel;
        }

        public double FixHour(double hour)
        {
            hour = hour - 24.0*Math.Floor(hour/24.0);
            hour = hour < 0 ? hour + 24.0 : hour;
            return hour;
        }
    }
}
