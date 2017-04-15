using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventManager;

public class BlockTest : MonoBehaviour {
	private static BlockTest _instance;

	public int x;
	public int y;
	public int value;
	public bool isTapped;
	public bool isActivated;

	public static BlockTest Instance { get; private set; }

	void Awake()
	{
		Instance = this;
	}

	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}

	void OnMouseDown() 
	{
		isTapped = true;
		this.PostEvent (EventID.BlockTap);
		//Debug.Log (1);
	}
		
}
