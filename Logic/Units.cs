using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MozaicLand
{
    public interface IUnit
    {
        double Value { get; set; }
        double ToBaseUnit();
        void FromBaseUnit(double value);
    }

    public static class Units
    {
        public struct non : IUnit
        {
            public double Value { get; set; }

            public static implicit operator non(double value) { return new non() { Value = value }; }
            public static implicit operator double(non value) { return value.Value; }

            public double ToBaseUnit() { return this; }
            public void FromBaseUnit(double value) { Value = value; }
            public override string ToString() { return Value.ToString("F4"); }
        }

        public interface IDistanceUnit : IUnit
        {
        }

        public struct m : IDistanceUnit
        {
            public double Value { get; set; }

            public static implicit operator m(double value) { return new m { Value = value }; }
            public static implicit operator double(m value) { return value.Value; }

            public static implicit operator cm(m value) { return (cm)(value.Value * 100.0); }
            public static implicit operator mm(m value) { return (mm)(value.Value * 1000.0); }

            public static m operator +(m a, IDistanceUnit b) { return a.Value + b.ToBaseUnit(); }
            public static m operator *(m a, non b) { return (m)(a.Value * b.Value); }
            public static m operator *(non a, m b) { return (m)(a.Value * b.Value); }
            public static m operator /(m a, non b) { return (m)(a.Value / b.Value); }
            public static m operator /(non a, m b) { return (m)(a.Value / b.Value); }

            public static bool operator ==(m a, IDistanceUnit b) { return a.Value == b.ToBaseUnit(); }
            public static bool operator !=(m a, IDistanceUnit b) { return a.Value != b.ToBaseUnit(); }

            public double ToBaseUnit() { return this; }
            public void FromBaseUnit(double value) { Value = value; }
            public override string ToString() { return Value.ToString("F4") + " m"; }
        }

        public struct cm : IDistanceUnit
        {
            public double Value { get; set; }

            public static explicit operator cm(double value) { return new cm { Value = value }; }
            public static explicit operator double(cm value) { return value.Value; }

            public static implicit operator m(cm value) { return (m)(value.Value * 0.01); }
            public static implicit operator mm(cm value) { return (mm)(value.Value * 10.0); }

            public static cm operator +(cm a, IDistanceUnit b) { return (cm)((a.ToBaseUnit() + b.ToBaseUnit()) * 0.01); }
            public static cm operator *(cm a, non b) { return (cm)(a.Value * b.Value); }
            public static cm operator *(non a, cm b) { return (cm)(a.Value * b.Value); }
            public static cm operator /(cm a, non b) { return (cm)(a.Value / b.Value); }
            public static cm operator /(non a, cm b) { return (cm)(a.Value / b.Value); }

            public static bool operator ==(cm a, IDistanceUnit b) { return a.ToBaseUnit() == b.ToBaseUnit(); }
            public static bool operator !=(cm a, IDistanceUnit b) { return a.ToBaseUnit() != b.ToBaseUnit(); }

            public double ToBaseUnit() { return (m)this; }
            public void FromBaseUnit(double value) { this = (m)value; }
            public override string ToString() { return Value.ToString("F4") + " cm"; }
        }

        public struct mm : IDistanceUnit
        {
            public double Value { get; set; }

            public static explicit operator mm(double value) { return new mm { Value = value }; }
            public static explicit operator double(mm value) { return value.Value; }

            public static implicit operator m(mm value) { return (m)(value.Value * 0.001); }
            public static implicit operator cm(mm value) { return (cm)(value.Value * 0.1); }

            public static mm operator +(mm a, IDistanceUnit b) { return (mm)((a.ToBaseUnit() + b.ToBaseUnit()) * 0.001); }
            public static mm operator *(mm a, non b) { return (mm)(a.Value * b.Value); }
            public static mm operator *(non a, mm b) { return (mm)(a.Value * b.Value); }
            public static mm operator /(mm a, non b) { return (mm)(a.Value / b.Value); }
            public static mm operator /(non a, mm b) { return (mm)(a.Value / b.Value); }

            public static bool operator ==(mm a, IDistanceUnit b) { return a.ToBaseUnit() == b.ToBaseUnit(); }
            public static bool operator !=(mm a, IDistanceUnit b) { return a.ToBaseUnit() != b.ToBaseUnit(); }

            public double ToBaseUnit() { return (m)this; }
            public void FromBaseUnit(double value) { this = (m)value; }
            public override string ToString() { return Value.ToString("F4") + " mm"; }
        }
    }
}
