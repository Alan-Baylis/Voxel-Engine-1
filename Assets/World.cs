using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class World : MonoBehaviour {

	Dictionary<Vector2, Chunk> chunks = new Dictionary<Vector2, Chunk> ();

	public Material chunkMaterial;

	public float seed = -1f;

	void Awake () {
		if (seed == -1f) {
			seed = Random.Range (-100000f, 100000f);
		}
	}

	void Start () {
		LoadChunk (0, 0);
		/*
		LoadChunk (-1, 0);
		LoadChunk (0, -1);
		LoadChunk (-1, -1);
		*/
	}

	void GenerateChunk (int x, int z) {
		StartCoroutine (genchunk (x, z));
	}

	IEnumerator genchunk (int x, int z) {
		Chunk c = null;
		c = new Chunk (x, z, this);

		while (!c.isInitialised) {
			yield return new WaitForEndOfFrame ();
		}
		c.Render ();

		chunks [new Vector2 (x, z)] = c;
	}

	public void LoadChunk (int x, int z) {
		if (chunks.ContainsKey (new Vector2 (x, z))) {
			if (chunks [new Vector2 (x, z)].go != null) {
				bool isActive = chunks [new Vector2 (x, z)].go.activeSelf;
				if (!isActive) {
					// If the chunk just got loaded from an unloaded state
					chunks [new Vector2 (x, z)].go.SetActive (true);
					chunks [new Vector2 (x, z)].Render ();
				}
			} else {
				// Still generating chunk
			}
		} else {
			GenerateChunk (x, z);
		}
	}

	public void UnloadChunk (int x, int z) {
		if (chunks.ContainsKey (new Vector2 (x, z))) {
			if (chunks [new Vector2 (x, z)].go != null) {
				chunks [new Vector2 (x, z)].go.SetActive (false);
			}
		}
	}

	public Chunk PositionToChunk (Vector3 pos) {
		int chunkX = Mathf.FloorToInt (pos.x / (float)Chunk.width);
		int chunkZ = Mathf.FloorToInt (pos.z / (float)Chunk.width);
		if (!chunks.ContainsKey (new Vector2 (chunkX, chunkZ))) {
			GenerateChunk (chunkX, chunkZ);
		} else {
			return chunks [new Vector2 (chunkX, chunkZ)];
		}
		return null;
	}

	public Voxel PositionToVoxel (Vector3 pos) {
		Chunk chunk = PositionToChunk (pos);
		int voxelX = Mathf.FloorToInt (pos.x % Chunk.width);
		int voxelY = Mathf.FloorToInt (pos.y);
		int voxelZ = Mathf.FloorToInt (pos.z % Chunk.width);

		if (voxelY > Chunk.height - 1) {
			print ("You may not build above the world height limit!");
			return null;
		}

		if (voxelX < 0)
			voxelX += Chunk.width;
 		if (voxelZ < 0)
			voxelZ += Chunk.width;

		if (chunk != null) {
			Voxel[,,] v = chunk.voxels;

			if (v.GetLength (0) > voxelX && v.GetLength (1) > voxelY && v.GetLength (2) > voxelZ && voxelX >= 0 && voxelY >= 0 && voxelZ >= 0) {
				return v [voxelX, voxelY, voxelZ];
			}
		}

		int chunkX = Mathf.FloorToInt (pos.x / (float)Chunk.width);
		int chunkZ = Mathf.FloorToInt (pos.z / (float)Chunk.width);
		print ("ERROR: Could not get voxel at:\nChunk: " + chunkX + "," + chunkZ + "	Voxel: " + voxelX + "," + voxelY + "," + voxelZ);
		return null;
	}

	// CURRENTLY NONFUNCTIONAL
	public Vector3 VoxelToPosition (Voxel v) {
		int chunkX = v.parent.posX;
		int chunkZ = v.parent.posZ;

		return new Vector3 (chunkX * Chunk.width + v.posX, v.posY, chunkZ * Chunk.width + v.posZ);
	}

	public bool HasChunk (int x, int z) {
		return chunks.ContainsKey (new Vector2 (x, z));
	}

	public Chunk GetChunk (int x, int z) {
		return chunks [new Vector2 (x, z)];
	}

	public void SetVoxel (int x, int y, int z, short id) {
		PositionToVoxel (new Vector3 (x, y, z)).Set (id);
	}
}
