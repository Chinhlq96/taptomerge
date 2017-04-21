using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMergeEffect : MonoBehaviour {

	// Use this for initialization
	public Vector2 preScale;
	
	// Update is called once per frame

	void Start ()
	{
		preScale = (Vector2)transform.localScale;
	}
	void Update () {
		transform.localScale = new Vector2 (transform.localScale.x * 1.1f, transform.localScale.y * 1.1f	);
		if (transform.localScale.x > preScale.x * 4f)
		{
			transform.localScale = new Vector2 (preScale.x, preScale.y);
			GetComponentInParent<BlockController> ().Merged = false;
			gameObject.SetActive (false);
		}
	}
}
