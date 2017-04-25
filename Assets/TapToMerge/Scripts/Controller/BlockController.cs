using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EventManager;

public class BlockController : MonoBehaviour{

	//[SerializeField] 	private int ID;
	public int value;
	public int x;
	public int y;
	[SerializeField]	private Sprite normalForm;
	[SerializeField] 	private Sprite activeForm;
	[SerializeField]	private Sprite rightNowForm;
	private float offset;
	public bool isActivated;
	public bool isTapped;

	public bool Merged;
	public GameObject boxMergeEffect;


	// Use this for initialization
	void Start () {

		isActivated = false;
		offset = GameController.Instance.offset;
		Merged = false;
	}

	void Update()
	{
		ActiveBlock ();
		MergeEffect ();
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
		if (GameController.Instance.isMerging)
			return;
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
