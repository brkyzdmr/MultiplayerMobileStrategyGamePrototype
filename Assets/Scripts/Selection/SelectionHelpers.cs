using UnityEngine;

namespace MMSGP.Selection
{
    public static class SelectionHelpers
    {
        /// <summary>
        /// Create a bounding box (4 corners in order) from the start and end mouse position
        /// </summary>
        public static Vector2[] GetBoundingBox(Vector2 p1,Vector2 p2)
        {
            var bottomLeft = Vector3.Min(p1, p2);
            var topRight = Vector3.Max(p1, p2);

            Vector2[] corners =
            {
                new Vector2(bottomLeft.x, topRight.y), // top left
                new Vector2(topRight.x, topRight.y), // top right
                new Vector2(bottomLeft.x, bottomLeft.y), // bottom left
                new Vector2(topRight.x, bottomLeft.y) // bottom right
            };
            return corners;
        }
        
        /// <summary>
        /// Generate a mesh from the 4 bottom points
        /// </summary>
        public static Mesh GenerateSelectionMesh(Vector3[] corners, Vector3[] vecs)
        {
            var verts = new Vector3[8];
            int[] tris =
            {
                0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7
            }; //map the tris of our cube

            for(int i = 0; i < 4; i++)
            {
                verts[i] = corners[i];
            }

            for(int j = 4; j < 8; j++)
            {
                verts[j] = corners[j - 4] + vecs[j - 4];
            }

            var selectionMesh = new Mesh
            {
                vertices = verts,
                triangles = tris
            };

            return selectionMesh;
        }

        public static MeshCollider AddMeshCollider(GameObject go, Mesh selectionMesh)
        {
            var selectionBox = go.AddComponent<MeshCollider>();
            selectionBox.sharedMesh = selectionMesh;
            selectionBox.convex = true;
            selectionBox.isTrigger = true;

            return selectionBox;
        }
    }
}