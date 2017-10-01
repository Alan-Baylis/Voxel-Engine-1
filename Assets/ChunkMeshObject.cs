using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMeshObject {

	public List<Vector3> verts;
	public List<int> tris;
	public List<Vector2> uvs;
	public List<Vector3> colliderPositions;

	public bool hasWater = false;
	public List<Vector3> waterVerts;
	public List<int> waterTris;
	public List<Vector2> waterUvs;

	public ChunkMeshObject (List<Vector3> _verts, List<int> _tris, List<Vector2> _uvs, List<Vector3> colliders) {
		verts = _verts;
		tris = _tris;
		uvs = _uvs;
		colliderPositions = colliders;
	}

	public ChunkMeshObject (List<Vector3> _verts, List<int> _tris, List<Vector2> _uvs, List<Vector3> colliders, List<Vector3> _waterVerts, List<int> _waterTris, List<Vector2> _waterUvs) {
		verts = _verts;
		tris = _tris;
		uvs = _uvs;
		colliderPositions = colliders;
		hasWater = true;
		waterVerts = _waterVerts;
		waterTris = _waterTris;
		waterUvs = _waterUvs;
	}
}
