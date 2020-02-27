using UnityEngine;

namespace LiteFramework.Core.BezierCurve
{
    public static class BezierCurveFactory
    {
        public static IBezierCurve CreateBezierCurve(Vector3 Begin, Vector3 End, BezierCurveModulateMode Mode = BezierCurveModulateMode.None)
        {
            return new BezierCurveCommon(CreateModulator(Mode), new[] {Begin, End});
        }

        public static IBezierCurve CreateBezierCurve(Vector3 Begin, Vector3 Control, Vector3 End, BezierCurveModulateMode Mode = BezierCurveModulateMode.None)
        {
            return new BezierCurveCommon(CreateModulator(Mode), new[] {Begin, Control, End});
        }

        public static IBezierCurve CreateBezierCurve(Vector3 Begin, Vector3 Control1, Vector3 Control2, Vector3 End, BezierCurveModulateMode Mode = BezierCurveModulateMode.None)
        {
            return new BezierCurveCommon(CreateModulator(Mode), new[] {Begin, Control1, Control2, End});
        }

        public static IBezierCurve CreateBezierCurve(Vector3[] Points, BezierCurveModulateMode Mode = BezierCurveModulateMode.None)
        {
            return new BezierCurveCommon(CreateModulator(Mode), Points);
        }
		
		private static IBezierCurveModulator CreateModulator(BezierCurveModulateMode Mode)
        {
            switch (Mode)
            {
                case BezierCurveModulateMode.None:
                    return null;
                case BezierCurveModulateMode.In:
                    return new BezierCurveModulatorIn();
                case BezierCurveModulateMode.Out:
                    return new BezierCurveModulatorOut();
                case BezierCurveModulateMode.InOut:
                    return new BezierCurveModulatorInOut();
                default:
                    return null;
            }
        }
    }
}