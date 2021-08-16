using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour, IInitialisable
{
    public  TutorialData tutorialdata;
    public static TutorialManager instance;
    public static bool InTutorial;
    [SerializeField] private List<BaseTutorial> tutorials;

    public BaseTutorial movementTutorial;
    public BaseTutorial aimTutorial;
    public BaseTutorial dodgeTutorial;


    [SerializeField] private GameObject bowTutorialPrefab,swordTutorialPrefab,staffTutorialPrefab;
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


    public void ShowWeaponTutorial(WeaponType weapon)
    {
        switch (weapon)
        {
            case WeaponType.Sword:
                ShowSwordTutorial();
                break;
            case WeaponType.Bow:
                ShowBowTutorial();
                break;
            case WeaponType.Staff:
                ShowStaffutorial();
                break;
    
        }
    }


    public void ShowSwordTutorial()
    {
        if (swordTutorialPrefab)
        {
            GameObject tutorialObject = ObjectPoolManager.Spawn(swordTutorialPrefab, transform.position);
            tutorialObject.transform.SetParent(transform);

            if (tutorialObject)
            {
                BaseTutorial[] weaponTutorials = tutorialObject.GetComponentsInChildren<BaseTutorial>();

                foreach(BaseTutorial tut in weaponTutorials)
                {
                    if (tut) tut.InitTutorial();
                }
            }
        }
    }

    public void ShowBowTutorial()
    {

        if (bowTutorialPrefab)
        {
            GameObject tutorialObject = ObjectPoolManager.Spawn(bowTutorialPrefab, transform.position);
            tutorialObject.transform.SetParent(transform);
        

            if (tutorialObject)
            {
                BaseTutorial[] weaponTutorials = tutorialObject.GetComponentsInChildren<BaseTutorial>();

                foreach (BaseTutorial tut in weaponTutorials)
                {
                    if (tut) tut.InitTutorial();
                }
            }
        }
    }
    public void ShowStaffutorial()
    {
        if (staffTutorialPrefab)
        {
            GameObject tutorialObject = ObjectPoolManager.Spawn(staffTutorialPrefab, transform.position);
            tutorialObject.transform.SetParent(transform);

            if (tutorialObject)
            {
                BaseTutorial[] weaponTutorials = tutorialObject.GetComponentsInChildren<BaseTutorial>();

                foreach (BaseTutorial tut in weaponTutorials)
                {
                    if (tut) tut.InitTutorial();
                }
            }
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
