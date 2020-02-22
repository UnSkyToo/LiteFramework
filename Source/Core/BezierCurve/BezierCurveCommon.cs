using UnityEngine;

namespace LiteFramework.Core.BezierCurve
{
    public class BezierCurveCommon : IBezierCurve
    {
        private readonly Vector3[] Points_;
        private readonly Vector3[] Controller_;

        public BezierCurveCommon(Vector3[] Points)
        {
            if (Points != null && Points.Length > 1)
            {
                Points_ = Points;
                Controller_ = new Vector3[Points.Length];

                for (var Index = 0; Index < Controller_.Length; ++Index)
                {
                    Controller_[Index] = new Vector3();
                }
            }
        }

        public Vector3 Lerp(float Time)
        {
            if (Points_ == null)
            {
                return Vector3.zero;
            }

            var Count = GeneratorController(Points_, Points_.Length, Time);
            while (Count > 1)
            {
                Count = GeneratorController(Controller_, Count, Time);
            }
            return Controller_[0];
        }

        private int GeneratorController(Vector3[] Points, int Count, float Time)
        {
            var Index = 0;
            for (var Offset = 0; Offset < Count - 1; ++Offset)
            {
                Controller_[Index].x = (Points[Offset + 1].x - Points[Offset].x) * Time + Points[Offset].x;
                Controller_[Index].y = (Points[Offset + 1].y - Points[Offset].y) * Time + Points[Offset].y;
                Controller_[Index].z = (Points[Offset + 1].z - Points[Offset].z) * Time + Points[Offset].z;
                Index++;
            }
            return Index;
        }
    }
}