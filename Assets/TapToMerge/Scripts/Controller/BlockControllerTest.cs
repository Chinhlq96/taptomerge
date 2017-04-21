using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockControllerTest : MonoBehaviour {

	private BlockController block;
	private Vector3[] listPosition;


	void Start ()
	{
		block = GetComponent<BlockController> ();
		listPosition = new Vector3[10];
		InitListPosition ();
		block.Move (listPosition);
	}

	void InitListPosition ()
	{

		listPosition [0] = new Vector3 (0f, 0f, 0f);
		listPosition [1] = new Vector3 (1f, 0f, 0f);
		listPosition [2] = new Vector3 (1f, 1f, 0f);
		listPosition [3] = new Vector3 (2f, 1f, 0f);
	}
}
