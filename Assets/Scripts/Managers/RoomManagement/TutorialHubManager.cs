using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHubManager : MonoBehaviour
{
    public static TutorialHubManager instance;
    public LevelRoom tutorialRoom;
    [SerializeField] private RoomDoor entranceDoor;
    [SerializeField] private HubManager hubManager;
    public Turret tutorialTurret;
    private bool isTriggered;

    private void Awake()
    {
        if (tutorialTurret.gameObject.activeInHierarchy) tutorialTurret.gameObject.SetActive(false);
    }
    public void Init()
    {
        if (instance == false)
        {
            instance = this;
      
            entranceDoor.Init();
            TutorialManager.InTutorial = true;



        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        if (tutorialTurret) tutorialTurret.OnTurretDestroy -= StartGame;
        if (hubManager) hubManager.SetUpBossDoors();
        TutorialManager.InTutorial = false;

    }
    public void StartTutorial()
    {
        if (isTriggered) return;
        if (RoomManager.instance)
        {
            List<string> roomsToKeep = new List<string>();
            roomsToKeep.Add(tutorialRoom.ID());

            RoomManager.instance.ClearAllRoomNotInSet(roomsToKeep);
        }
        entranceDoor.ToggleLock(true);
        tutorialTurret.gameObject.gameObject.SetActive(true);
        tutorialTurret.InitTurret();
        if (!TutorialManager.instance.tutorialdata.isDodgeTutorialComplete)
        {
            tutorialTurret.SetCanBeHurt(false);
            Invoke("ShowDodgeTutorial", 3f);
            TutorialManager.instance.dodgeTutorial.OnTutorialComplete += BeginWeaponTutorial;
        }
        else
        {
            EvaluateWeaponTutorial();
        }
        tutorialTurret.OnTurretDestroy += StartGame;
        isTriggered = true;
    }
    public void ShowDodgeTutorial()
    {
        TutorialManager.instance.ShowDodgeTutorial();
    }
    public void BeginWeaponTutorial()
    {
        if (TutorialManager.instance.dodgeTutorial) TutorialManager.instance.dodgeTutorial.OnTutorialComplete -= BeginWeaponTutorial;
        if (tutorialTurret) tutorialTurret.SetCanBeHurt(true);
        EvaluateWeaponTutorial();
        hubManager.Init();
    }
    public void EvaluateWeaponTutorial()
    {
      

        if (WeaponManager.instance&&TutorialManager.instance)
        {
            if ((WeaponManager.instance.equippedWeapon.GetWeaponType() != WeaponType.none)){
                TutorialManager.instance.ShowWeaponTutorial(WeaponManager.instance.equippedWeapon.GetWeaponType());
            }
            
        }
    }

}
