using UnityEngine;

namespace Voronoi
{
    public class VParabola
    {
        public Vector3 FocusPoint;
        public VEdge LeftEdge;
        public VEdge RightEdge;

        public VParabola LeftParabola;
        public VParabola RightParabola;

        public VEvent CircleEvent;

        public VParabola(Vector3 focusPoint)
        {
            FocusPoint = focusPoint;
        }
    }
}