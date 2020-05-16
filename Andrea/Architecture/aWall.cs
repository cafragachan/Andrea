using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino;
using Rhino.Geometry;

using Andrea.Base;

namespace Andrea.Architecture
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class aWall
    {
        public Mesh wallMesh;

        public void Create(Line line, double height, double thickness, aEnumStyle style)
        {
            Vector3d x = new Vector3d();
            x = line.To - line.From;

            Vector3d y = new Vector3d();
            y = Vector3d.CrossProduct(Vector3d.ZAxis, x);

            Plane bPlane = new Plane(line.From, x, y);
           
            //construct corners
            y.Unitize();
            Point3d c1, c2, c3;
            c1 = line.From + (y * thickness * 0.5);
            c2 = line.To + (y * thickness * -0.5);
            c3 = line.From + Vector3d.ZAxis * height;

            List<Point3d> corners = new List<Point3d>();
            corners.Add(c1);
            corners.Add(c2);
            corners.Add(c3);

            Box box = new Box(bPlane, corners);

            wallMesh = Mesh.CreateFromBox(box, 1, 1, 1);
        }
    }
}
