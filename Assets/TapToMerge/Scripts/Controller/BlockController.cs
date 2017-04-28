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

	public float offset;

    SpriteRenderer _spriteRenderer;
    SpriteRenderer spriteRenderer 
    {
        get{ 
            if(_spriteRenderer==null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
            return _spriteRenderer;
        }
    }
    

    public int sortingOrder
    {
        get { return spriteRenderer.sortingOrder; }
        set { spriteRenderer.sortingOrder = value; }
    }
    

	// Use this for initialization
	void OnEnable () {
        isActivated = false;
		offset = 0.07f;
	}

    public void Init(int x,int y,int value, int sortingOrder)
    {
        this.x = x;
        this.y = y;
        this.value = value;
        this.sortingOrder = sortingOrder;
        gameObject.name =x + "" + y;
    }
    public void Init(int x,int y, int sortingOrder)
    {
        this.x = x;
        this.y = y;
        this.sortingOrder = sortingOrder;
        gameObject.name =x + "" + y;
    }

    private bool _isActived;

    public bool isActivated
    {
        get { return _isActived; }
        set
        { 
                _isActived = value;
                ActiveBlock(value);
        }
    }
    

	void ActiveBlock (bool isActivated)
	{
        var newPos = GameController.Instance.ConvertBoardToPosition (x, y);

        spriteRenderer.sprite = isActivated ?activeForm: normalForm;
        if (isActivated)
        {
            newPos = new Vector2(newPos.x,newPos.y+offset); 
        }

        transform.position = newPos;
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
                if (action!=null)
                {
                    action.Invoke();
                }
            DestroyBlock(); 
            
        });
	}

	public void DestroyBlock () {
        //Destroy(gameObject);
        ContentMgr.Instance.Despaw (gameObject);
	}

	public void OnMouseDown() 
	{
        this.PostEvent (EventID.BlockTap, this);
       // GameController.Instance.CheckTap(new Vector2(x,y));
	}
        
}
