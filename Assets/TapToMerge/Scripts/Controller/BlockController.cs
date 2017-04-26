using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EventManager;
using System;

public class BlockController : MonoBehaviour{


	public int value;
	public int x;
	public int y;
	[SerializeField]	private Sprite normalForm;
	[SerializeField] 	private Sprite activeForm;
	[SerializeField]	private Sprite rightNowForm;
	public float offset;

	public bool isTapped;

	public bool Merged;
	public GameObject boxMergeEffect;


    public int sortingOrder
    {
        get { return GetComponent<Renderer>().sortingOrder; }
        set { GetComponent<Renderer>().sortingOrder = value; }
    }
    

	// Use this for initialization
	void OnEnable () {
        isActivated = false;
		offset = 0.07f;
		Merged = false;
	}


    private bool _isActived;

    public bool isActivated
    {
        get { return _isActived; }
        set { _isActived = value; ActiveBlock(value); }
    }
    

	public void ActiveBlock (bool isActivated)
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

	public void Drop(int x,int y,bool delay = false)
	{
		var newPos = GameController.Instance.ConvertBoardToPosition (x, y);
		float distance = Vector3.Distance (transform.position, newPos);
		float timeDelay = delay ? (x+y*5)*0.05f:0f;
		transform.DOMoveY (newPos.y, distance / 25f).SetDelay(timeDelay);

	}

	public void Move (Vector3[] preBlockPos,Action action=null) {
        transform.DOPath(preBlockPos, 0.2f).OnComplete(() =>
        {
            DestroyBlock(); 
            if (action!=null)
            {
                action.Invoke();
            }
        });
	}

	public void DestroyBlock () {
		ContentMgr.Instance.Despaw (gameObject);
	}

	public void OnMouseDown() 
	{
		isTapped = true;
		if (this==null) {
			Debug.Log ("null");
		}
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
