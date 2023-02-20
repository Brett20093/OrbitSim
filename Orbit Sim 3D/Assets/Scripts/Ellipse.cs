using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Ellipse : Orbit {
    double b; // small width of orbit

    double n;
    double ecc_anom;
    double period;
    public double omega;
    public double raan;
    public double inc;
    private double theta;

    double time = 0.0;

    public double tol = 0.00001;

    void Start() {
        planetScript = planet.GetComponent<Planet>();
        omega *= Globals.degToRad;
        raan *= Globals.degToRad;
        inc *= Globals.degToRad;
        GivenRpRa(rp, ra);
        theta = omega + true_anom;
    }

    void Update() {
        time += Time.deltaTime*timeMultiplier;
        time = time % period;
        KeplerSolve(time);
        CalculateRadius();
        double x = r * (Math.Cos(raan) * Math.Cos(theta) - Math.Sin(raan) * Math.Sin(theta) * Math.Cos(inc)) + planet.transform.position.x;
        double y = r * (Math.Sin(theta) * Math.Sin(inc)) + planet.transform.position.y;
        double z = r * (Math.Sin(raan) * Math.Cos(theta) + Math.Cos(raan) * Math.Sin(theta) * Math.Cos(inc)) + planet.transform.position.z;
        transform.position = new Vector3((float)x, (float)y, (float)z);
    }

    private void CalculateRadius() {
        r = (a * (1 - Math.Pow(e, 2))) / (1 + e * Math.Cos(true_anom));
    }

    private double EccentricityQuadraticFormula(double a_q, double b_q, double c_q) {
        double e1, e2;
        e1 = (-b_q + Math.Sqrt(Math.Pow(b_q, 2) - 4 * a_q * c_q)) / (2 * a);
        e2 = (-b_q - Math.Sqrt(Math.Pow(b_q, 2) - 4 * a_q * c_q)) / (2 * a);

        // TODO: this is not the best, if e1 is exactly 1, then it will return e1
        if (e2 >= 0.0f && e2 < 1.0f)
            return e2;
        else if (e1 >= 0.0f && e1 < 1.0f)
            return e1;
        else
            return -1.0;
    }

    private void GivenPosVeloSolve() {
        eng = (Math.Pow(velo, 2) / 2) - (planetScript.mew / r);
        a = -1.0*planetScript.mew / (2 * eng);
        n = Math.Sqrt(planetScript.mew / Math.Pow(a, 3));
        period = (2.0 * Math.PI) / n;

        double a_q = a;
        double b_q = r * Math.Cos(true_anom);
        double c_q = r - a;

        e = EccentricityQuadraticFormula(a_q, b_q, c_q);

        b = a * Math.Sqrt(1 - Math.Pow(e, 2));
    }

    private void GivenRpRaSolve() {
        a = 0.5 * (ra + rp);
        e = (ra / a) - 1;
        b = a * Math.Sqrt(1 - Math.Pow(e, 2));
        eng = -planetScript.mew / (2.0 * a);
        n = Math.Sqrt(planetScript.mew / Math.Pow(a, 3));
        period = (2.0 * Math.PI) / n;
    }

    private void EccToTrueAnom() {
        true_anom = 2.0 * Math.Atan(Math.Sqrt((1.0 + e) / (1.0 - e)) * Math.Tan(ecc_anom / 2.0));
        theta = omega + true_anom;
    }

    public void GivenPosVelo(double true_anomaly, double radius, double velocity) {
        true_anom = true_anomaly;
        r = radius;
        velo = velocity;

        GivenPosVeloSolve();
    }

    public void GivenRpRa(double r_p, double r_a) {
        rp = r_p;
        ra = r_a;
        true_anom = 0.0f;

        GivenRpRaSolve();
    }

    public void KeplerSolve(double t) {
        double nt_tp = n * t;
        double ecc_n = n * t;
        double check = ecc_n - e * Math.Sin(ecc_n);

        int i = 0;
        while (Math.Abs(nt_tp - check) > tol && i < 10) {
            ecc_n = ecc_n - (ecc_n - e * Math.Sin(ecc_n) - nt_tp) / (1 - e * Math.Cos(ecc_n));
            check = ecc_n - e * Math.Sin(ecc_n);
            i++;
        }
        ecc_anom = ecc_n;
        EccToTrueAnom();
    }
}
