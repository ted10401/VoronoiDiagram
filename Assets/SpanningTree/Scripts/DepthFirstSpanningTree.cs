using UnityEngine;
using System.Collections.Generic;

namespace SpanningTree
{
    public class DepthFirstSpanningTree : BaseSpanningTree
    {
        private Stack<Vector3> m_processStack;
        private Dictionary<Vector3, List<STSegment>> m_neighborSegments;

        public DepthFirstSpanningTree(STEdge[] stEdges) : base(stEdges)
        {
        }

        protected override void AfterUpdateSegments()
        {
            m_neighborSegments = new Dictionary<Vector3, List<STSegment>>();

            Vector3 pointA;
            Vector3 pointB;
            for(int i = 0; i < m_segments.Count - 1; i++)
            {
                pointA = m_segments[i].PointA;
                pointB = m_segments[i].PointB;

                if(!m_neighborSegments.ContainsKey(pointA))
                {
                    m_neighborSegments.Add(pointA, new List<STSegment>() { m_segments[i] });
                }
                else
                {
                    m_neighborSegments[pointA].Add(m_segments[i]);
                }

                if (!m_neighborSegments.ContainsKey(pointB))
                {
                    m_neighborSegments.Add(pointB, new List<STSegment>() { m_segments[i] });
                }
                else
                {
                    m_neighborSegments[pointB].Add(m_segments[i]);
                }
            }
        }

        protected override void Algorithm()
        {
            m_processStack = new Stack<Vector3>();

            int cacheRootA;
            int cacheRootB;
            STSegment cacheSegment;
            m_processStack.Push(InputPoints[Random.Range(0, InputPoints.Count)]);

            while (m_processStack.Count != 0)
            {
                cacheSegment = GetRandomSegment(m_processStack.Peek());
                if(cacheSegment == null)
                {
                    m_processStack.Pop();
                    continue;
                }

                cacheRootA = FindRoot(cacheSegment.PointAIndex);
                cacheRootB = FindRoot(cacheSegment.PointBIndex);

                if (cacheRootA != cacheRootB)
                {
                    UnionSet(cacheSegment.PointAIndex, cacheSegment.PointBIndex);
                    Segments.Add(cacheSegment);
                }

                RemoveSegment(cacheSegment);
                AddProcess(cacheSegment);
            }
        }

        private void RemoveSegment(STSegment segment)
        {
            m_neighborSegments[segment.PointA].Remove(segment);
            m_neighborSegments[segment.PointB].Remove(segment);
        }

        private STSegment GetRandomSegment(Vector3 point)
        {
            if(!m_neighborSegments.ContainsKey(point))
            {
                return null;
            }

            List<STSegment> segments = m_neighborSegments[point];
            if(segments.Count == 0)
            {
                return null;
            }

            return segments[Random.Range(0, segments.Count)];
        }

        private void AddProcess(STSegment segment)
        {
            if(m_neighborSegments[segment.PointA].Count == 0 &&
                m_neighborSegments[segment.PointB].Count == 0)
            {
                return;
            }

            Vector3 lastPoint = m_processStack.Peek();
            if(segment.PointA != lastPoint && m_neighborSegments[segment.PointA].Count != 0)
            {
                m_processStack.Push(segment.PointA);
            }
            else if (segment.PointB != lastPoint && m_neighborSegments[segment.PointB].Count != 0)
            {
                m_processStack.Push(segment.PointB);
            }
        }
    }
}
