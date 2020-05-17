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
    public class ArchManager
    {
        public SpaceLayout spaceLayout;

        public ArchManager() { }

        public void ImportSpaceLayout(string path_SpaceGraph, HeMesh3d _inputMesh)
        {
            spaceLayout = new SpaceLayout(path_SpaceGraph, _inputMesh);
        }

        public void CreateArchGeom()
        {
            //TODO change to style match
            spaceLayout.CreateWallSystem(3.00, 0.1);
        }

        public List<Mesh> GetWallSystemMesh()
        {
            List<Mesh> wallMeshes = new List<Mesh>();

            foreach (var w in spaceLayout.wallSystem.walls)
                wallMeshes.Add(w.WallMesh);

            return wallMeshes;
        }
    }
}
