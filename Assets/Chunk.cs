using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Chunk {

	public static int width = 16;
	public static int height = 128;

	static int numTrees = 3;
	static int leafRadius = 2;
	static int minTreeHeight = 4;
	static int maxTreeHeight = 7;

	static short treeTrunkID = 6;
	static short treeLeafID = 8;

	public bool renderInProgress = false;

	public volatile Voxel[,,] voxels = new Voxel[width, height, width];

	public GameObject go;

	public volatile bool isInitialised = false;

	public World world;

	public ChunkMeshObject cmo = null;

	int x, z;

	public Chunk (int _x, int _z, World _world) {
		x = _x;
		z = _z;
		world = _world;
		InitVoxels (this);
	}

	void InitVoxels (Chunk c) {

		float seed = world.seed;

		Thread t = new Thread (() => {
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					for (int k = 0; k < width; k++) {
						int noiseX = i + (width * x);
						int noiseY = k + (width * z);

						voxels[i,j,k] = new Voxel(WorldGen.Get(noiseX, j, noiseY, seed), i, j, k);
						voxels [i, j, k].parent = c;
					}
				}
			}
			GenerateTrees ();
			isInitialised = true;
		});
		t.Start ();
	}

	void GenerateTrees () {
		System.Random r = new System.Random ();
		for (int i = 0; i < numTrees; i++) {
			int x = r.Next (leafRadius, width - 1 - leafRadius); //Random.Range (leafRadius, width - 1 - leafRadius);
			int y = 0;
			int z = r.Next (leafRadius, width - 1 - leafRadius);
			bool startYFound = false;
			while (!startYFound && y < height - 1) {
				y++;
				if (voxels [x, y, z] != null) {
					if (voxels [x, y, z].blockID == 0) {
						startYFound = true;
					} else if (voxels [x, y, z].blockID == 6) {
						// No stacked trees!
						break;
					} else if (voxels [x, y, z].blockID == 9) {
						// No underwater trees!
						break;
					}
				}
			}

			if (startYFound) {
				int treeHeight = r.Next (minTreeHeight, maxTreeHeight);
				for (int j = 0; j < treeHeight; j++) {
					if (y + j < height && voxels [x, y + j, z] != null) {
						voxels [x, y + j, z].SetWithoutReload (treeTrunkID);
					}
				}

				for (int k = -leafRadius; k <= leafRadius; k++) {
					for (int l = -leafRadius; l <= leafRadius; l++) {
						for (int m = -leafRadius; m <= leafRadius; m++) {
							if (Vector3.Distance (new Vector3 (x, y + treeHeight - 1, z), new Vector3 (x + k, y + treeHeight - 1 + l, z + m)) <= leafRadius) {
								if (voxels [x + k, y + treeHeight - 1 + l, z + m].blockID == 0) {
									voxels [x + k, y + treeHeight - 1 + l, z + m].SetWithoutReload (treeLeafID);
								}
							}
						}
					}
				}
			}
		}
		isInitialised = true;
	}

	public void Render () {
		if (isInitialised) {
			new GameObject ("Coroutine Object").AddComponent<CoroutineObject> ().BuildMesh (this, doneRendering);
		}
	}

	public delegate void renderChunkDelegate (ChunkMeshObject cmo);

	void doneRendering (ChunkMeshObject cmo) {
		ApplyMeshObject (cmo);
		go.GetComponent<MeshRenderer> ().material = world.chunkMaterial;
	}

	void ApplyMeshObject (ChunkMeshObject _cmo) {

		cmo = _cmo;

		MonoBehaviour.Destroy (go);
		go = new GameObject ("Chunk");
		go.transform.position = new Vector3 (posX * Chunk.width, 0f, posZ * Chunk.width);

		Mesh m = new Mesh ();
		m.vertices = cmo.verts.ToArray();
		m.triangles = cmo.tris.ToArray();
		m.uv = cmo.uvs.ToArray();
		m.RecalculateBounds();
		m.RecalculateNormals();

		go.AddComponent<MeshFilter>().mesh = m;
		go.AddComponent<MeshRenderer>();

		AddColliders ();

		/*
		GameObject col = new GameObject("Collider");
		col.transform.parent = go.transform;
		col.transform.localPosition = Vector3.zero;
		col.AddComponent<MeshCollider> ().sharedMesh = m;
		*/

		/*
		GameObject col = new GameObject("Colliders");
		col.transform.parent = go.transform;

		for(int i = 0; i < cmo.colliderPositions.Count; i++) {
			col.AddComponent<BoxCollider> ().center = cmo.colliderPositions [i] + (Vector3.one * 0.5f) + new Vector3 (posX * Chunk.width, 0f, posZ * Chunk.width);
		}
		*/
	}

	public void AddColliders () {
		if (cmo != null) {
			new GameObject ("Coroutine Object").AddComponent<CoroutineObject> ().AddColliders (this, go, cmo.colliderPositions);
		}
	}

	public void RemoveColliders () {
		if (go.transform.Find ("Colliders") != null) {
			MonoBehaviour.Destroy (go.transform.Find ("Colliders").gameObject);
		}
	}

	public int posX {
		get {
			return x;
		}
	}

	public int posZ {
		get {
			return z;
		}
	}
}
