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

namespace Andrea.Architecture
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SpaceLayout
    {
        public HeMesh3d spaceGraph;
        public HeMesh3d inputMesh;

        public WallSystem wallSystem;

        public SpaceLayout(HeMesh3d _inputMesh)
        {
            spaceGraph = new HeMesh3d();
            inputMesh = _inputMesh;
        }

        public SpaceLayout(string path_SpaceGraph, HeMesh3d _inputMesh)
        {
            spaceGraph = HeMesh3d.Factory.CreateFromOBJ(path_SpaceGraph);
            inputMesh = _inputMesh;

            MorphGraph();
        }

        public void MorphGraph()
        {
            List<Point3d> vs = new List<Point3d>();

            //TODO pass corners list in input Mesh.
            Point3d a0 = spaceGraph.Vertices[0].Position;
            Point3d a1 = spaceGraph.Vertices[1].Position;
            Point3d a2 = spaceGraph.Vertices[3].Position;
            Point3d a3 = spaceGraph.Vertices[2].Position;

            Point3d b0 = inputMesh.Vertices[0].Position;
            Point3d b1 = inputMesh.Vertices[1].Position;
            Point3d b2 = inputMesh.Vertices[3].Position;
            Point3d b3 = inputMesh.Vertices[2].Position;

            Surface sA = (Surface)Rhino.Geometry.NurbsSurface.CreateFromCorners(a0, a1, a2, a3);
            Surface sB = (Surface)Rhino.Geometry.NurbsSurface.CreateFromCorners(b0, b1, b2, b3);

            Rhino.Geometry.Morphs.SporphSpaceMorph sporph = new Rhino.Geometry.Morphs.SporphSpaceMorph(sA, sB, new Point2d(0.5, 0.5), new Point2d(0.5, 0.5));

            foreach (var v in spaceGraph.Vertices)
                v.Position = sporph.MorphPoint(v.Position);
        }

        public void CreateWallSystem(double height, double thickness)
        {
            wallSystem = new WallSystem(spaceGraph);
            wallSystem.Create(height, thickness);
            //throw new ArgumentException("hey Cesar");
        }

    }
}
