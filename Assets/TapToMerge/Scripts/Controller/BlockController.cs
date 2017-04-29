using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EventManager;
using System;

public class BlockController : MonoBehaviour
{
    public int value;
    public int x;
    public int y;
  
    public float offset;

    [SerializeField]
    GameObject selectedObject;


    // Use this for initialization
    void OnEnable()
    {
        isActivated = false;
        offset = 0.07f;
    }

    public void Init(int x, int y, int value)
    {
        this.x = x;
        this.y = y;
        this.value = value;
        
        gameObject.name = x + "" + y;
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


    void ActiveBlock(bool isActivated)
    {
        selectedObject.SetActive(isActivated);
    }

    public void Drop(int x, int y, bool delay = false)
    {
        var newPos = GameController.Instance.ConvertBoardToPosition(x, y);
        float distance = Vector3.Distance(transform.position, newPos);
        float timeDelay = delay ? (x + y * 5) * 0.05f : 0f;
        transform.DOMoveY(newPos.y, distance / 100f).SetDelay(timeDelay);

    }

    public void Move(Vector3[] preBlockPos, Action action = null)
    {
        transform.DOPath(preBlockPos, .2f).OnComplete(() =>
        {
           
            if (action != null)
            {
                action.Invoke();
            }
            DestroyBlock();

        });
    }

    public void DestroyBlock()
    {
        ContentMgr.Instance.Despaw(gameObject);
    }

    public void OnMouseDown()
    {
        //if (GameController.Instance.isMerging) return;
        this.PostEvent(EventID.BlockTap, this);
    }
		
}
