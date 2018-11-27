using UnityEngine;

namespace Voronoi
{
    public class VEvent
    {
        public Vector3 Site;
        public bool IsSiteEvent;
        public VParabola Parabola;
        public Vector3 VertexPoint;

        public VEvent(Vector3 site, bool isSiteEvent)
        {
            Site = site;
            IsSiteEvent = isSiteEvent;
        }
    }
}