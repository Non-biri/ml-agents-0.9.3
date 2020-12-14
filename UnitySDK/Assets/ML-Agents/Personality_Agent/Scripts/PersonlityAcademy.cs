using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PersonlityAcademy : Academy
{

    public float agentRunSpeed;
    public float agentRotationSpeed;
    public float gravityMultiplier;

    public override void InitializeAcademy()
    {
        base.InitializeAcademy();
        Physics.gravity *= gravityMultiplier;
    }

    public override void AcademyReset()
    {
        base.AcademyReset();
    }

    public override void AcademyStep()
    {
        base.AcademyStep();
    }


    public void SetEnvironment()
    {

    }

}
