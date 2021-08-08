using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTutorial : BaseTutorial
{

    protected Controls inputs;
    protected void Awake()
    {
        inputs = new Controls();
    }



    protected void OnDisable()
    {
        if (inputs != null) inputs.Disable();
    }


}
