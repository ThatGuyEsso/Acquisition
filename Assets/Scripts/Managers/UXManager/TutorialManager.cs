using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour, IInitialisable
{
    public  TutorialData tutorialdata;
    public static TutorialManager instance;

    [SerializeField] private List<BaseTutorial> tutorials;

    public BaseTutorial movementTutorial;
    public BaseTutorial aimTutorial;
    public BaseTutorial dodgeTutorial;
    public void Init()
    {
        if (!tutorialdata) Destroy(gameObject);
        if(tutorialdata.isTutorialComplete) Destroy(gameObject);
        if (instance == false)
        {
            instance = this;
            SetUpTutorials();
            BindToGameStateManager();
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    public void BindToGameStateManager()
    {

        if (GameManager.instance) GameManager.instance.OnNewEvent += EvaluateNewEvent;
    }

    public void SetUpTutorials()
    {
        BaseTutorial[] tuts = GetComponentsInChildren<BaseTutorial>();

        if (tuts.Length <= 0) return;
        
        for (int i = 0; i < tuts.Length; i++)
        {
            if (tuts[i])
                tutorials.Add(tuts[i]);
        }
     
        foreach(BaseTutorial tutorial in tutorials)
        {
            if (tutorial) tutorial.gameObject.SetActive(false);
        }
    }



    public void EvaluateNewEvent(GameEvents newEvent)
    {
        switch (newEvent)
        {
            case GameEvents.PlayerSpawned:
                if (!tutorialdata.isMoveTutorialComplete)
                {
                    movementTutorial.gameObject.SetActive(true);
                    movementTutorial.InitTutorial();
                    if (!tutorialdata.isAimTutorialComplete)
                    {
                        Invoke("ShowAimTutorial", 1f);
                    }
                }
                break;
      
        }
    }




    public void ShowAimTutorial()
    {
        aimTutorial.gameObject.SetActive(true);
        aimTutorial.InitTutorial();
    }


    public void ShowDodgeTutorial()
    {
        dodgeTutorial.gameObject.SetActive(true);
        dodgeTutorial.InitTutorial();
    }
    private void OnDisable()
    {
        
        if (GameManager.instance) GameManager.instance.OnNewEvent -= EvaluateNewEvent;
    }

    private void OnDestroy()
    {
        if (GameManager.instance) GameManager.instance.OnNewEvent -= EvaluateNewEvent;
    }
}
