using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicConeCollider : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask ViewBlockingLayers;
    [SerializeField] public float offset;
    [SerializeField] private int nPoints = 20;

    private PolygonCollider2D polyCol;

    private void Awake() {


        polyCol = GetComponent<PolygonCollider2D>();
    
    }

    private void OnEnable()
    {
        if(polyCol)
            polyCol = GetComponent<PolygonCollider2D>();
    }




    //Update light shape
    public void SetColliderShape(Vector2 aimDir,float radius, float maxAngle, Vector2 origin)
    {
        //polyCol.points = new Vector2[nPoints];
        Vector2[] points = new Vector2[nPoints];
        float startingAngle = (EssoUtility.GetAngleFromVector(aimDir) - maxAngle / 2);
   
        float currentAngle = startingAngle + offset;
        float angleIncrease = maxAngle / nPoints;

        for (int i = 0; i < points.Length; i++)
        {

            if (i > 0)
            {
                RaycastHit2D hitInfo = Physics2D.Raycast(origin, EssoUtility.GetVectorFromAngle(currentAngle), radius, ViewBlockingLayers);
                if (hitInfo)
                {
                    points[i] = hitInfo.point;
                    Debug.DrawLine(polyCol.points[0], hitInfo.point);
                }
                else
                {
                  
                    points[i] = origin + (Vector2) EssoUtility.GetVectorFromAngle(currentAngle) * radius;
                    Debug.DrawRay(points[0], points[0] + (Vector2)EssoUtility.GetVectorFromAngle(currentAngle) * radius);
                }


            }
            else
            {
                points[i] = origin;
            }


            currentAngle -= angleIncrease;
        }
       
        polyCol.SetPath(0 , points);
    }
}
