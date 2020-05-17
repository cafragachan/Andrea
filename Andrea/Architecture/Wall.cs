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
    public class Wall
    {
        private Mesh wallMesh;

        public Wall()
        {
            wallMesh = new Mesh();
        }

        public Mesh WallMesh
        {
            get { return wallMesh; }
            set { wallMesh = value; }
        }
       
    }
}
