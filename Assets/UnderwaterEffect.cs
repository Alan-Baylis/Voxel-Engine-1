using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class UnderwaterEffect : MonoBehaviour {

	public PostProcessingProfile normal;
	public PostProcessingProfile underwater;
	public PostProcessingBehaviour ppb;
	public Transform target;
	public World world;

	void Update () {
		if (world.PositionToVoxel (target.position) != null && world.PositionToVoxel (target.position).blockID == 9) {
			// Water height check (because water isn't at the top of the block)
			if (target.position.y <= WorldGen.seaLevel + ChunkMeshBuilder.wh) {
				ppb.profile = underwater;
			} else {
				ppb.profile = normal;
			}
		} else {
			ppb.profile = normal;
		}
	}
}
