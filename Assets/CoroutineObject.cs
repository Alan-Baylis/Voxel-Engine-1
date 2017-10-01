using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CoroutineObject : MonoBehaviour {

	static int maxNumRunning = 5;
	static float timeout = 2f;

	public bool isRunning = false;

	void Awake () {
		this.gameObject.tag = "CoroutineObject";
	}

	public void BuildMesh (Chunk c, Chunk.renderChunkDelegate callback) {
		StartCoroutine (buildMesh (c, callback));
	}

	IEnumerator buildMesh (Chunk c, Chunk.renderChunkDelegate callback) {

		/*
		#region CoroutineObject No Stacking
		List<CoroutineObject> others = new List<CoroutineObject> (0);
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("CoroutineObject")) {
			if (g != this.gameObject) {
				others.Add(g.GetComponent<CoroutineObject>());
			}
		}
		bool needsToWait = true;
		while(needsToWait) {
			int numRunning = 0;
			foreach(CoroutineObject co in others) {
				if(co.isRunning) {
					numRunning++;
				}
			}

			if(numRunning > maxNumRunning) {
				needsToWait = true;
			} else {
				needsToWait = false;
			}

			if(needsToWait) {
				yield return null;
			}
		}
		#endregion
		*/

		isRunning = true;

		float time = 0.0f;

		ChunkMeshObject cmo = null;
		bool done = false;
		Thread thread = new Thread (() => {
			cmo = ChunkMeshBuilder.BuildMesh (c);
			done = true;
		});
		thread.Start ();

		while (!done && time < timeout) {
			yield return new WaitForEndOfFrame ();
			time += Time.deltaTime;
		}

		if (time >= timeout) {
			BuildMesh (c, callback);
			thread.Abort ();
			print ("CoroutineObject timed out building mesh. Retrying...");
			yield break;
		}
		callback.Invoke (cmo);
		Destroy (this.gameObject);
	}

	public void AddColliders (Chunk c, GameObject g, List<Vector3> centres) {
		StartCoroutine (addColliders (c, g, centres));
	}

	IEnumerator addColliders (Chunk c, GameObject g, List<Vector3> centres) {
		isRunning = true;

		GameObject col = new GameObject("Colliders");
		col.transform.parent = g.transform;

		int iterator = 0;
		int numPerFrame = 100;

		for(int i = 0; i < centres.Count; i++) {

			if (col == null || g == null) {
				yield break;
			}

			col.AddComponent<BoxCollider> ().center = centres [i] + (Vector3.one * 0.5f) + new Vector3 (c.posX * Chunk.width, 0f, c.posZ * Chunk.width);
			iterator++;

			if (iterator >= numPerFrame) {
				iterator = 0;
				yield return new WaitForEndOfFrame ();
			}
		}

		Destroy (this.gameObject);
	}
}
