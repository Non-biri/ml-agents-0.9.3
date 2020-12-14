using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PersonlityAgent : Agent
{
    public GameObject CrossWall;

    public bool useVectorObs;
    RayPerception rayPer;
    Rigidbody agentRB;
    PersonlityAcademy academy;





    public override void InitializeAgent()
    {
        base.InitializeAgent();
        academy = FindObjectOfType<PersonlityAcademy>();
        rayPer = GetComponent<RayPerception>();
        agentRB = GetComponent<Rigidbody>();
        // groundRenderer = ground.GetComponent<Renderer>();
        // groundMaterial = groundRenderer.material;
    }


    public override void CollectObservations()
    {
        base.CollectObservations();

        if (useVectorObs)
        {
            float rayDistance = 12f;
            float[] rayAngles = { 20f, 60f, 90f, 120f, 160f };
            string[] detectableObjects = { "wall" };
            AddVectorObs(GetStepCount() / (float)agentParameters.maxStep);
            AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));

        }
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
        AddReward(-1f / agentParameters.maxStep);
        MoveAgent(vectorAction);
    }


    public override void AgentReset()
    {
        base.AgentReset();
    }
}
