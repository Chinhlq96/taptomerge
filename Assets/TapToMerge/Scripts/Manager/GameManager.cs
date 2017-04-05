using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EventManager;

public class GameManager : MonoBehaviour 
{
    [SerializeField]
    private GameObject imageObject;

    GameManager gameMgr;  

    void OnEnable()
    {
        this.RegisterListener(EventID.PostScore, (sender, param) => UpdateScore((int)param));
    }

    void OnDisable()
    {
        this.RemoveListener(EventID.PostScore, (sender, param) => UpdateScore((int)param)); 
    }

    int currentScore;
    void UpdateScore(int score)
    {
        currentScore += score;
        Debug.Log(currentScore);  
    }



    public void MoveImage()
    {
        this.PostEvent(EventID.PostScore, 10);
//        imageObject.transform.DOLocalMoveY(0, .7f).SetEase(Ease.OutBack).OnComplete(
//        
//            () =>
//            {
//                Debug.Log("Completed");
//            }
//        
//        );
    }


}
