using UnityEngine;

namespace MST
{
    public class MSTSegment
    {
        public Vector3 PointA;
        public Vector3 PointB;
        public int PointAIndex;
        public int PointBIndex;
        public float Weight;

        public MSTSegment(Vector3 pointA, Vector3 pointB, int pointAIndex, int pointBIndex)
        {
            PointA = pointA;
            PointB = pointB;
            PointAIndex = pointAIndex;
            PointBIndex = pointBIndex;
            Weight = Vector3.Distance(pointA, pointB);
        }
    }
}