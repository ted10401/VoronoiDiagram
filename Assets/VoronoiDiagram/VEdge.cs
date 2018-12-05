using UnityEngine;

namespace Voronoi
{
    public class VEdge
    {
        private enum Direction
        {
            Left,
            Right,
        }

        public Vector3 StartPoint;
        public Vector3 EndPoint;
        public Vector3 VertexPoint;

        public Vector3 LeftSite;
        public Vector3 RightSite;

        private float m_lerp;
        private float m_intercept;
        private Direction m_direction;
        private bool m_hasVertex;

        public VEdge(Vector3 startPoint, Vector3 leftSite, Vector3 rightSite)
        {
            StartPoint = startPoint;
            EndPoint = startPoint;

            LeftSite = leftSite;
            RightSite = rightSite;
            m_lerp = GetLerp();
            m_intercept = GetIntercept();
            m_direction = GetDirection();
        }

        public void UpdateDirectrix(float directrix)
        {
            Vector3[] intersectPoints = VParabolaUtils.GetIntersectPoints(LeftSite, RightSite, directrix);

            if (intersectPoints.Length == 1)
            {
                EndPoint = intersectPoints[0];
            }
            else
            {
                if (m_direction == Direction.Left)
                {
                    if (intersectPoints[0].x < intersectPoints[1].x)
                    {
                        EndPoint = intersectPoints[0];
                    }
                    else
                    {
                        EndPoint = intersectPoints[1];
                    }
                }
                else
                {
                    if (intersectPoints[0].x < intersectPoints[1].x)
                    {
                        EndPoint = intersectPoints[1];
                    }
                    else
                    {
                        EndPoint = intersectPoints[0];
                    }
                }
            }

            if (m_hasVertex)
            {
                if (m_direction == Direction.Left)
                {
                    if (EndPoint.x < VertexPoint.x)
                    {
                        EndPoint = VertexPoint;
                    }
                }
                else
                {
                    if (EndPoint.x > VertexPoint.x)
                    {
                        EndPoint = VertexPoint;
                    }
                }

                if (IsInfinityLerp())
                {
                    if (EndPoint.y < VertexPoint.y)
                    {
                        EndPoint = VertexPoint;
                    }
                }
            }
        }

        public void SetVertexPoint(Vector3 vertexPoint)
        {
            if (m_hasVertex)
            {
                return;
            }

            m_hasVertex = true;
            VertexPoint = vertexPoint;
        }

        public void Finish(VBorder border)
        {
            if (!m_hasVertex)
            {
                float x = 0;

                if (m_direction == Direction.Left)
                {
                    x = -border.HalfWidth;
                }
                else
                {
                    x = border.HalfWidth;
                }

                if (!IsInfinityLerp())
                {
                    float y = m_lerp * x + m_intercept;

                    if (y > border.HalfHeight || y < -border.HalfHeight)
                    {
                        y = Mathf.Clamp(y, -border.HalfHeight, border.HalfHeight);
                        x = (y - m_intercept) / m_lerp;
                    }

                    SetVertexPoint(new Vector3(x, y, 0));
                }
                else
                {
                    SetVertexPoint(new Vector3(EndPoint.x, -border.HalfHeight, 0));
                }
            }

            if (StartPoint.x < -border.HalfWidth ||
                StartPoint.x > border.HalfWidth ||
                StartPoint.y < -border.HalfHeight ||
                StartPoint.y > border.HalfHeight)
            {
                if (StartPoint.x < -border.HalfWidth || StartPoint.x > border.HalfWidth)
                {
                    StartPoint.x = Mathf.Clamp(StartPoint.x, -border.HalfWidth, border.HalfWidth);

                    if (!IsInfinityLerp())
                    {
                        StartPoint.y = m_lerp * StartPoint.x + m_intercept;

                        if (StartPoint.y < -border.HalfHeight || StartPoint.y > border.HalfHeight)
                        {
                            StartPoint.y = Mathf.Clamp(StartPoint.y, -border.HalfHeight, border.HalfHeight);
                            StartPoint.x = (StartPoint.y - m_intercept) / m_lerp;
                        }
                    }
                }
                else
                {
                    StartPoint.y = Mathf.Clamp(StartPoint.y, -border.HalfHeight, border.HalfHeight);
                    StartPoint.x = (StartPoint.y - m_intercept) / m_lerp;

                    if (StartPoint.x < -border.HalfWidth || StartPoint.x > border.HalfWidth)
                    {
                        StartPoint.x = Mathf.Clamp(StartPoint.x, -border.HalfWidth, border.HalfWidth);
                        StartPoint.y = m_lerp * StartPoint.x + m_intercept;
                    }
                }
            }

            if (VertexPoint.x < -border.HalfWidth ||
                VertexPoint.x > border.HalfWidth ||
                VertexPoint.y < -border.HalfHeight ||
                VertexPoint.y > border.HalfHeight)
            {
                if (VertexPoint.x < -border.HalfWidth || VertexPoint.x > border.HalfWidth)
                {
                    VertexPoint.x = Mathf.Clamp(VertexPoint.x, -border.HalfWidth, border.HalfWidth);

                    if (!IsInfinityLerp())
                    {
                        VertexPoint.y = m_lerp * VertexPoint.x + m_intercept;

                        if (VertexPoint.y < -border.HalfHeight || VertexPoint.y > border.HalfHeight)
                        {
                            VertexPoint.y = Mathf.Clamp(VertexPoint.y, -border.HalfHeight, border.HalfHeight);
                            VertexPoint.x = (VertexPoint.y - m_intercept) / m_lerp;
                        }
                    }
                }
                else
                {
                    VertexPoint.y = Mathf.Clamp(VertexPoint.y, -border.HalfHeight, border.HalfHeight);
                    VertexPoint.x = (VertexPoint.y - m_intercept) / m_lerp;

                    if (VertexPoint.x < -border.HalfWidth || VertexPoint.x > border.HalfWidth)
                    {
                        VertexPoint.x = Mathf.Clamp(VertexPoint.x, -border.HalfWidth, border.HalfWidth);
                        VertexPoint.y = m_lerp * VertexPoint.x + m_intercept;
                    }
                }
            }
        }

        public bool IsInfinityLerp()
        {
            return RightSite.y == LeftSite.y;
        }

        private float GetLerp()
        {
            return (LeftSite.x - RightSite.x) / (RightSite.y - LeftSite.y);
        }

        private float GetIntercept()
        {
            if (LeftSite.y == RightSite.y)
            {
                return (LeftSite.x + RightSite.x) / 2;
            }

            return StartPoint.y - m_lerp * StartPoint.x;
        }

        private Direction GetDirection()
        {
            if (m_lerp > 0)
            {
                return LeftSite.x < RightSite.x ? Direction.Left : Direction.Right;
            }
            else if (m_lerp < 0)
            {
                return LeftSite.x > RightSite.x ? Direction.Left : Direction.Right;
            }
            else
            {
                return LeftSite.y > RightSite.y ? Direction.Left : Direction.Right;
            }
        }

        public static bool HasValidIntersectPoint(VEdge leftEdge, VEdge rightEdge)
        {
            if(leftEdge == null || rightEdge == null)
            {
                return false;
            }

            if (leftEdge.m_lerp == rightEdge.m_lerp)
            {
                return false;
            }

            Vector3 intersectPoint = GetIntersectPoint(leftEdge, rightEdge);

            bool valid = true;
            if(!leftEdge.IsInfinityLerp())
            {
                if (leftEdge.m_direction == Direction.Left)
                {
                    valid &= intersectPoint.x < leftEdge.StartPoint.x;
                }
                else
                {
                    valid &= intersectPoint.x > leftEdge.StartPoint.x;
                }
            }

            if(!rightEdge.IsInfinityLerp())
            {
                if (rightEdge.m_direction == Direction.Left)
                {
                    valid &= intersectPoint.x < rightEdge.StartPoint.x;
                }
                else
                {
                    valid &= intersectPoint.x > rightEdge.StartPoint.x;
                }
            }

            return valid;
        }

        public static Vector3 GetIntersectPoint(VEdge leftEdge, VEdge rightEdge)
        {
            Vector3 intersectPoint = Vector3.zero;

            if (leftEdge.IsInfinityLerp())
            {
                intersectPoint.x = leftEdge.StartPoint.x;
                intersectPoint.y = rightEdge.m_lerp * intersectPoint.x + rightEdge.m_intercept;
            }
            else if (rightEdge.IsInfinityLerp())
            {
                intersectPoint.x = rightEdge.StartPoint.x;
                intersectPoint.y = leftEdge.m_lerp * intersectPoint.x + leftEdge.m_intercept;
            }
            else
            {
                intersectPoint.x = (rightEdge.m_intercept - leftEdge.m_intercept) / (leftEdge.m_lerp - rightEdge.m_lerp);
                intersectPoint.y = leftEdge.m_lerp * intersectPoint.x + leftEdge.m_intercept;
            }

            return intersectPoint;
        }
    }
}