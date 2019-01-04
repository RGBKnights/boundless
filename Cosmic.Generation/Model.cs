using System;
using System.Collections.Generic;
using System.Numerics;

namespace Cosmic.Generation
{
    public class Context
    {
        public int Seed { get; private set; }
        public int Arenas { get; private set; }
        public Random Random { get; private set; }

        public int Arms { get; set; }
        public int Scale { get; set; }

        public Context(int seed, int amount)
        {
            this.Seed = seed;
            this.Random = new Random(seed);
            this.Arenas = amount;
            this.Arms = 7;
            this.Scale = 100;
        }
    }

    public class Arena
    {
        public Vector3 Coordinates { get; set; }

        public string Name { get; set; }

        public List<Zone> Zones { get; set; }
        public List<Star> Stars { get; set; }
        public List<Satellite> Satellites { get; set; }

        public Arena()
        {
            this.Coordinates = new Vector3();
            this.Name = String.Empty;
            this.Zones = new List<Zone>();
            this.Stars = new List<Star>();
            this.Satellites = new List<Satellite>();
        }

        public override string ToString()
        {
            return string.Format("{0} [[1}]", this.Name, this.Stars.Count + 1);
        }
    }

    public class Star
    {
        public string Name { get; set; }
        public EntityType Type { get; set; }
        public string Classifcation { get; set; }
        public double Mass { get; set; }
        public double Raduis { get; set; }
        public double Gravity { get; set; }
        public double Tempature { get; set; }
        public string Color { get; set; }
        public double Luminosity { get; set; }

        public Star()
        {
            this.Type = EntityType.Unknown;
            this.Name = string.Empty;
            this.Classifcation = string.Empty;
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", this.Classifcation, this.Name);
        }
    }

    public class Satellite
    {
        public string Name { get; set; }
        public EntityType Type { get; set; }
        public string Classifcation { get; set; }
        public double Mass { get; set; }
        public double Raduis { get; set; }
        public double Gravity { get; set; }

        public Zones Zone { get; set; }
        public double Orbit { get; set; }
        public double SurfaceTemperature { get; set; }

        public double HZD { get; set; }
        public double HZC { get; set; }
        public double HZA { get; set; }

        public Satellite()
        {
            this.Type = EntityType.Unknown;
            this.Name = string.Empty;
            this.Classifcation = string.Empty;
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1} - {2}", this.Classifcation, this.Name, Enum.GetName(typeof(Zones), this.Zone));
        }
    }

    public enum EntityType
    {
        Unknown = 0,
        Star = 1,
        Planetary = 2,
        Moon = 3,
    }

    public enum Zones
    {
        Unknown = 0,
        Hot = 1,
        Warm = 2,
        Cold = 3,
    }
   
    public class Zone
    {
        public Zones Type { get; set; }
        public double Inner { get; set; }
        public double Outer { get; set; }

        public Zone()
        {
            this.Type = Zones.Unknown;
        }
    }

    public class Range
    {
        public double Min { get; set; }
        public double Max { get; set; }

        public Range()
        {
        }

        public Range(double min, double max)
        {
            this.Min = min;
            this.Max = max;
        }

        public double Percentage(double input)
        {
            return ((input * 100) * (this.Max - this.Min) / 100) + this.Min;
        }
    }
}

