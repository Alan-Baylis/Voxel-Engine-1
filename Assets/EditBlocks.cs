using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditBlocks : MonoBehaviour {

	public short currentBlock = 1;
	public float maxDistance = 8000f;
	public World world;
	public Text display;

	void Update () {

		Vector3 pos = Vector3.zero;
		Vector3 norm = Vector3.zero;
		RaycastHit hit;
		if (Physics.Raycast (Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance)) {
			pos = hit.point;
			norm = hit.normal;
		}

		if (pos != Vector3.zero && norm != Vector3.zero) {
			if (Input.GetMouseButton/*Down*/ (0)) {
				Voxel v = world.PositionToVoxel (pos - norm / 2f);
				// Only break bedrock if bedrock is selected
				if (v.blockID != 5 || (v.blockID == 5 && currentBlock == 5)) {
					v.Set (0);
				}
			}
			if (Input.GetMouseButtonDown (1)) {
				Voxel v = world.PositionToVoxel (pos + norm / 2f);
				if (v != null/* && !Physics.CheckBox (world.VoxelToPosition (v), Vector3.one * 0.49f)*/) {
					v.Set (currentBlock);
				}
			}
		}

		if (Input.GetKeyDown (KeyCode.Equals)) {
			currentBlock = (short) (currentBlock + 1 % (WorldGen.blocks.Length - 1));
			if (currentBlock == 9) {
				// Don't place water; it's broken
				currentBlock = (short) (currentBlock + 1 % (WorldGen.blocks.Length - 1));
			}
		}
		if (Input.GetKeyDown (KeyCode.Minus)) {
			currentBlock = (short) (currentBlock - 1 % (WorldGen.blocks.Length - 1));
			if (currentBlock == 9) {
				// Don't place water; it's broken
				currentBlock = (short) (currentBlock - 1 % (WorldGen.blocks.Length - 1));
			}
		}

		currentBlock = (short)Mathf.Clamp ((int)currentBlock, 1, WorldGen.blocks.Length - 1);
		display.text = "Current Block:\n<color=#ff3333>" + WorldGen.blocks [currentBlock].name + "</color>";
	}
}
