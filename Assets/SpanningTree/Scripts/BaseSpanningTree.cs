using UnityEngine;
using System.Collections.Generic;

namespace SpanningTree
{
    public abstract class BaseSpanningTree
    {
        public List<STSegment> Segments = new List<STSegment>();
        public List<Vector3> InputPoints { get { return m_inputPoints; } }

        private STEdge[] m_edges = null;
        private List<Vector3> m_inputPoints;
        private int[] m_inputPointSet;
        protected List<STSegment> m_segments = new List<STSegment>();

        public BaseSpanningTree(STEdge[] stEdges)
        {
            m_edges = stEdges;

            UpdateInputPoints();
            UpdateInputPointSet();
            UpdateSegments();
            StartAlgorithm();
        }

        private void UpdateInputPoints()
        {
            if(m_inputPoints == null)
            {
                m_inputPoints = new List<Vector3>();
            }
            else
            {
                m_inputPoints.Clear();
            }

            for (int i = 0; i < m_edges.Length; i++)
            {
                if (!m_inputPoints.Contains(m_edges[i].PointA))
                {
                    m_inputPoints.Add(m_edges[i].PointA);
                }

                if (!m_inputPoints.Contains(m_edges[i].PointB))
                {
                    m_inputPoints.Add(m_edges[i].PointB);
                }
            }
        }

        private void UpdateInputPointSet()
        {
            m_inputPointSet = new int[m_inputPoints.Count];
            for (int i = 0; i < m_inputPointSet.Length; i++)
            {
                m_inputPointSet[i] = -1;
            }
        }

        private void UpdateSegments()
        {
            STSegment cacheSegment;

            m_segments.Clear();
            for (int i = 0; i < m_edges.Length; i++)
            {
                cacheSegment = new STSegment(m_edges[i].PointA, m_edges[i].PointB, GetPointIndex(m_edges[i].PointA), GetPointIndex(m_edges[i].PointB));
                m_segments.Add(cacheSegment);
            }

            AfterUpdateSegments();
        }

        private int GetPointIndex(Vector3 point)
        {
            int index = -1;
            for (int i = 0; i < m_inputPoints.Count; i++)
            {
                if (point == m_inputPoints[i])
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        protected abstract void AfterUpdateSegments();

        private void StartAlgorithm()
        {
            Segments.Clear();
            Algorithm();
        }

        protected abstract void Algorithm();

        protected void UnionSet(int indexA, int indexB)
        {
            int rootA = FindRoot(indexA);
            int rootB = FindRoot(indexB);

            if (m_inputPointSet[rootA] <= m_inputPointSet[rootB])
            {
                m_inputPointSet[rootA] += m_inputPointSet[rootB];
                m_inputPointSet[rootB] = rootA;
            }
            else
            {
                m_inputPointSet[rootB] += m_inputPointSet[rootA];
                m_inputPointSet[rootA] = rootB;
            }
        }

        protected int FindRoot(int index)
        {
            int root = m_inputPointSet[index];
            while (root >= 0)
            {
                index = root;
                root = m_inputPointSet[index];
            }

            return index;
        }
    }
}
