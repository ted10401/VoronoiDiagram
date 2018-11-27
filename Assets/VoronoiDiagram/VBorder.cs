
namespace Voronoi
{
    public class VBorder
    {
        public float HalfWidth;
        public float HalfHeight;

        public VBorder(float width, float height)
        {
            HalfWidth = width / 2;
            HalfHeight = height / 2;
        }
    }
}