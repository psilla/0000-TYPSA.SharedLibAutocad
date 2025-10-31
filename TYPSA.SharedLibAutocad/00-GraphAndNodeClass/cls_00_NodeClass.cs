using Autodesk.AutoCAD.Geometry;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_NodeClass
    {
        public class NodePoint
        {
            public Point3d Point { get; }

            public NodePoint(Point3d pt)
            {
                Point = pt;
            }

            public override bool Equals(object obj)
            {
                if (obj is NodePoint other)
                    return Point.IsEqualTo(other.Point, new Tolerance(0.01, 0.01));
                return false;
            }
        }


    }
}
