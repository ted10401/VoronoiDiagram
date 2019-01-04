using UnityEngine;

namespace Voronoi
{
    public class VParabolaUtils
    {
        public static Vector3[] GetIntersectPoints(Vector3 focusPoint1, Vector3 focusPoint2, float directrix)
        {
            if (IsInfinityLerp(focusPoint1, focusPoint2))
            {
                float x = (focusPoint1.x + focusPoint2.x) / 2;
                return new Vector3[]
                {
                    new Vector3(x, 0, GetParabolaValueZ(focusPoint1, directrix, x))
                };
            }

            float lerp = GetBisectorLerp(focusPoint1, focusPoint2);
            float intercept = GetBisectorIntercept(focusPoint1, focusPoint2);

            Vector3 vertexPoint = GetVertexPoint(focusPoint1, directrix);
            float focalLength = GetFocalLength(focusPoint1, directrix);

            float a = 1;
            float b = -2 * vertexPoint.x - 2 * lerp * focalLength;
            float c = vertexPoint.x * vertexPoint.x - 2 * focalLength * intercept + 2 * focalLength * vertexPoint.z;

            float x1 = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
            float x2 = (-b - Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

            return new Vector3[]
            {
                new Vector3(x1, 0, GetParabolaValueZ(focusPoint1, directrix, x1)),
                new Vector3(x2, 0, GetParabolaValueZ(focusPoint1, directrix, x2)),
            };
        }

        private static bool IsInfinityLerp(Vector3 focusPoint1, Vector3 focusPoint2)
        {
            return focusPoint1.z == focusPoint2.z;
        }

        public static float GetParabolaValueZ(Vector3 focusPoint, float directrix, float x)
        {
            if (focusPoint.z == directrix)
            {
                if (focusPoint.x == x)
                {
                    return focusPoint.z;
                }
            }

            Vector3 vertexPoint = GetVertexPoint(focusPoint, directrix);

            float z = (x - vertexPoint.x) * (x - vertexPoint.x) / (2 * GetFocalLength(focusPoint, directrix)) + vertexPoint.z;
            return z;
        }

        private static Vector3 GetVertexPoint(Vector3 focusPoint, float directrix)
        {
            Vector3 vertexPoint = focusPoint;
            vertexPoint.z = directrix + GetFocalLength(focusPoint, directrix) / 2;

            return vertexPoint;
        }

        private static float GetFocalLength(Vector3 focusPoint, float directrix)
        {
            return focusPoint.z - directrix;
        }

        public static float GetBisectorLerp(Vector3 focusPoint1, Vector3 focusPoint2)
        {
            return (focusPoint1.x - focusPoint2.x) / (focusPoint2.z - focusPoint1.z);
        }

        public static float GetBisectorIntercept(Vector3 focusPoint1, Vector3 focusPoint2)
        {
            return (focusPoint2.x * focusPoint2.x - focusPoint1.x * focusPoint1.x + focusPoint2.z * focusPoint2.z - focusPoint1.z * focusPoint1.z) / (2 * (focusPoint2.z - focusPoint1.z));
        }
    }
}