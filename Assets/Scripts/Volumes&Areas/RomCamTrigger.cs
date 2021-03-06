using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;
public class RomCamTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera roomCamera;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SwitchToRoomCamera();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SwitchToPlayerCamera();
        }
    }



    public void SwitchToRoomCamera()
    {
        if (CamShake.instance&& roomCamera)
        {
            roomCamera.gameObject.SetActive(true);
            CamShake.instance.DisableDefault();
            CamShake.instance.SetShakeCam(roomCamera);
        }
  
    }

    public void SwitchToPlayerCamera()
    {
        if (CamShake.instance&& roomCamera)
        {
            CamShake.instance.UseDefault();
            roomCamera.gameObject.SetActive(false);
        }

    }
}
