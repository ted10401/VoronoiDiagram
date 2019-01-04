using UnityEngine;
using System.Collections.Generic;

namespace SpanningTree
{
    public class STSegment
    {
        public Vector3 PointA;
        public Vector3 PointB;
        public int PointAIndex;
        public int PointBIndex;
        public float Weight;
        public List<STSegment> NeighborSegments;

        public STSegment(Vector3 pointA, Vector3 pointB, int pointAIndex, int pointBIndex)
        {
            PointA = pointA;
            PointB = pointB;
            PointAIndex = pointAIndex;
            PointBIndex = pointBIndex;
            Weight = Vector3.Distance(pointA, pointB);

            NeighborSegments = new List<STSegment>();
        }

        public void AddNeighbor(STSegment segment)
        {
            if(NeighborSegments.Contains(segment))
            {
                return;
            }

            NeighborSegments.Add(segment);
        }

        public Vector3 GetCenter()
        {
            return (PointA + PointB) / 2;
        }
    }
}