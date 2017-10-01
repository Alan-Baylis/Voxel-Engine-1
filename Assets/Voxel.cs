using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel {

	/** BLOCK IDS:
	 * 0 : AIR
	 * 1 : STONE
	 * 2 : DIRT
	 * 3 : GRASS
	 * 4 : SAND
	 * */

	short id;
	int x, y, z;
	public Chunk parent;

	public Voxel (short _id, int _x, int _y, int _z) {
		id = _id;
		x = _x;
		y = _y;
		z = _z;
	}

	/**
	 * 0 : Bottom (Y-)	1 : Top (Y+)
	 * 2 : Left (X-)	3 : Right (X+)
	 * 4 : Back (Z-)	5 : Front (Z+)
	 **/
	public List<int> GetOpenFaces () {
		List<int> openFaces = new List<int> (0);
		List<int> waterFaces = new List<int> (0);
		World w = parent.world;

		if (id == 0) {
			// Don't draw any faces if this is an air block!
			return openFaces;
		} else if (id == 9) {
			// Water doesn't need non-top faces rendered or top faces if there's water above it
			if (y == Chunk.height - 1 || (parent.voxels [x, y + 1, z].transparent && parent.voxels [x, y + 1, z].blockID != 9)) {
				waterFaces.Add (6); // Water's special lowered-top face
				waterFaces.Add (7); // Water's special inverted-top face
			}
			return waterFaces;
		}

		if (y == 0 || parent.voxels [x, y - 1, z].transparent) {
			// Bottom
			if (y != 0) {
				// Don't render world bottoms
				openFaces.Add (0);
			}
		}
		if (y == Chunk.height - 1 || parent.voxels [x, y + 1, z].transparent) {
			// Top
			openFaces.Add(1);
		}
		if (x == 0 || parent.voxels [x - 1, y, z].transparent) {
			// Left
			if (x == 0 && w.HasChunk (parent.posX - 1, parent.posZ)) {
				if (w.GetChunk (parent.posX - 1, parent.posZ).voxels [Chunk.width - 1, y, z].transparent) {
					// Don't render chunk edges if occluded by other chunks
					openFaces.Add (2);
				}
			} else {
				openFaces.Add (2);
			}
		}
		if (x == Chunk.width - 1 || parent.voxels [x + 1, y, z].transparent) {
			// Right
			if (x == Chunk.width - 1 && w.HasChunk (parent.posX + 1, parent.posZ)) {
				if (w.GetChunk (parent.posX + 1, parent.posZ).voxels [0, y, z].transparent) {
					// Don't render chunk edges if occluded by other chunks
					openFaces.Add (3);
				}
			} else {
				openFaces.Add (3);
			}
		}
		if (z == 0 || parent.voxels [x, y, z - 1].transparent) {
			// Back
			if (z == 0 && w.HasChunk (parent.posX, parent.posZ - 1)) {
				if (w.GetChunk (parent.posX, parent.posZ - 1).voxels [x, y, Chunk.width - 1].transparent) {
					// Don't render chunk edges if occluded by other chunks
					openFaces.Add (4);
				}
			} else {
				openFaces.Add (4);
			}
		}
		if (z == Chunk.width - 1 || parent.voxels [x, y, z + 1].transparent) {
			// Front
			if (z == Chunk.width - 1 && w.HasChunk (parent.posX, parent.posZ + 1)) {
				if (w.GetChunk (parent.posX, parent.posZ + 1).voxels [x, y, 0].transparent) {
					// Don't render chunk edges if occluded by other chunks
					openFaces.Add (5);
				}
			} else {
				openFaces.Add (5);
			}
		}
		return openFaces;
	}

	public Vector2 UV_BL {
		get {
			/*switch (id) {
			case 1:
				return new Vector2(1f / ChunkMeshBuilder.n, 0f);
			case 2:
				return new Vector2(2f / ChunkMeshBuilder.n, 0f);
			case 3:
				return new Vector2(3f / ChunkMeshBuilder.n, 0f);
			case 4:
				return new Vector2(4f / ChunkMeshBuilder.n, 0f);
			case 5:
				return new Vector2(5f / ChunkMeshBuilder.n, 0f);
			default:
				return Vector2.zero;
			}*/
			return new Vector2 (((float)id / (float)ChunkMeshBuilder.n) % (float)ChunkMeshBuilder.n, Mathf.Floor (id / ChunkMeshBuilder.n));
		}
	}

	public short blockID {
		get {
			return id;
		}
	}

	public int posX {
		get {
			return x;
		}
	}

	public int posY {
		get {
			return y;
		}
	}

	public int posZ {
		get {
			return z;
		}
	}

	public bool transparent {
		get {
			return WorldGen.blocks [id].transparent;
		}
	}

	public void SetWithoutReload (short _id) {
		id = _id;
	}

	public void Set (short _id) {
		id = _id;
		if(id == 0) // Only check if water flows in if this voxel is now air
			FlowInSurroundingWater ();
		Reload ();
	}

	void FlowInSurroundingWater () {
		Vector3 pos = parent.world.VoxelToPosition (this);

		bool setAsWater = false;

		List<Voxel> neighbours = new List<Voxel> ();
		neighbours.Add (parent.world.PositionToVoxel (pos + Vector3.left));
		neighbours.Add (parent.world.PositionToVoxel (pos + Vector3.right));
		neighbours.Add (parent.world.PositionToVoxel (pos + Vector3.forward));
		neighbours.Add (parent.world.PositionToVoxel (pos + Vector3.back));
		neighbours.Add (parent.world.PositionToVoxel (pos + Vector3.up));
		// Not bottom as we don't water to flow from a lower neighbour up

		foreach (Voxel v in neighbours) {
			if (v != null && v.blockID == 9) {
				SetWithoutReload (9);
				setAsWater = true;
				break;
			}
		}

		if (setAsWater) {
			Reload ();
		}
	}

	public void Reload () {
		parent.Render ();

		// Reload surrounding chunks if this block is at a chunk edge on the X-axis
		if (x == 0) {
			if (parent.world.HasChunk (parent.posX - 1, parent.posZ)) {
				parent.world.GetChunk (parent.posX - 1, parent.posZ).Render ();
			}
		} else if (x == Chunk.width - 1) {
			if (parent.world.HasChunk (parent.posX + 1, parent.posZ)) {
				parent.world.GetChunk (parent.posX + 1, parent.posZ).Render ();
			}
		}

		// Same with the Z-axis
		if (z == 0) {
			if (parent.world.HasChunk (parent.posX, parent.posZ - 1)) {
				parent.world.GetChunk (parent.posX, parent.posZ - 1).Render ();
			}
		} else if (z == Chunk.width - 1) {
			if (parent.world.HasChunk (parent.posX, parent.posZ + 1)) {
				parent.world.GetChunk (parent.posX, parent.posZ + 1).Render ();
			}
		}
	}
}
