using System;
using System.Collections.Generic;
using System.Numerics;
namespace Cosmic.Generation
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new Context(1546098698, 10000);
            var arenas = InitializeArenas(ctx);

            PrintListOfAreanas(arenas);
        }


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
                var p = i / (float)ctx.Arenas;

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
                var z = (float)sZ * (ctx.Scale / 8) * up;

                var system = new Arena
                {
                    Coordinates = new Vector3(x, y, z),
                    Name = ctx.Random.GenerateName(),
                };

                // Get star type for aerna and if there is a secondar star get it's type

                InitializeStars(ctx, p, system);

                systems.Add(system);
            }

            return systems;
        }

        static List<string> GetStarTypes(Context ctx, double arenaPercentage)
        {
            var multiplicity = ctx.Random.NextDouble();

            if (arenaPercentage < 76.45)
            {
                var multiple = multiplicity < 0.26 ? true : false;
                return new List<string>() { "M" };
            }
            else if (arenaPercentage < 88.55)
            {
                var multiple = multiplicity < 0.44 ? true : false;
                return new List<string>() { "K" };
            }
            else if (arenaPercentage < 96.15)
            {
                var multiple = multiplicity < 0.50 ? true : false;
                return new List<string>() { "G" };
            }
            else if (arenaPercentage < 99.15)
            {
                var multiple = multiplicity < 0.50 ? true : false;
                return new List<string>() { "F" };
            }
            else if (arenaPercentage < 99.75)
            {
                var multiple = multiplicity < 0.60 ? true : false;
                return new List<string>() { "A" };
            }
            else if (arenaPercentage < 99.88)
            {
                var multiple = multiplicity < 0.60 ? true : false;
                return new List<string>() { "B" };
            }
            else
            {
                var multiple = multiplicity < 0.80 ? true : false;
                return new List<string>() { "O" };
            }
        }

        static void InitializeStars(Context ctx, double arenaPercentage, Arena arena)
        {
            var star = new CosmisBody();
            star.Type = EntityType.Star;
            star.Name = arena.Name;

            var starPercentage = ctx.Random.NextDouble();

            if (arenaPercentage < 76.45)
            {
                star.Classifcation = "M";
                star.Color = "#FFB56C";

                star.Tempature = new Range(2400, 3700);
            }
            else if (arenaPercentage < 88.55)
            {
                star.Classifcation = "K";
                star.Color = "#FFDAB5";

                star.Tempature = new Range(3700, 5200);
            }
            else if (arenaPercentage < 96.15)
            {
                star.Classifcation = "G";
                star.Color = "#FFEDE3";

                star.Tempature = new Range(5200, 6000);
            }
            else if (arenaPercentage < 99.15)
            {
                star.Classifcation = "F";
                star.Color = "#F9F5FF";

                star.Tempature = new Range(6000, 7500);
            }
            else if (arenaPercentage < 99.75)
            {
                star.Classifcation = "A";
                star.Color = "#D5E0FF";

                star.Tempature = new Range(7500, 10000);
            }
            else if (arenaPercentage < 99.88)
            {
                star.Classifcation = "B";
                star.Color = "#A2C0FF";

                star.Tempature = new Range(10000, 30000);
            }
            else
            {
                star.Classifcation = "O";
                star.Color = "#92B5FF";

                star.Tempature = new Range(30000, 50000);
            }

            arena.Satellites.Add(star);
        }

        static void PrintListOfAreanas(List<Arena> arenas)
        {
            var data = string.Empty;

            foreach (var item in arenas)
            {
                data += string.Format("{0:00.000000} {1:00.000000} {2:00.000000} 128 128 128", item.Coordinates.X, item.Coordinates.Y, item.Coordinates.Z) + Environment.NewLine;
            }

            System.IO.File.WriteAllText("‪galaxy.xzy", data);
        }
    }
}
