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
                    if (EndPoint.z < VertexPoint.z)
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
                    float z = m_lerp * x + m_intercept;

                    if (z > border.HalfHeight || z < -border.HalfHeight)
                    {
                        z = Mathf.Clamp(z, -border.HalfHeight, border.HalfHeight);
                        x = (z - m_intercept) / m_lerp;
                    }

                    SetVertexPoint(new Vector3(x, 0, z));
                }
                else
                {
                    SetVertexPoint(new Vector3(EndPoint.x, 0, -border.HalfHeight));
                }
            }

            if (StartPoint.x < -border.HalfWidth ||
                StartPoint.x > border.HalfWidth ||
                StartPoint.z < -border.HalfHeight ||
                StartPoint.z > border.HalfHeight)
            {
                if (StartPoint.x < -border.HalfWidth || StartPoint.x > border.HalfWidth)
                {
                    StartPoint.x = Mathf.Clamp(StartPoint.x, -border.HalfWidth, border.HalfWidth);

                    if (!IsInfinityLerp())
                    {
                        StartPoint.z = m_lerp * StartPoint.x + m_intercept;

                        if (StartPoint.z < -border.HalfHeight || StartPoint.z > border.HalfHeight)
                        {
                            StartPoint.z = Mathf.Clamp(StartPoint.z, -border.HalfHeight, border.HalfHeight);
                            StartPoint.x = (StartPoint.z - m_intercept) / m_lerp;
                        }
                    }
                }
                else
                {
                    StartPoint.z = Mathf.Clamp(StartPoint.z, -border.HalfHeight, border.HalfHeight);
                    StartPoint.x = (StartPoint.z - m_intercept) / m_lerp;

                    if (StartPoint.x < -border.HalfWidth || StartPoint.x > border.HalfWidth)
                    {
                        StartPoint.x = Mathf.Clamp(StartPoint.x, -border.HalfWidth, border.HalfWidth);
                        StartPoint.z = m_lerp * StartPoint.x + m_intercept;
                    }
                }
            }

            if (VertexPoint.x < -border.HalfWidth ||
                VertexPoint.x > border.HalfWidth ||
                VertexPoint.z < -border.HalfHeight ||
                VertexPoint.z > border.HalfHeight)
            {
                if (VertexPoint.x < -border.HalfWidth || VertexPoint.x > border.HalfWidth)
                {
                    VertexPoint.x = Mathf.Clamp(VertexPoint.x, -border.HalfWidth, border.HalfWidth);

                    if (!IsInfinityLerp())
                    {
                        VertexPoint.z = m_lerp * VertexPoint.x + m_intercept;

                        if (VertexPoint.z < -border.HalfHeight || VertexPoint.z > border.HalfHeight)
                        {
                            VertexPoint.z = Mathf.Clamp(VertexPoint.z, -border.HalfHeight, border.HalfHeight);
                            VertexPoint.x = (VertexPoint.z - m_intercept) / m_lerp;
                        }
                    }
                }
                else
                {
                    VertexPoint.z = Mathf.Clamp(VertexPoint.z, -border.HalfHeight, border.HalfHeight);
                    VertexPoint.x = (VertexPoint.z - m_intercept) / m_lerp;

                    if (VertexPoint.x < -border.HalfWidth || VertexPoint.x > border.HalfWidth)
                    {
                        VertexPoint.x = Mathf.Clamp(VertexPoint.x, -border.HalfWidth, border.HalfWidth);
                        VertexPoint.z = m_lerp * VertexPoint.x + m_intercept;
                    }
                }
            }
        }

        public bool IsInfinityLerp()
        {
            return RightSite.z == LeftSite.z;
        }

        private float GetLerp()
        {
            return (LeftSite.x - RightSite.x) / (RightSite.z - LeftSite.z);
        }

        private float GetIntercept()
        {
            if (LeftSite.z == RightSite.z)
            {
                return (LeftSite.x + RightSite.x) / 2;
            }

            return StartPoint.z - m_lerp * StartPoint.x;
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
                return LeftSite.z > RightSite.z ? Direction.Left : Direction.Right;
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
                intersectPoint.z = rightEdge.m_lerp * intersectPoint.x + rightEdge.m_intercept;
            }
            else if (rightEdge.IsInfinityLerp())
            {
                intersectPoint.x = rightEdge.StartPoint.x;
                intersectPoint.z = leftEdge.m_lerp * intersectPoint.x + leftEdge.m_intercept;
            }
            else
            {
                intersectPoint.x = (rightEdge.m_intercept - leftEdge.m_intercept) / (leftEdge.m_lerp - rightEdge.m_lerp);
                intersectPoint.z = leftEdge.m_lerp * intersectPoint.x + leftEdge.m_intercept;
            }

            return intersectPoint;
        }
    }
}