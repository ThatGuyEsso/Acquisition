using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCamBoundsTrigger : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D roomCamBounds;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            UseRoomBounds();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            RemoveBounds();
        }
    }



    public void UseRoomBounds()
    {
        if (CamShake.instance && roomCamBounds)
        {
            CamShake.instance.bounds.m_BoundingShape2D = roomCamBounds;

        }
        else
        {
            RemoveBounds();
        }

    }

    public void RemoveBounds()
    {
        if (CamShake.instance)
        {
            CamShake.instance.bounds.m_BoundingShape2D = null;
        }

    }
}
