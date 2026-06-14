using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

public class SkyObject
// Contains functions to calculate coordinates of a body from RA, dec, and GST
{
    public string name { get; protected set; }

    protected double RA { get; set; }
    protected double dec { get; set; }

    protected double HA { get; set; }
    public double alt { get; protected set; }
    public double az { get; protected set; }

    public bool isVisible { get; protected set; }

    public Tuple<double, double> coordinates { get; protected set; }

    protected void calc_HA(double GST, double longitude)
    {
        double LST = (GST + longitude) % (2 * Math.PI); //LST = GST + longitude
        HA = (LST - RA) % (2 * Math.PI);
    }

    protected void calc_Alt(double latitude)
    {
        alt = Math.Asin((Math.Sin(latitude) * Math.Sin(dec)) + (Math.Cos(latitude) * Math.Cos(dec) * Math.Cos(HA)));
    }

    protected void check_visible()
    {
        if (alt > 0)
        {
            isVisible = true;
        }
        else
        {
            isVisible = false;
        }
    }

    protected void calc_Az(double latitude)
    {
        double sin_az = -((Math.Sin(HA) * Math.Cos(dec)) / Math.Cos(alt));
        double cos_az = (Math.Sin(dec) - (Math.Sin(latitude) * Math.Sin(alt))) / (Math.Cos(latitude) * Math.Cos(alt));

        if (cos_az != 0)
        {
            az = Math.Atan2(sin_az, cos_az);
        }
        else
        {
            az = 0;
        }
    }

    protected void calc_screen_coordinates()
    {
        double R = 90; //radius of sky map

        double r = 2 * R * Math.Tan((Math.PI / 4) - (alt / 2));

        double angle = az - (Math.PI / 2);

        double x = r * Math.Cos(angle);
        double y = r * Math.Sin(angle);

        coordinates = Tuple.Create(x, y);
    }

    public double deg_to_rad(double deg)
    {
        return deg * (Math.PI / 180);
    }

    public double rad_to_deg(double rad)
    {
        return rad * (180 / Math.PI);
    }

}

////////////////////////////////////////////////////////////////////////////////////////////////////

public class Star : SkyObject
{
    public double mag { get; protected set; }

    public Star(string name_in, double RA_in, double dec_in, double mag_in) : base()
    {
        if (name_in == "")
        {
            name = "Unnamed";
        }
        else
        {
            name = name_in;
        }

        RA = RA_in; // * (Math.PI / 180); //needs to be in radians
        dec = dec_in; // * (Math.PI / 180); // also in radians
        mag = mag_in;
    }

    public void calc_pos_1(double GST, double lon, double lat)
    {
        calc_HA(GST, lon);
        calc_Alt(lat);
        check_visible();
    }

    public void calc_pos_2(double lat)
    {
        calc_Az(lat);
        calc_screen_coordinates();
    }

}

////////////////////////////////////////////////////////////////////////////////////////////////////

class Sun : SkyObject
{
    private double mean_lon { get; set; }
    private double sun_at_epoch { get; set; }

    public Sun(double c_in) : base()
    {
        sun_at_epoch = c_in;
    }

    protected void calc_mean_lon(double sun_orbit_ang_vel, double days_since_epoch)
    {
        mean_lon = sun_at_epoch + (days_since_epoch * sun_orbit_ang_vel);
    }

    protected void calc_mean_anom()
    {

    }

    protected void calc_true_lon()
    {

    }

    protected void calc_obliquity()
    {

    }

    protected void calc_RA()
    {

    }

    protected void calc_dec()
    {

    }
}

class Moon : SkyObject
{
    public Moon() : base()
    {
    }
}

class Planet : SkyObject
{
    public Planet() : base()
    {
    }
}