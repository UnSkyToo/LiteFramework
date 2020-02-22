using UnityEngine;

namespace LiteFramework.Core.BezierCurve
{
    public interface IBezierCurve
    {
        Vector3 Lerp(float Time);
    }
}