using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkMeshBuilder {

	public static int n = 16;	// Number of cells in one row of the texture atlas
	public static float m = 0;	// Margin around texture box to prevent bleed from surrounding textures on the atlas
	public static float wh = 0.75f;	// Water height (eg if wh was 0.9 then the top rendered face of water would be 0.1 below normal block faces)

	public static ChunkMeshObject BuildMesh (Chunk c) {

		List<Vector3> verts = new List<Vector3> (0);
		List<int> tris = new List<int> (0);
		List<Vector2> uvs = new List<Vector2> (0);
		List<Vector3> colliderPositions = new List<Vector3> (0);

		List<Vector3> waterVerts = new List<Vector3> (0);
		List<int> waterTris = new List<int> (0);
		List<Vector2> waterUvs = new List<Vector2> (0);

		float mx = ChunkMeshBuilder.m;

		float oneNth = 1f / (float)n;

		for (int i = 0; i < Chunk.width; i++) {
			for (int j = 0; j < Chunk.height; j++) {
				for (int k = 0; k < Chunk.width; k++) {
					Voxel a = c.voxels [i, j, k];
					List<int> faces = a.GetOpenFaces ();

					bool isWater = false;

					if (faces [0] == 6 && faces [1] == 7) {
						// This is a water block; move it to a separate mesh
						isWater = true;
					}

					if (faces.Count > 0) {
						// This block has some faces to render!
						// OLD IMPLEMENTATION: int si = verts.Count; // Start index;
						/**
						 * 0 : left-bottom-back		1 : left-top-back
						 * 2 : right-top-back		3 : right-bottom-back
						 * 4 : left-bottom-front	5 : left-top-front
						 * 6 : right-top-front		7 : right-bottom-front
						 * */

						#region NEW IMPLEMENTATION
						for (int l = 0; l < faces.Count; l++) {
							int si = verts.Count; // Start index;
							switch (faces [l]) {
							case 0:
								// Bottom
								verts.Add (new Vector3 (i, j, k));
								verts.Add (new Vector3 (i + 1, j, k));
								verts.Add (new Vector3 (i, j, k + 1));
								verts.Add (new Vector3 (i + 1, j, k + 1));

								uvs.Add (new Vector2 (a.UV_BL.x + oneNth - mx, a.UV_BL.y + mx));
								uvs.Add (new Vector2 (a.UV_BL.x + mx, a.UV_BL.y + mx));
								uvs.Add (new Vector2 (a.UV_BL.x + oneNth - mx, a.UV_BL.y + oneNth - mx));
								uvs.Add (new Vector2 (a.UV_BL.x + mx, a.UV_BL.y + oneNth - mx));

								tris.Add (si + 0);
								tris.Add (si + 1);
								tris.Add (si + 3);

								tris.Add (si + 0);
								tris.Add (si + 3);
								tris.Add (si + 2);
								break;
							case 1:
								// Top
								verts.Add (new Vector3 (i, j + 1, k));
								verts.Add (new Vector3 (i, j + 1, k + 1));
								verts.Add (new Vector3 (i + 1, j + 1, k));
								verts.Add (new Vector3 (i + 1, j + 1, k + 1));

								uvs.Add (new Vector2 (a.UV_BL.x + oneNth - mx, a.UV_BL.y + mx));
								uvs.Add (new Vector2 (a.UV_BL.x + mx, a.UV_BL.y + mx));
								uvs.Add (new Vector2 (a.UV_BL.x + oneNth - mx, a.UV_BL.y + oneNth - mx));
								uvs.Add (new Vector2 (a.UV_BL.x + mx, a.UV_BL.y + oneNth - mx));

								tris.Add (si + 0);
								tris.Add (si + 1);
								tris.Add (si + 3);

								tris.Add (si + 0);
								tris.Add (si + 3);
								tris.Add (si + 2);
								break;
							case 2:
								// Left
								verts.Add (new Vector3 (i, j, k));
								verts.Add (new Vector3 (i, j, k + 1));
								verts.Add (new Vector3 (i, j + 1, k));
								verts.Add (new Vector3 (i, j + 1, k + 1));

								uvs.Add (new Vector2 (a.UV_BL.x + oneNth - mx, a.UV_BL.y + mx));
								uvs.Add (new Vector2 (a.UV_BL.x + mx, a.UV_BL.y + mx));

								uvs.Add (new Vector2 (a.UV_BL.x + oneNth - mx, a.UV_BL.y + oneNth - mx));
								uvs.Add (new Vector2 (a.UV_BL.x + mx, a.UV_BL.y + oneNth - mx));

								tris.Add (si + 0);
								tris.Add (si + 1);
								tris.Add (si + 3);

								tris.Add (si + 0);
								tris.Add (si + 3);
								tris.Add (si + 2);
								break;
							case 3:
								// Right
								verts.Add (new Vector3 (i + 1, j, k));
								verts.Add (new Vector3 (i + 1, j + 1, k));
								verts.Add (new Vector3 (i + 1, j, k + 1));
								verts.Add (new Vector3 (i + 1, j + 1, k + 1));

								uvs.Add (new Vector2 (a.UV_BL.x + mx, a.UV_BL.y + mx));
								uvs.Add (new Vector2 (a.UV_BL.x + mx, a.UV_BL.y + oneNth - mx));
								uvs.Add (new Vector2 (a.UV_BL.x + oneNth - mx, a.UV_BL.y + mx));
								uvs.Add (new Vector2 (a.UV_BL.x + oneNth - mx, a.UV_BL.y + oneNth - mx));

								tris.Add (si + 0);
								tris.Add (si + 1);
								tris.Add (si + 3);

								tris.Add (si + 0);
								tris.Add (si + 3);
								tris.Add (si + 2);
								break;
							case 4:
								// Back
								verts.Add (new Vector3 (i, j, k));
								verts.Add (new Vector3 (i, j + 1, k));
								verts.Add (new Vector3 (i + 1, j, k));
								verts.Add (new Vector3 (i + 1, j + 1, k));

								uvs.Add (new Vector2 (a.UV_BL.x + mx, a.UV_BL.y + mx));
								uvs.Add (new Vector2 (a.UV_BL.x + mx, a.UV_BL.y + oneNth - mx));
								uvs.Add (new Vector2 (a.UV_BL.x + oneNth - mx, a.UV_BL.y + mx));
								uvs.Add (new Vector2 (a.UV_BL.x + oneNth - mx, a.UV_BL.y + oneNth - mx));

								tris.Add (si + 0);
								tris.Add (si + 1);
								tris.Add (si + 3);

								tris.Add (si + 0);
								tris.Add (si + 3);
								tris.Add (si + 2);
								break;
							case 5:
								// Front
								verts.Add (new Vector3 (i, j, k + 1));
								verts.Add (new Vector3 (i + 1, j, k + 1));
								verts.Add (new Vector3 (i, j + 1, k + 1));
								verts.Add (new Vector3 (i + 1, j + 1, k + 1));

								uvs.Add (new Vector2 (a.UV_BL.x + oneNth - mx, a.UV_BL.y + mx));
								uvs.Add (new Vector2 (a.UV_BL.x + mx, a.UV_BL.y + mx));

								uvs.Add (new Vector2 (a.UV_BL.x + oneNth - mx, a.UV_BL.y + oneNth - mx));
								uvs.Add (new Vector2 (a.UV_BL.x + mx, a.UV_BL.y + oneNth - mx));

								tris.Add (si + 0);
								tris.Add (si + 1);
								tris.Add (si + 3);

								tris.Add (si + 0);
								tris.Add (si + 3);
								tris.Add (si + 2);
								break;
							case 6:
								// Water top
								waterVerts.Add (new Vector3 (i, j + wh, k));
								waterVerts.Add (new Vector3 (i, j + wh, k + 1));
								waterVerts.Add (new Vector3 (i + 1, j + wh, k));
								waterVerts.Add (new Vector3 (i + 1, j + wh, k + 1));

								waterUvs.Add (new Vector2 (a.UV_BL.x + oneNth - mx, a.UV_BL.y + mx));
								waterUvs.Add (new Vector2 (a.UV_BL.x + mx, a.UV_BL.y + mx));
								waterUvs.Add (new Vector2 (a.UV_BL.x + oneNth - mx, a.UV_BL.y + oneNth - mx));
								waterUvs.Add (new Vector2 (a.UV_BL.x + mx, a.UV_BL.y + oneNth - mx));

								waterTris.Add (si + 0);
								waterTris.Add (si + 1);
								waterTris.Add (si + 3);

								waterTris.Add (si + 0);
								waterTris.Add (si + 3);
								waterTris.Add (si + 2);
								break;
							case 7:
								// Inverted water top
								waterVerts.Add (new Vector3 (i, j + wh, k));
								waterVerts.Add (new Vector3 (i, j + wh, k + 1));
								waterVerts.Add (new Vector3 (i + 1, j + wh, k));
								waterVerts.Add (new Vector3 (i + 1, j + wh, k + 1));

								waterUvs.Add (new Vector2 (a.UV_BL.x + oneNth - mx, a.UV_BL.y + mx));
								waterUvs.Add (new Vector2 (a.UV_BL.x + mx, a.UV_BL.y + mx));
								waterUvs.Add (new Vector2 (a.UV_BL.x + oneNth - mx, a.UV_BL.y + oneNth - mx));
								waterUvs.Add (new Vector2 (a.UV_BL.x + mx, a.UV_BL.y + oneNth - mx));

								waterTris.Add (si + 0);
								waterTris.Add (si + 2);
								waterTris.Add (si + 1);

								waterTris.Add (si + 2);
								waterTris.Add (si + 3);
								tris.Add (si + 1);
								break;
							}
						}
						#endregion
						#region Collider Generation
						if(WorldGen.blocks[a.blockID].collides)
							colliderPositions.Add (new Vector3 (i, j, k));
						#endregion
					}
				}
			}
		}

		return new ChunkMeshObject (verts, tris, uvs, colliderPositions);
	}
}

#region OLD IMPLEMENTATION
/*
						#region vertices
						verts.Add(new Vector3(i, j, k));					// LBB
						verts.Add(new Vector3(i, j + 1, k));				// LTB
						verts.Add(new Vector3(i + 1, j + 1, k));			// RTB
						verts.Add(new Vector3(i + 1, j, k));				// RBB
						verts.Add(new Vector3(i, j, k + 1));				// LBF
						verts.Add(new Vector3(i, j + 1, k + 1));			// LTF
						verts.Add(new Vector3(i + 1, j + 1, k + 1));		// RTF
						verts.Add(new Vector3(i + 1, j, k + 1));			// RBF
						#endregion
						#region triangles
						/**
						* 0 : Bottom (Y-)	1 : Top (Y+)
						* 2 : Left (X-)	3 : Right (X+)
						* 4 : Back (Z-)	5 : Front (Z+)
						*//*/
for(int l = 0; l < faces.Count; l++) {
	switch(faces[l]) {
	case 0:
		// Bottom
		tris.Add(si + 0);
		tris.Add(si + 3);
		tris.Add(si + 7);

		tris.Add(si + 0);
		tris.Add(si + 7);
		tris.Add(si + 4);
		break;
	case 1:
		// Top
		tris.Add(si + 1);
		tris.Add(si + 6);
		tris.Add(si + 2);

		tris.Add(si + 1);
		tris.Add(si + 5);
		tris.Add(si + 6);
		break;
	case 2:
		// Left
		tris.Add(si + 0);
		tris.Add(si + 4);
		tris.Add(si + 5);

		tris.Add(si + 0);
		tris.Add(si + 5);
		tris.Add(si + 1);
		break;
	case 3:
		// Right
		tris.Add(si + 3);
		tris.Add(si + 2);
		tris.Add(si + 6);

		tris.Add(si + 3);
		tris.Add(si + 6);
		tris.Add(si + 7);
		break;
	case 4:
		// Back
		tris.Add(si + 0);
		tris.Add(si + 1);
		tris.Add(si + 2);

		tris.Add(si + 0);
		tris.Add(si + 2);
		tris.Add(si + 3);
		break;
	case 5:
		// Front
		tris.Add(si + 4);
		tris.Add(si + 7);
		tris.Add(si + 6);

		tris.Add(si + 4);
		tris.Add(si + 6);
		tris.Add(si + 5);
		break;
	}
}
#endregion
*/
#endregion