using UnityEngine;
using System.Collections.Generic;

namespace Voronoi
{
    public class VoronoiDiagram
    {
        private VBorder m_border;
        private List<VParabola> m_parabolas;
        private List<VEvent> m_eventQueues;
        private float m_ly;
        public List<Vector3> Vertices;
        public List<VEdge> Edges;

        public VoronoiDiagram(Vector3[] inputPoints, VBorder border)
        {
            m_border = border;
            m_parabolas = new List<VParabola>();
            Edges = new List<VEdge>();
            Vertices = new List<Vector3>();

            m_eventQueues = new List<VEvent>();
            for (int i = 0; i < inputPoints.Length; i++)
            {
                m_eventQueues.Add(new VEvent(inputPoints[i], true));
            }
            SortEventQueues();

            while (m_eventQueues.Count > 0)
            {
                VEvent currentEvent = m_eventQueues[0];
                m_eventQueues.RemoveAt(0);

                m_ly = currentEvent.Site.y;

                if (currentEvent.IsSiteEvent)
                {
                    AddSiteEvent(currentEvent.Site, m_ly);
                }
                else
                {
                    AddCircleEvent(currentEvent);
                }
            }

            FinishEdge();
        }

        public VoronoiDiagram(Vector3[] inputPoints, VBorder border, float ly)
        {
            m_border = border;
            m_parabolas = new List<VParabola>();
            Edges = new List<VEdge>();
            Vertices = new List<Vector3>();

            m_eventQueues = new List<VEvent>();
            for (int i = 0; i < inputPoints.Length; i++)
            {
                if(inputPoints[i].y < ly)
                {
                    continue;
                }

                m_eventQueues.Add(new VEvent(inputPoints[i], true));
            }
            SortEventQueues();

            while (m_eventQueues.Count > 0)
            {
                if (m_eventQueues[0].Site.y < ly)
                {
                    break;
                }

                m_ly = m_eventQueues[0].Site.y;

                VEvent currentEvent = m_eventQueues[0];
                m_eventQueues.RemoveAt(0);

                if (currentEvent.IsSiteEvent)
                {
                    AddSiteEvent(currentEvent.Site, m_ly);
                }
                else
                {
                    AddCircleEvent(currentEvent);
                }
            }

            FinishEdge();
        }

        private void SortEventQueues()
        {
            m_eventQueues.Sort((VEvent x, VEvent y) =>
            {
                int yDiff = y.Site.y.CompareTo(x.Site.y);
                if (yDiff != 0)
                {
                    return yDiff;
                }
                else
                {
                    return x.Site.x.CompareTo(y.Site.x);
                }
            });
        }

        public void AddSiteEvent(Vector3 site, float ly)
        {
            if (m_parabolas.Count == 0)
            {
                m_parabolas.Add(new VParabola(site));
                return;
            }

            int beachLineIndex = GetBeachLineByX(site.x, ly);
            VParabola beachLineParabola = m_parabolas[beachLineIndex];
            VParabola p0 = null;
            VParabola p1 = null;
            VParabola p2 = null;

            if (beachLineParabola.FocusPoint.y == site.y)
            {
                Vector3 startPoint = new Vector3((beachLineParabola.FocusPoint.x + site.x) / 2, m_border.HalfHeight, 0);
                VEdge edge = null;
                if (beachLineParabola.FocusPoint.x < site.x)
                {
                    edge = new VEdge(startPoint, beachLineParabola.FocusPoint, site);

                    p0 = new VParabola(beachLineParabola.FocusPoint);
                    p1 = new VParabola(site);
                }
                else
                {
                    edge = new VEdge(startPoint, site, beachLineParabola.FocusPoint);

                    p0 = new VParabola(site);
                    p1 = new VParabola(beachLineParabola.FocusPoint);
                }

                Edges.Add(edge);

                p0.LeftParabola = beachLineParabola.LeftParabola;
                p0.LeftEdge = beachLineParabola.LeftEdge;
                p0.RightEdge = edge;
                p0.RightParabola = p1;

                p1.LeftParabola = p0;
                p1.LeftEdge = edge;
                p1.RightEdge = beachLineParabola.RightEdge;
                p1.RightParabola = beachLineParabola.RightParabola;

                if (p0.LeftParabola != null)
                {
                    p0.LeftParabola.RightParabola = p0;
                }

                if (p1.RightParabola != null)
                {
                    p1.RightParabola.LeftParabola = p1;
                }

                m_parabolas.RemoveAt(beachLineIndex);
                m_parabolas.Insert(beachLineIndex, p0);
                m_parabolas.Insert(beachLineIndex + 1, p1);

                CheckCircleEvent(p0);
                CheckCircleEvent(p1);

                return;
            }

            if (beachLineParabola.CircleEvent != null)
            {
                m_eventQueues.Remove(beachLineParabola.CircleEvent);
                beachLineParabola.CircleEvent = null;
            }

            Vector3 edgeStartPoint = new Vector3(site.x, VParabolaUtils.GetParabolaValueY(beachLineParabola.FocusPoint, ly, site.x), 0);
            VEdge leftEdge = new VEdge(edgeStartPoint, beachLineParabola.FocusPoint, site);
            Edges.Add(leftEdge);
            VEdge rightEdge = new VEdge(edgeStartPoint, site, beachLineParabola.FocusPoint);
            Edges.Add(rightEdge);

            p0 = new VParabola(beachLineParabola.FocusPoint);
            p1 = new VParabola(site);
            p2 = new VParabola(beachLineParabola.FocusPoint);

            p0.LeftParabola = beachLineParabola.LeftParabola;
            p0.LeftEdge = beachLineParabola.LeftEdge;
            p0.RightEdge = leftEdge;
            p0.RightParabola = p1;

            p1.LeftParabola = p0;
            p1.LeftEdge = leftEdge;
            p1.RightEdge = rightEdge;
            p1.RightParabola = p2;

            p2.LeftParabola = p1;
            p2.LeftEdge = rightEdge;
            p2.RightEdge = beachLineParabola.RightEdge;
            p2.RightParabola = beachLineParabola.RightParabola;

            if (p0.LeftParabola != null)
            {
                p0.LeftParabola.RightParabola = p0;
            }

            if (p2.RightParabola != null)
            {
                p2.RightParabola.LeftParabola = p2;
            }

            m_parabolas.RemoveAt(beachLineIndex);
            m_parabolas.Insert(beachLineIndex, p0);
            m_parabolas.Insert(beachLineIndex + 1, p1);
            m_parabolas.Insert(beachLineIndex + 2, p2);

            CheckCircleEvent(p0);
            CheckCircleEvent(p2);
        }

        private int GetBeachLineByX(float x, float directrix)
        {
            List<Vector3> beachLineIntersectPoints = new List<Vector3>();
            for (int i = 0; i < m_parabolas.Count; i++)
            {
                if (m_parabolas[i].RightEdge != null)
                {
                    m_parabolas[i].RightEdge.UpdateDirectrix(directrix);
                    beachLineIntersectPoints.Add(m_parabolas[i].RightEdge.EndPoint);
                }
            }

            beachLineIntersectPoints.Sort((Vector3 point1, Vector3 point2) =>
            {
                return point1.x.CompareTo(point2.x);
            });

            int beachLineIndex = 0;
            for (int i = 0; i < beachLineIntersectPoints.Count; i++)
            {
                if (x >= beachLineIntersectPoints[i].x)
                {
                    beachLineIndex = i + 1;
                }
            }

            return beachLineIndex;
        }

        public Vector3 GetBeachLine(float x, float ly)
        {
            if(m_parabolas == null || m_parabolas.Count == 0)
            {
                return new Vector3(x, m_border.HalfHeight, 0);
            }

            float minValue = 0f;
            float cacheMinValue = 0f;
            for (int i = 0; i < m_parabolas.Count; i++)
            {
                cacheMinValue = VParabolaUtils.GetParabolaValueY(m_parabolas[i].FocusPoint, ly, x);

                if (i == 0)
                {
                    minValue = cacheMinValue;
                    continue;
                }

                if (cacheMinValue < minValue)
                {
                    minValue = cacheMinValue;
                }
            }

            return new Vector3(x, minValue, 0);
        }

        private void CheckCircleEvent(VParabola parabola)
        {
            VEdge leftEdge = parabola.LeftEdge;
            VEdge rightEdge = parabola.RightEdge;

            if(!VEdge.HasValidIntersectPoint(leftEdge, rightEdge))
            {
                return;
            }

            Vector3 intersectPoint = VEdge.GetIntersectPoint(leftEdge, rightEdge);

            float distance = Vector3.Distance(intersectPoint, parabola.FocusPoint);
            float targetLy = intersectPoint.y - distance;

            Vector3 circlePoint = new Vector3(parabola.FocusPoint.x, targetLy, 0);

            VEvent algorithmEvent = new VEvent(circlePoint, false);
            algorithmEvent.Parabola = parabola;
            algorithmEvent.VertexPoint = intersectPoint;

            parabola.CircleEvent = algorithmEvent;

            m_eventQueues.Add(algorithmEvent);
            SortEventQueues();
        }

        private void AddCircleEvent(VEvent algorithmEvent)
        {
            List<VParabola> removeParabolas = new List<VParabola>();
            VParabola removeParabola = algorithmEvent.Parabola;

            VParabola leftParabola = removeParabola.LeftParabola;
            VParabola rightParabola = removeParabola.RightParabola;

            if(leftParabola == null || rightParabola == null)
            {
                return;
            }

            removeParabolas.Add(removeParabola);

            if(removeParabolas.Count == 0)
            {
                return;
            }

            Vertices.Add(algorithmEvent.VertexPoint);

            VEdge edge = new VEdge(algorithmEvent.VertexPoint, leftParabola.FocusPoint, rightParabola.FocusPoint);
            Edges.Add(edge);

            leftParabola.RightParabola = rightParabola;
            leftParabola.RightEdge = edge;

            rightParabola.LeftParabola = leftParabola;
            rightParabola.LeftEdge = edge;

            for (int i = 0; i < removeParabolas.Count; i++)
            {
                m_eventQueues.Remove(removeParabolas[i].CircleEvent);
                removeParabolas[i].CircleEvent = null;
                removeParabolas[i].LeftEdge.SetVertexPoint(algorithmEvent.VertexPoint);
                removeParabolas[i].RightEdge.SetVertexPoint(algorithmEvent.VertexPoint);
                m_parabolas.Remove(removeParabolas[i]);
            }

            CheckCircleEvent(leftParabola);
            CheckCircleEvent(rightParabola);
        }

        private void FinishEdge()
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                Edges[i].Finish(m_border);
            }

            List<Vector3> removeVertices = new List<Vector3>();
            for (int i = 0; i < Vertices.Count; i++)
            {
                if (Vertices[i].x > m_border.HalfWidth || Vertices[i].x < -m_border.HalfWidth ||
                    Vertices[i].y > m_border.HalfHeight || Vertices[i].y < -m_border.HalfHeight)
                {
                    removeVertices.Add(Vertices[i]);
                }
            }

            for (int i = 0; i < removeVertices.Count; i++)
            {
                Vertices.Remove(removeVertices[i]);
            }
        }
    }
}