using UnityEngine;
using System.Collections.Generic;

namespace MST
{
    public class MinimumSpanningTree
    {
        public List<Vector3> InputPointList = new List<Vector3>();
        public List<MSTSegment> MSTSegments = new List<MSTSegment>();

        private MSTEdge[] m_mstEdges = null;
        private int[] m_inputPointSet;
        private List<MSTSegment> m_increaseWeight = new List<MSTSegment>();

        public MinimumSpanningTree(MSTEdge[] mstEdges)
        {
            m_mstEdges = mstEdges;

            UpdateInputPointList();
            UpdateInputPointSet();
            UpdateIncreaseWeight();
            StartAlgorithm();
        }

        private void UpdateInputPointList()
        {
            InputPointList.Clear();
            for (int i = 0; i < m_mstEdges.Length; i++)
            {
                if (!InputPointList.Contains(m_mstEdges[i].PointA))
                {
                    InputPointList.Add(m_mstEdges[i].PointA);
                }

                if (!InputPointList.Contains(m_mstEdges[i].PointB))
                {
                    InputPointList.Add(m_mstEdges[i].PointB);
                }
            }
        }

        private void UpdateInputPointSet()
        {
            m_inputPointSet = new int[InputPointList.Count];
            for (int i = 0; i < m_inputPointSet.Length; i++)
            {
                m_inputPointSet[i] = -1;
            }
        }

        private void UpdateIncreaseWeight()
        {
            m_increaseWeight.Clear();
            for (int i = 0; i < m_mstEdges.Length; i++)
            {
                MSTSegment cacheSegment = new MSTSegment(m_mstEdges[i].PointA, m_mstEdges[i].PointB, GetPointIndex(m_mstEdges[i].PointA), GetPointIndex(m_mstEdges[i].PointB));
                m_increaseWeight.Add(cacheSegment);
            }

            m_increaseWeight.Sort((MSTSegment x, MSTSegment y) =>
            {
                if (x.Weight > y.Weight)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });
        }

        private int GetPointIndex(Vector3 point)
        {
            int index = -1;
            for (int i = 0; i < InputPointList.Count; i++)
            {
                if (point == InputPointList[i])
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private void StartAlgorithm()
        {
            MSTSegments.Clear();

            while (m_increaseWeight.Count > 0)
            {
                int index = 0;
                MSTSegment tempSegment = m_increaseWeight[index];
                int rootA = FindRoot(tempSegment.PointAIndex);
                int rootB = FindRoot(tempSegment.PointBIndex);

                if (rootA != rootB)
                {
                    UnionSet(tempSegment.PointAIndex, tempSegment.PointBIndex);
                    MSTSegments.Add(tempSegment);
                }

                m_increaseWeight.RemoveAt(index);
            }
        }

        private void UnionSet(int indexA, int indexB)
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

        private int FindRoot(int index)
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
