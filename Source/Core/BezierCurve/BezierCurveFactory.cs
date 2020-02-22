using UnityEngine;

namespace LiteFramework.Core.BezierCurve
{
    public static class BezierCurveFactory
    {
        public static IBezierCurve CreateBezierCurve(Vector3 Begin, Vector3 End)
        {
            return new BezierCurveCommon(new[] {Begin, End});
        }

        public static IBezierCurve CreateBezierCurve(Vector3 Begin, Vector3 Control, Vector3 End)
        {
            return new BezierCurveCommon(new[] {Begin, Control, End});
        }

        public static IBezierCurve CreateBezierCurve(Vector3 Begin, Vector3 Control1, Vector3 Control2, Vector3 End)
        {
            return new BezierCurveCommon(new[] {Begin, Control1, Control2, End});
        }

        public static IBezierCurve CreateBezierCurve(Vector3[] Points)
        {
            return new BezierCurveCommon(Points);
        }
    }
}