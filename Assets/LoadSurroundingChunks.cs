﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSurroundingChunks : MonoBehaviour {

	public World world;
	public int radius = 10;
	public int collisionRadius = 3;

	void Update () {
		Chunk current = world.PositionToChunk (transform.position);

		if (current == null) {
			// Waiting for chunk to generate
			return;
		}

		for (int i = -radius + current.posX - 2; i <= radius + current.posX + 2; i++) {
			for (int j = -radius + current.posZ - 2; j <= radius + current.posZ + 2; j++) {

				float distToCurrent = Vector2.Distance (new Vector2 (i, j), new Vector2 (current.posX, current.posZ));

				if (distToCurrent <= radius) {
					world.LoadChunk (i, j);
				} else {
					world.UnloadChunk (i, j);
				}

				if (distToCurrent <= collisionRadius) {
					world.LoadChunkColliders (i, j);
				} else if (distToCurrent <= collisionRadius + 2) {
					world.UnloadChunkColliders (i, j);
				}
			}
		}
	}
}
