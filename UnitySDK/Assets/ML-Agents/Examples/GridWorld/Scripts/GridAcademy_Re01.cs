using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MLAgents;

//リポジトリ同期のテスト

public class GridAcademy_Re01 : Academy
{
    [HideInInspector]
    public List<GameObject> actorObjs;
    [HideInInspector]
    public int[] players;

    public GameObject trueAgent;

    public int gridSize;

    public GameObject camObject;
    Camera cam;
    Camera agentCam;

    public GameObject agentPref;
    public GameObject goalPref;
    public GameObject pitPref;
    public GameObject sWallPref;
    public GameObject ExReword;

    GameObject[] objects;

    GameObject plane;
    GameObject sN;
    GameObject sS;
    GameObject sE;
    GameObject sW;

    ///迷路のマッピング処理
    ///agentPref：0, goalPref：1, pitPref：2, WallPref：3, ExReword：4
    public int[,] mapping = new int[10, 10] { {9,3,2,9,9,3,9,9,9,4},
                                            {9,3,3,3,9,3,9,3,3,3},
                                            {9,9,9,3,9,3,9,9,3,2},
                                            {3,3,9,3,9,3,3,9,3,9},
                                            {3,9,9,9,9,9,9,9,9,9},
                                            {9,9,3,3,3,9,3,3,3,9},
                                            {9,3,3,4,3,9,9,3,4,9},
                                            {9,9,3,9,3,3,9,9,9,3},
                                            {3,9,3,9,3,9,9,3,9,3},
                                            {3,9,9,9,3,2,3,3,9,1}};


    public override void InitializeAcademy()
    {

        gridSize = 10;

        cam = camObject.GetComponent<Camera>();

        objects = new GameObject[5] { agentPref, goalPref, pitPref, sWallPref, ExReword };

        agentCam = GameObject.Find("agentCam").GetComponent<Camera>();

        actorObjs = new List<GameObject>();

        plane = GameObject.Find("Plane");
        sN = GameObject.Find("sN");
        sS = GameObject.Find("sS");
        sW = GameObject.Find("sW");
        sE = GameObject.Find("sE");
    }

    /// <summary>
    /// 環境の構築
    /// </summary>
    public void SetEnvironment()
    {
        cam.transform.position = new Vector3(-((int)resetParameters["gridSize"] - 1) / 2f,
                                             (int)resetParameters["gridSize"] * 1.25f,
                                             -((int)resetParameters["gridSize"] - 1) / 2f);
        cam.orthographicSize = ((int)resetParameters["gridSize"] + 5f) / 2f;

        List<int> playersList = new List<int>();

        for (int i = 0; i < mapping.GetLength(0); i++)
        {
            for(int j =0; j< mapping.GetLength(1); j++)
            {
                playersList.Add(mapping[i,j]);
            }
        }

        //リストを一次元配列化
        players = playersList.ToArray();

        plane.transform.localScale = new Vector3(gridSize / 10.0f, 1f, gridSize / 10.0f);
        plane.transform.position = new Vector3((gridSize - 1) / 2f, -0.5f, (gridSize - 1) / 2f);

        //北南壁
        sN.transform.localScale = new Vector3(1, 1, gridSize + 2);
        sS.transform.localScale = new Vector3(1, 1, gridSize + 2);
        sN.transform.position = new Vector3((gridSize - 1) / 2f, 0.0f, gridSize);
        sS.transform.position = new Vector3((gridSize - 1) / 2f, 0.0f, -1);

        //東西壁
        sE.transform.localScale = new Vector3(1, 1, gridSize + 2);
        sW.transform.localScale = new Vector3(1, 1, gridSize + 2);
        sE.transform.position = new Vector3(gridSize, 0.0f, (gridSize - 1) / 2f);
        sW.transform.position = new Vector3(-1, 0.0f, (gridSize - 1) / 2f);

        //環境の広さに合わせたカメラの調整
        agentCam.orthographicSize = (gridSize) / 2f;
        agentCam.transform.position = new Vector3((gridSize - 1) / 2f, gridSize + 1f, (gridSize - 1) / 2f);

    }
    
    /// <summary>
    /// 要素の配置処理
    /// </summary>
    public override void AcademyReset()
    {
        //c#版拡張for
        foreach (GameObject actor in actorObjs)
        {
            DestroyImmediate(actor);
        }
        SetEnvironment();

        actorObjs.Clear();

        for (int i = 0; i < players.Length; i++)
        {

            int x = i / gridSize;
            int y = i % gridSize;
            
            if (players[i] != 9)
            {
                GameObject actorObj = Instantiate(objects[players[i]]);
                actorObj.transform.position = new Vector3(x, -0.25f, y);
                actorObjs.Add(actorObj);
            }
        }

        int x_a = players.Length / gridSize;
        int y_a = players.Length % gridSize;
        trueAgent.transform.position = new Vector3(x_a, -0.25f, y_a);

    }

    public override void AcademyStep()
    {

    }
}
