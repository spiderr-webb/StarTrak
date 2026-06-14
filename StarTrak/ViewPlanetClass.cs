using System;

class ViewPlanet
{
    public double days_per_yr { get; private set; }
    public double hours_per_solar { get; private set; }
    public double greenwich_at_epoch { get; private set; }

    public double sun_orbit_ang_vel { get; private set; }

    public string filePath { get; private set; }

    public ViewPlanet(double days_yr, double hours_sol, double c_in, string filePath_in)
    {
        //b = hours_sol / ((360 * hours_sol) / ((360 / days_yr) + 360));
        //a = (b - 1) * hours_sol;
        //c = c_in;

        days_per_yr = days_yr;
        hours_per_solar = hours_sol;
        greenwich_at_epoch = c_in;

        filePath = filePath_in;

        sun_orbit_ang_vel = (2 * Math.PI) / days_per_yr;
    }

    public double calc_utc_h(double local_H, double lon)
    {
        double utc_H = (local_H - Math.Round(lon / (Math.PI / 12))) % hours_per_solar;

        return utc_H;
    }

    public double calc_GST(double D, double H)
    {
        //double GST_h = (((a * D) + (b * H)) % 24); // GST in hours mod 24
        //double GST = (2 * Math.PI) * (GST_h / 24); // convert to radians

        double sidereal_per_solar = 1.0 + (1.0 / days_per_yr);

        double total_sol_hours = (D * hours_per_solar) + H;

        double GST_h = (greenwich_at_epoch + (sidereal_per_solar * total_sol_hours) % hours_per_solar);
        double GST = (2 * Math.PI) * (GST_h / hours_per_solar); // convert to radians

        return GST;
    }

    public double calc_days_since_epoch(double D, double H)
    {
        double total_sol_days = D + (H / hours_per_solar);

        return total_sol_days;
    }

}