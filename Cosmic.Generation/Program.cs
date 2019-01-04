using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Cosmic.Generation
{
    public class Constants
    {
        // Gravitational Constant. Units dyne-cm^2/g^2
        public const double G = 6.6720e-08;

        // Speed of Light in a Vacuum. Units cm/sec 
        public const double C = 2.9979e10;

        public static readonly List<double> KSteps = new List<double>() { 0.4, 0.7, 1.0, 1.6, 2.8, 5.2, 10.0, 19.6, 38.8, 77.2 };
    }

    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new Context(1546098698, 10000);
            var arenas = InitializeArenas(ctx);

            PrintListOfAreanas(arenas);
        }

        #region Export

        static void PrintListOfAreanas(List<Arena> arenas)
        {
            var data = string.Empty;

            foreach (var item in arenas)
            {
                data += string.Format("{0:00.000000} {1:00.000000} {2:00.000000} 128 128 128", item.Coordinates.X, item.Coordinates.Y, item.Coordinates.Z) + Environment.NewLine;
            }

            System.IO.File.WriteAllText("‪galaxy.txt", data);
        }

        #endregion

        #region Systems

        static List<Arena> InitializeArenas(Context ctx)
        {
            int numArms = ctx.Arms;
            double armSeparationDistance = 2 * Math.PI / numArms;
            double armOffsetMax = 0.5f;
            double rotationFactor = 5;
            double randomOffsetXY = 0.1f;

            var systems = new List<Arena>();

            for (int i = 0; i < ctx.Arenas; i++)
            {
                // Choose a distance from the center of the galaxy.
                double distance = ctx.Random.NextDouble();
                distance = Math.Pow(distance, 2);

                // Choose an angle between 0 and 2 * PI.
                double angle = ctx.Random.NextDouble() * 2 * Math.PI;
                double armOffset = ctx.Random.NextDouble() * armOffsetMax;
                armOffset = armOffset - armOffsetMax / 2;
                armOffset = armOffset * (1 / distance);

                double squaredArmOffset = Math.Pow(armOffset, 2);
                if (armOffset < 0) squaredArmOffset = squaredArmOffset * -1;
                armOffset = squaredArmOffset;

                double rotation = distance * rotationFactor;

                angle = (int)(angle / armSeparationDistance) * armSeparationDistance + armOffset + rotation;

                // Convert polar coordinates to 2D cartesian coordinates.
                double sX = Math.Cos(angle) * distance;
                double sY = Math.Sin(angle) * distance;
                double randomOffsetX = ctx.Random.NextDouble() * randomOffsetXY;
                double randomOffsetY = ctx.Random.NextDouble() * randomOffsetXY;

                sX += randomOffsetX;
                sY += randomOffsetY;

                double max = Math.Exp(-3 * distance);
                double sZ = ctx.Random.NextDouble() * max;

                int up = (ctx.Random.Next(100) > 50) ? 1 : -1;

                // Now we can assign xy coords.
                var x = (float)sX * ctx.Scale;
                var y = (float)sY * ctx.Scale;
                var z = (float)sZ * (ctx.Scale / 4) * up;

                // System
                var arena = new Arena
                {
                    Coordinates = new Vector3(x, y, z),
                    Name = ctx.Random.GenerateName(),
                };

                // Stars
                var starTypes = GetStarTypes(ctx, i);
                for (int s = 0; s < starTypes.Count; s++)
                {
                    var type = starTypes[s];
                    var name = starTypes.Count > 1 ? arena.Name + "-" + Convert.ToChar(65 + s) : arena.Name;
                    InitializeStar(ctx, type, name, arena);
                }

                // Zones
                AddZones(ctx, arena);
                var hz = arena.Zones.FirstOrDefault(_ => _.Type == Zones.Warm);

                // Planets
                var planetTypes = GetPlanetaryTypes(ctx);
                for (int p = 0; p < planetTypes.Count; p++)
                {
                    var item = planetTypes[p];
                    var name = arena.Name + "-" + (p + 1).ToString();
                    InitializePlanetary(ctx, name, item.type, item.distance, hz, arena);
                }

                // Moons
                var planetaryCollection = arena.Satellites.Where(_ => _.Type == EntityType.Planetary);
                foreach (var item in planetaryCollection)
                {

                }

                // Astriods

                systems.Add(arena);
            }

            return systems;
        }

        static void AddZones(Context ctx, Arena arena)
        {
            double flux = arena.Stars.Sum(_ => _.Luminosity);
            double Teff = arena.Stars.Sum(_ => _.Tempature);

            var Ts = 5700;
            var ai = 2.7619e-5;
            var bi = 3.8095e-9;
            var ao = 1.3786e-4;
            var bo = 1.4286e-9;
            var ris = 0.72;
            var ros = 1.77;
            var ri = (ris - (ai * (Teff - Ts)) - (bi * Math.Pow(Teff - Ts, 2))) * Math.Sqrt(flux);
            var ro = (ros - (ao * (Teff - Ts)) - (bo * Math.Pow(Teff - Ts, 2))) * Math.Sqrt(flux);

            arena.Zones.Add(new Zone() { Type = Zones.Hot, Inner = 0.0, Outer = ri });
            arena.Zones.Add(new Zone() { Type = Zones.Warm, Inner = ri, Outer = ro });
            arena.Zones.Add(new Zone() { Type = Zones.Cold, Inner = ro, Outer = 100.0 });
        }

        #endregion

        #region Stars

        static List<string> GetStarTypes(Context ctx, int index)
        {
            var arenaPercentage = index / (float)ctx.Arenas;

            var multiplicity = ctx.Random.NextDouble();

            if (arenaPercentage < 0.7645)
            {
                if(multiplicity < 0.26)
                    return new List<string>() { "M", "M" };
                else
                    return new List<string>() { "M" };

            }
            else if (arenaPercentage < 0.8855)
            {
                if (multiplicity < 0.44)
                    return new List<string>() { "K", "M" };
                else
                    return new List<string>() { "K" };
            }
            else if (arenaPercentage < 0.9615)
            {
                if (multiplicity < 0.45)
                    return new List<string>() { "G", "M" };
                else if (multiplicity < 0.50)
                    return new List<string>() { "G", "K" };
                else
                    return new List<string>() { "G" };
            }
            else if (arenaPercentage < 0.9915)
            {
                if (multiplicity < 0.45)
                    return new List<string>() { "F", "M" };
                else if (multiplicity < 0.50)
                    return new List<string>() { "F", "K" };
                else
                    return new List<string>() { "F" };
            }
            else if (arenaPercentage < 0.9975)
            {
                if (multiplicity < 0.20)
                    return new List<string>() { "A", "M" };
                else if (multiplicity < 0.50)
                    return new List<string>() { "A", "K" };
                else if (multiplicity < 0.60)
                    return new List<string>() { "A", "G" };
                else
                    return new List<string>() { "A" };
            }
            else if (arenaPercentage < 0.9988)
            {
                if (multiplicity < 0.20)
                    return new List<string>() { "B", "M" };
                else if (multiplicity < 0.50)
                    return new List<string>() { "B", "K" };
                else if (multiplicity < 0.60)
                    return new List<string>() { "B", "G" };
                else
                    return new List<string>() { "B" };
            }
            else
            {
                var multiple = multiplicity < 0.80 ? true : false;
                if (multiplicity < 0.20)
                    return new List<string>() { "O", "M" };
                else if (multiplicity < 0.40)
                    return new List<string>() { "O", "K" };
                else if (multiplicity < 0.50)
                    return new List<string>() { "O", "G" };
                else if (multiplicity < 0.60)
                    return new List<string>() { "O", "F" };
                else if (multiplicity < 0.70)
                    return new List<string>() { "O", "A" };
                else if (multiplicity < 0.80)
                    return new List<string>() { "O", "B" };
                else
                    return new List<string>() { "O" };
            }
        }

        static void InitializeStar(Context ctx, string classifcation, string name, Arena arena)
        {
            var p = ctx.Random.NextDouble();

            var star = new Star();
            star.Type = EntityType.Star;
            star.Name = name;
            star.Classifcation = classifcation;
            
            if (classifcation == "M")
            {
                star.Color = "#FFB56C";
                star.Tempature = new Range(2400, 3700).Percentage(p);
                star.Mass = new Range(0.08, 0.45).Percentage(p);
                star.Raduis = new Range(0.3, 0.7).Percentage(p);
                star.Luminosity = new Range(0, 0.08).Percentage(p);
            }
            else if (classifcation == "K")
            {
                star.Color = "#FFDAB5";
                star.Tempature = new Range(3700, 5200).Percentage(p);
                star.Mass = new Range(0.45, 0.8).Percentage(p);
                star.Raduis = new Range(0.7, 0.96).Percentage(p);
                star.Luminosity = new Range(0.08, 0.6).Percentage(p);
            }
            else if (classifcation == "G")
            {
                star.Color = "#FFEDE3";
                star.Tempature = new Range(5200, 6000).Percentage(p);
                star.Mass = new Range(0.8, 1.04).Percentage(p);
                star.Raduis = new Range(0.96, 1.15).Percentage(p);
                star.Luminosity = new Range(0.6, 1.5).Percentage(p);
            }
            else if (classifcation == "F")
            {
                star.Color = "#F9F5FF";
                star.Tempature = new Range(6000, 7500).Percentage(p);
                star.Mass = new Range(1.04, 1.4).Percentage(p);
                star.Raduis = new Range(1.15, 1.4).Percentage(p);
                star.Luminosity = new Range(1.5, 5).Percentage(p);
            }
            else if (classifcation == "A")
            {
                star.Color = "#D5E0FF";
                star.Tempature = new Range(7500, 10000).Percentage(p);
                star.Mass = new Range(1.4, 2.1).Percentage(p);
                star.Raduis = new Range(1.4, 1.8).Percentage(p);
                star.Luminosity = new Range(5, 25).Percentage(p);
            }
            else if (classifcation == "B")
            {
                star.Color = "#A2C0FF";
                star.Tempature = new Range(10000, 30000).Percentage(p);
                star.Mass = new Range(2.1, 16).Percentage(p);
                star.Raduis = new Range(1.8, 6.6).Percentage(p);
                star.Luminosity = new Range(25, 30000).Percentage(p);
            }
            else if(classifcation == "O")
            {
                star.Color = "#92B5FF";
                star.Tempature = new Range(30000, 50000).Percentage(p);
                star.Mass = new Range(16, 20).Percentage(p);
                star.Raduis = new Range(6.6, 10).Percentage(p);
                star.Luminosity = new Range(30000, 50000).Percentage(p);
            }

            star.Mass = star.Mass * 1.9891e30;
            star.Raduis = star.Raduis * 6.96265e5;
            // star.Luminosity = star.Luminosity * 3.827e25;
            star.Gravity = (Constants.G * star.Mass) / Math.Pow(star.Raduis, 2);

            arena.Stars.Add(star);
        }

        #endregion

        #region Satellites

        static List<(string type, double distance)> GetPlanetaryTypes(Context ctx)
        {
            var classifications = new List<string>() { "", "M", "T", "N", "J" };
            var collection = new List<(string type, double distance)>();

            foreach (var dis in Constants.KSteps)
            {
                var index = ctx.Random.Next(0, 5);
                var item = classifications[index];
                if(!string.IsNullOrWhiteSpace(item))
                {
                    collection.Add((item, dis));
                }
            }

            return collection;
        }

        static void InitializePlanetary(Context ctx, string name, string classifcation, double orbit, Zone hz, Arena arena)
        {
            var p = ctx.Random.NextDouble();
            var p1 = ctx.Random.NextDouble();
            var p2 = ctx.Random.NextDouble();
            var p3 = ctx.Random.NextDouble();
            var p4 = ctx.Random.NextDouble();

            var planet = new Satellite();
            planet.Type = EntityType.Planetary;
            planet.Name = name;
            planet.Classifcation = classifcation;

            double A = 0.3; // [0-1] bond albedo
            double f = 0.5; // [0-1..2] atmosphere redistribution factor (e.g. f = 1 for fast rotators and f = 2 for tidally locked planets without atmospheres)
            double g = 0.3; // [0-1] normalized greenhouse effect 

            if (classifcation == "M")
            {
                planet.Mass = new Range(0, 0.1).Percentage(p);
                planet.Raduis = new Range(0.03, 0.7).Percentage(p);

                A = new Range(0, 0.1).Percentage(p1);
                f = (p2 > 0.3) ? new Range(0, 1).Percentage(p3) : 2;
                g = new Range(0.01, 1).Percentage(p4);
            }
            else if (classifcation == "T")
            {
                planet.Mass = new Range(0.1, 10).Percentage(p);
                planet.Raduis = new Range(0.5, 3.3).Percentage(p);

                A = new Range(0.1, 0.9).Percentage(p1);
                f = (p2 > 0.3) ? new Range(0, 1).Percentage(p3) : 2;
                g = new Range(0.01, 1).Percentage(p4);
            }
            else if (classifcation == "N")
            {
                planet.Mass = new Range(10, 50).Percentage(p);
                planet.Raduis = new Range(2.1, 5.7).Percentage(p);

                A = new Range(0.3, 0.6).Percentage(p1);
                f = (p2 > 0.3) ? new Range(0, 1).Percentage(p3) : 2;
                g = new Range(0.01, 1).Percentage(p4);
            }
            else if (classifcation == "J")
            {
                planet.Mass = new Range(50, 5000).Percentage(p);
                planet.Raduis = new Range(3.5, 27).Percentage(p);

                A = new Range(0.4, 0.8).Percentage(p1);
                f = (p2 > 0.3) ? new Range(0, 1).Percentage(p3) : 2;
                g = new Range(0.01, 1).Percentage(p4);
            }

            double flux = arena.Stars.Sum(_ => _.Luminosity);
            double Teff = arena.Stars.Sum(_ => _.Tempature);
            double mass = planet.Mass;
            double raduis = planet.Raduis;

            planet.Zone = arena.Zones.Where(_ => orbit >= _.Inner && orbit < _.Outer).Select(_ => _.Type).FirstOrDefault();
            planet.Orbit = orbit;
            planet.Mass = planet.Mass * 5.9736e24;
            planet.Raduis = planet.Raduis * 6371;
            planet.Gravity = (Constants.G * planet.Mass) / Math.Pow(planet.Raduis, 2);

            planet.SurfaceTemperature = 278.5 * Math.Pow((f * flux * (1.0 - A)) / (Math.Pow(planet.Orbit, 2) * (1.0 - g)), (1.0 / 4.0));

            planet.HZD = ((2 * planet.Orbit) - hz.Outer - hz.Inner) / (hz.Outer - hz.Inner);

            double rIron = Math.Pow(2.52 * 10, (-0.209490 + (1.0 / 3.0) * Math.Log(mass / 5.80) - 0.0804 * Math.Pow(mass / 5.80, 0.394)));
            double rWater = Math.Pow(4.43 * 10, (-0.209396 + (1.0 / 3.0) * Math.Log(mass / 5.52) - 0.0807 * Math.Pow(mass / 5.52, 0.375)));
            planet.HZC = mass < 20.0 ? ((2 * raduis) - rWater - rIron) / (rWater - rIron) : 0.0;

            double ven = Math.Sqrt(2e-2 * planet.SurfaceTemperature / 14);
            double veh = Math.Sqrt(2e-2 * planet.SurfaceTemperature / 1);
            planet.HZA = 2 * Math.Sqrt(mass / raduis) - veh - ven / (veh - ven);

            arena.Satellites.Add(planet);
        }

        #endregion
    }
}
