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
	[SerializeField]	private Sprite rightNowForm;
	public float offset;
	public bool isActivated;
	public bool isTapped;

	public bool Merged;
	public GameObject boxMergeEffect;



	public static BlockController Instance { get; private set; }

	void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start () {

		isActivated = false;
		offset = 0.07f;
		Merged = false;
	}

	void Update()
	{
		ActiveBlock ();
		//MergeEffect ();
	}

	void ActiveBlock ()
	{
		rightNowForm = GetComponent<SpriteRenderer> ().sprite;
		if (isActivated) 
		{
			GetComponent<SpriteRenderer>().sprite = activeForm;
			if (rightNowForm != GetComponent<SpriteRenderer> ().sprite) 
			{
				transform.position = new Vector2 (transform.position.x, transform.position.y + offset);
			}
		}
		else
		{
			GetComponent<SpriteRenderer>().sprite = normalForm;
			if (rightNowForm != GetComponent<SpriteRenderer> ().sprite) 
			{
				transform.position = new Vector2 (transform.position.x, transform.position.y - offset);
			}
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
		this.PostEvent (EventID.BlockTap, this);
	}

	void MergeEffect ()
	{
		if (Merged) 
		{
			boxMergeEffect.SetActive (true);
			Merged = false;
		}
	}
}
