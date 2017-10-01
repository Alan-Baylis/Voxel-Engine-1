using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {

	public readonly short id;
	public readonly string name;
	public readonly bool transparent;
	public readonly bool collides;

	public Block (short _id, string _name, bool _transparent, bool _collides) {
		id = _id;
		name = _name;
		transparent = _transparent;
		collides = _collides;
	}
}
