using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EventManager;

public class BlockController : MonoBehaviour{

	//[SerializeField] 	private int ID;
	private static BlockController _instance;
	public int value;
	public int x;
	public int y;
	[SerializeField]	private Sprite normalForm;
	[SerializeField] 	private Sprite activeForm;
	public bool isActivated;
	public bool isTapped;

	public static BlockController Instance { get; private set; }

	void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start () {

		isActivated = true;
	}

	void Update()
	{
		ActiveBlock ();
	}

	void ActiveBlock ()
	{
		if (isActivated) 
		{
			GetComponent<SpriteRenderer>().sprite = activeForm;
		}
		else
		{
			GetComponent<SpriteRenderer>().sprite = normalForm;
		}
	}

	void Move (Vector3[] preBlockPos) {
		for (int i = 0; i < preBlockPos.Length; i++) 
		{
			transform.DOMove (preBlockPos[i],0.5f);
		}
		DestroyBlock ();
	}

	void DestroyBlock () {
		Destroy (gameObject);
	}

	void OnMouseDown() 
	{
		isTapped = true;
		this.PostEvent (EventID.BlockTap);
	}
}
