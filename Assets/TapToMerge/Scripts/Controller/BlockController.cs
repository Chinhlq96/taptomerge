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

	public void ActiveBlock ()
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

	public void Move (Vector3[] preBlockPos) {
		transform.DOPath (preBlockPos, 0.2f).OnComplete(()=>{DestroyBlock();});
	}

	public void DestroyBlock () {
		Destroy (gameObject);
	}

	public void OnMouseDown() 
	{
		isTapped = true;
		this.PostEvent (EventID.BlockTap, this);
	}

	public void MergeEffect ()
	{
		if (Merged) 
		{
			boxMergeEffect.SetActive (true);
		}
	}
}
