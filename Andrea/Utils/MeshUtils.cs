using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using SpatialSlur;
using SpatialSlur.Collections;
using SpatialSlur.Meshes;
using SpatialSlur.Fields;
using SpatialSlur.Rhino;

using Vec2d = SpatialSlur.Vector2d;
using Vec3d = SpatialSlur.Vector3d;
using Vec4d = SpatialSlur.Vector4d;

using Rhino;
using Rhino.Geometry;

namespace Andrea.Utils
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MeshUtils
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vPos"></param>
        /// <param name="connectedVerts"></param>
        /// <param name="thickness_"></param>
        /// <returns></returns>

        public static Polyline OffsetNode(Point3d vPos, List<Point3d> connectedVerts, double thickness_)
        {
            List<Curve[]> offsets = new List<Curve[]>();
            List<Curve> crvs = new List<Curve>();
            Polyline out_c = new Polyline();


            //set curves per graph connectivity
            int connectedCount = connectedVerts.Count;

            if (connectedCount < 2) return out_c;

            for (int i = 0; i < connectedCount; i++)
            {
                List<Point3d> vp = new List<Point3d>();

                Rhino.Geometry.Vector3d dir = new Rhino.Geometry.Vector3d();

                dir = connectedVerts[i] - vPos;
                dir.Unitize();
                dir *= thickness_;
                dir += (Rhino.Geometry.Vector3d)vPos;

                vp.Add((Point3d)dir);

                vp.Add(vPos);

                dir = connectedVerts[(i + 1) % connectedCount] - vPos;
                dir.Unitize();
                dir *= thickness_;
                dir += (Rhino.Geometry.Vector3d)vPos;


                vp.Add((Point3d)dir);

                Polyline crv = new Polyline(vp);
                crvs.Add(crv.ToPolylineCurve());
            }

            //create offsets based on curves
            if (connectedCount > 2)
            {
                foreach (var c in crvs)
                {
                    Point3d s = c.PointAtStart;
                    Point3d e = c.PointAtEnd;

                    Point3d dir = (s + e) / 2;

                    offsets.Add(c.Offset(dir, Rhino.Geometry.Vector3d.ZAxis, thickness_ * 0.5, 0.001, CurveOffsetCornerStyle.Sharp));
                }
            }
            else
            {
                foreach (var c in crvs)
                {
                    c.Reverse();
                    offsets.Add(c.Offset(Plane.WorldXY, thickness_ * 0.5, 0.001, CurveOffsetCornerStyle.Sharp));
                }
            }

            //create end connections for close polyline
            crvs = new List<Curve>();

            foreach (var o in offsets)
            {
                foreach (var c in o)
                {
                    crvs.Add(c);
                }
            }

            List<Curve> joinCurves = new List<Curve>();

            for (int i = 0; i < crvs.Count; i++)
            {
                List<Point3d> vp = new List<Point3d>();
                vp.Add(crvs[i].PointAtEnd);
                vp.Add(crvs[(i + 1) % crvs.Count].PointAtStart);
                PolylineCurve c = new PolylineCurve(vp);

                joinCurves.Add(offsets[i][0]);
                joinCurves.Add(c);
            }

            //Join curves
            Curve.JoinCurves(joinCurves)[0].TryGetPolyline(out out_c);

            return out_c;
        }


        /// <summary>
        /// //
        /// </summary>
        /// <param name="PointA"></param>
        /// <param name="PointB"></param>
        /// <param name="_thickness"></param>
        /// <returns></returns>
        public static Polyline OffsetEdge(Point3d PointA, Point3d PointB, double _thickness)
        {
            List<Point3d> plPoints = new List<Point3d>();

            Rhino.Geometry.Vector3d dir, off1, off2;

            dir = PointB - PointA;

            off1 = Rhino.Geometry.Vector3d.CrossProduct(dir, Rhino.Geometry.Vector3d.ZAxis);
            off1.Unitize();
            off1 *= _thickness * 0.5;

            off2 = off1 * -1;

            //add empty points/corners
            for (int i = 0; i < 5; i++)
            {
                Point3d p = new Point3d();
                plPoints.Add(p);
            }

            //move start and end points to match node (offset)
            dir.Unitize();
            dir *= _thickness;

            PointA += dir;
            PointB += -dir;

            plPoints[0] = PointA + off1;
            plPoints[1] = PointB + off1;
            plPoints[2] = PointB + off2;
            plPoints[3] = PointA + off2;
            plPoints[4] = PointA + off1;

            Polyline out_pl = new Polyline(plPoints);
            return out_pl;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="polylineA"></param>
        /// <param name="polylineB"></param>
        /// <returns></returns>

        public static Mesh CreateLoftClosed(Polyline polylineA, Polyline polylineB)
        {
            Mesh result = new Mesh();
            var verts = result.Vertices;
            var faces = result.Faces;

            int n = polylineA.Count - 1;

            // add verts 
            for (int i = 0; i < n; i++)
                verts.Add(polylineA[i]);

            for (int i = 0; i < n; i++)
                verts.Add(polylineB[i]);

            // add faces
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                faces.AddFace(i, j, j + n, i + n);
            }

            // add cap faces
            Mesh capA = Mesh.CreateFromClosedPolyline(polylineA);
            Mesh capB = Mesh.CreateFromClosedPolyline(polylineB);

            result.Append(capA);
            result.Append(capB);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="heMesh"></param>
        /// <returns></returns>

        static PolylineCurve GetBoundaryPolyline(HeMesh3d heMesh)
        {
            PolylineCurve poly_out;

            List<Point3d> pts = new List<Point3d>();
            HeMesh3d.Halfedge start_he = new HeMesh3d.Halfedge();

            //get Halfedge on boundary
            if (heMesh.Vertices[0].First.Face == null) start_he = heMesh.Vertices[0].First;
            else start_he = heMesh.Vertices[0].First.Twin;

            pts.Add((Point3d)heMesh.Vertices[0].Position);

            bool exit = false;
            HeMesh3d.Halfedge temp = new HeMesh3d.Halfedge();
            temp = start_he;
            do
            {
                pts.Add((Point3d)temp.End.Position);
                temp = temp.Next;
                if (temp == start_he) exit = true;
            }
            while (!exit);

            poly_out = new PolylineCurve(pts);

            return poly_out;
        }


    }
}
