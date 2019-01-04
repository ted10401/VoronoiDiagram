
namespace SpanningTree
{
    public class MinimumSpanningTree : BaseSpanningTree
    { 
        public MinimumSpanningTree(STEdge[] stEdges) : base(stEdges)
        {
        }

        protected override void AfterUpdateSegments()
        {
            m_segments.Sort((STSegment x, STSegment y) =>
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

        protected override void Algorithm()
        {
            int cacheIndex;
            STSegment cacheSegment;
            int cahceRootA;
            int cacheRootB;

            while (m_segments.Count > 0)
            {
                cacheIndex = 0;
                cacheSegment = m_segments[cacheIndex];
                cahceRootA = FindRoot(cacheSegment.PointAIndex);
                cacheRootB = FindRoot(cacheSegment.PointBIndex);

                if (cahceRootA != cacheRootB)
                {
                    UnionSet(cacheSegment.PointAIndex, cacheSegment.PointBIndex);
                    Segments.Add(cacheSegment);
                }

                m_segments.RemoveAt(cacheIndex);
            }
        }
    }
}
