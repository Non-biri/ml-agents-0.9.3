using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PersonlityAgent : Agent
{
    public bool useVectorObs;
    Rigidbody agentRB;
    PersonlityAcademy academy;




    public override void InitializeAgent()
    {
        base.InitializeAgent();
    }


    public override void CollectObservations()
    {
        if (useVectorObs)
        {
            float reyDistanse = 12f;
            float[] reyAngles = { 20f, 60f, 90f, 120f, 160f };
            string[] detectableObjects = { "Wall" };

        }
        base.CollectObservations();
    }


    public void MoveAgent(float[] act)
    {

        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        if (brain.brainParameters.vectorActionSpaceType == SpaceType.continuous)
        {
            dirToGo = transform.forward * Mathf.Clamp(act[0], -1f, 1f);
            rotateDir = transform.up * Mathf.Clamp(act[1], -1f, 1f);
        }
        else
        {
            int action = Mathf.FloorToInt(act[0]);
            switch (action)
            {
                case 1:
                    dirToGo = transform.forward * 1f;
                    break;
                case 2:
                    dirToGo = transform.forward * -1f;
                    break;
                case 3:
                    rotateDir = transform.up * 1f;
                    break;
                case 4:
                    rotateDir = transform.up * -1f;
                    break;
            }
        }
        transform.Rotate(rotateDir, Time.deltaTime * 150f);
        agentRB.AddForce(dirToGo * academy.agentRunSpeed, ForceMode.VelocityChange);
    }


    public override void AgentAction(float[] vectorAction, string textAction)
    {
        base.AgentAction(vectorAction, textAction);
    }


    public override void AgentReset()
    {
        base.AgentReset();
    }
}
