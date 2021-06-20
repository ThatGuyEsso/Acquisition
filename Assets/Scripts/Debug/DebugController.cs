using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
public class DebugController : MonoBehaviour { 
    private bool showConsole;
    private Controls inputAction;


    private string input = string.Empty;


    public static DebugCommand KILL_PLAYER;
    public static DebugCommand KILL_BOSS;
    public List<object> commandList;
    private void Awake()
    {
        inputAction = new Controls();
        inputAction.Console.Enable();
        inputAction.Console.ToggleConsole.performed += ToggleConsole;
        inputAction.Console.Return.performed += OnReturn;

        KILL_PLAYER = new DebugCommand("/Kill_Player", "Kills player if they exist","/Kill_Player",() => KillPlayer());
        KILL_BOSS = new DebugCommand("/Kill_Boss", "Kills active boss", "Kill_Boss", () => KillBoss());

       commandList = new List<object>
        {
            KILL_PLAYER,
            KILL_BOSS

        };
    }

    public void ToggleConsole(InputAction.CallbackContext context)
    {

        if(context.performed) showConsole = !showConsole;
        if (showConsole)
        {
           
        }
        else
        {
            
        }
    }

    private void OnGUI()
    {
        if (!showConsole) return;

        float y=0;
        GUI.Box(new Rect(0, y, Screen.width, 40), "");
        GUI.backgroundColor = new Color(1.0f, 1.0f, 1.0f, 0.6f);
        GUI.skin.textField.fontSize = 40;
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 60f), input);
    }

    public void OnReturn(InputAction.CallbackContext context)
    {
        if (context.performed&&showConsole)
        {
            HandleInput();
            input = string.Empty;
            showConsole = false;


        }

    }
    public void HandleInput()
    {
       
        for(int i=0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if (input.Contains(commandBase.CommandID))
            {
                if(commandList[i] as DebugCommand != null)
                {
                    (commandList[i] as DebugCommand).CallCommand();
                }
            }
        }
    }

    public void KillPlayer()
    {
        PlayerHealth player;
        if((player=FindObjectOfType<PlayerHealth>())!= false)
        {
            player.KillPlayer();
        }
    }
    public void KillBoss()
    {
        if (BossRoomManager.instance)
        {
            BaseBossAI boss = BossRoomManager.instance.GetBoss();
            if (boss) boss.KillBoss();
        }
    }

}
