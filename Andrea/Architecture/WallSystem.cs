using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino;
using Rhino.Geometry;

using SpatialSlur;
using SpatialSlur.Collections;
using SpatialSlur.Meshes;
using SpatialSlur.Fields;
using SpatialSlur.Rhino;

using Vec2d = SpatialSlur.Vector2d;
using Vec3d = SpatialSlur.Vector3d;
using Vec4d = SpatialSlur.Vector4d;

using Andrea.Base;
using Andrea.Utils;

namespace Andrea.Architecture
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class WallSystem
    {
        private HeMesh3d spaceGraph;
        public List<Wall> walls;

        public WallSystem()
        {
            spaceGraph = new HeMesh3d();
            walls = new List<Wall>();
        }

        public WallSystem(HeMesh3d _inputHeMesh)
        {
            spaceGraph = _inputHeMesh;
            walls = new List<Wall>();
        }

        public void Create(double height, double thickness)
        {
            List<Polyline> wallCrvs = new List<Polyline>();

            if (spaceGraph.Vertices.Count == 0) return;

            for (int i = 0; i < spaceGraph.Vertices.Count; i++)
            {
                List<Point3d> connectedVs = new List<Point3d>();

                foreach (var v in spaceGraph.Vertices[i].ConnectedVertices)
                {
                    connectedVs.Add((Point3d)v.Position);
                }

                if (connectedVs.Count == 0) continue;

                Point3d vPos = (Point3d)spaceGraph.Vertices[i].Position;

                Polyline out_c = MeshUtils.OffsetNode(vPos, connectedVs, thickness);

                wallCrvs.Add(out_c);
            }

            for (int i = 0; i < spaceGraph.Edges.Count; i++)
            {
                Point3d a = (Point3d)spaceGraph.Edges[i].Start.Position;
                Point3d b = (Point3d)spaceGraph.Edges[i].End.Position;

                Polyline out_c = MeshUtils.OffsetEdge(a, b, thickness);
                wallCrvs.Add(out_c);
            }

            foreach (var c in wallCrvs)
            {
                Wall w = new Wall();

                Polyline cMoved = new Polyline();
                cMoved = c.Duplicate();
                Transform move = Transform.Translation(Rhino.Geometry.Vector3d.ZAxis * height);
                cMoved.Transform(move);

                w.WallMesh = MeshUtils.CreateLoftClosed(c, cMoved);
                walls.Add(w);
            }


        }
    }
}
