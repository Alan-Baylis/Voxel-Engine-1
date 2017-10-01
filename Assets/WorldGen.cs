using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldGen {

	/** BLOCK IDS:
	 * 0 : AIR
	 * 1 : STONE
	 * 2 : DIRT
	 * 3 : GRASS
	 * 4 : SAND
	 * 5 : BEDROCK
	 * 6 : LOG
	 * 7 : PLANKS
	 * */

	public static readonly Block[] blocks = {
		new Block (0, "Air", true, false),
		new Block (1, "Stone", false, true),
		new Block (2, "Dirt", false, true),
		new Block (3, "Grass", false, true),
		new Block (4, "Sand", false, true),
		new Block (5, "Bedrock", false, true),
		new Block (6, "Wood Log", false, true),
		new Block (7, "Wood Planks", false, true),
		new Block (8, "Leaves", true, true),
		new Block (9, "Water", true, false),
		/*
		"Air",
		"Stone",
		"Dirt",
		"Grass",
		"Sand",
		"Bedrock",
		"Wood Log",
		"Wood Planks"
		*/
	};

	public static int seaLevel = 40;
	static int beachHeight = 3;

	static float baseNoiseScale = 0.01f;
	static float baseNoiseHeight = 64f;
	static float noiseScale = 0.05f;
	static float noiseHeight = 12f;
	static int stonePadding = 15;

	public static short Get (int x, int y, int z, float seed) {
		int baseNoise = (int)(Mathf.PerlinNoise (x * baseNoiseScale + 0.5f + seed, z * baseNoiseScale + 0.5f + seed) * baseNoiseHeight);
		int mainNoise = (int)(Mathf.PerlinNoise (x * noiseScale + 0.5f + seed, z * noiseScale + 0.5f + seed) * noiseHeight) + stonePadding;
		int value = baseNoise + mainNoise;

		if (value >= seaLevel) {
			if (y <= value) {
				if (y == 0) {
					return 5;
				}
				if (y < value - 3) {
					return 1;
				} else if (y < value) {
					if (y > seaLevel + beachHeight) {
						return 2;
					} else {
						return 4;
					}
				} else {
					// y = value
					if (value > seaLevel + beachHeight) {
						return 3;
					} else {
						return 4;
					}
				}
			} else {
				// y > value
				return 0;
			}
		} else {
			if (y < seaLevel) {
				if (y == 0) {
					return 5; // Bedrock at 0
				}
				if (y < value - 2) {
					return 1; // Stone up to the top 2 layers
				} else if (y <= value) {
					return 4; // Sand at the bottom of the sea
				} else {
					// y < waterLevel and y > value
					return 9;
				}
			} else {
				// y > waterLevel
				return 0;
			}
		}

		return 0; // If all else fails, it's air
	}
}
