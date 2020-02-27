using UnityEngine;

namespace LiteFramework.Core.BezierCurve
{
    public enum BezierCurveModulateMode
    {
        None,
        In,
        Out,
        InOut,
    }

    public class BezierCurveModulatorIn : IBezierCurveModulator
    {
        public float Modulation(float Time)
        {
            return Time * Time;
        }
    }

    public class BezierCurveModulatorOut : IBezierCurveModulator
    {
        public float Modulation(float Time)
        {
            //return Mathf.Sqrt(Time);
            return Mathf.Pow(Time, 0.85f);
        }
    }

    public class BezierCurveModulatorInOut : IBezierCurveModulator
    {
        public float Modulation(float Time)
        {
            return ((Time * Time * Time) + (float)System.Math.Pow(Time, 0.33334f)) / 2.0f;
        }
    }
}