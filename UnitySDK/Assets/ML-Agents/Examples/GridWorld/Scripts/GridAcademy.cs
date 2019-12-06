using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MLAgents;

//リポジトリ同期のテスト

public class GridAcademy : Academy
{
    // 各GameObject型の変数は、宣言だけして中身はUnityエディタ上で設定できる
    // これによりソース上では任意のGameObjectは宣言するだけで使えるようになる

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
    ////////////////////////////////////////////////////////////////////////////////////
    public GameObject sWallPref;
    public GameObject ExReword;
    ////////////////////////////////////////////////////////////////////////////////////
    GameObject[] objects;

    GameObject plane;
    GameObject sN;
    GameObject sS;
    GameObject sE;
    GameObject sW;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ///迷路のマッピング処理
    ///agentPref：0, goalPref：1, pitPref：2, WallPref：3, ExReword：4
    public int[,] mapping = new int[10, 10] { {0,3,2,9,9,3,9,9,9,4},
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
        //gridSizeは入力値ではなく10に固定
        //gridSize = (int)resetParameters["gridSize"];
        gridSize = 10;

        cam = camObject.GetComponent<Camera>();

        //objectsの種類として登録。agentPref：0, goalPref：1, pitPref：2, WallPref：3, ExReword：4で指定する
        objects = new GameObject[5] { agentPref, goalPref, pitPref, sWallPref, ExReword };

        agentCam = GameObject.Find("agentCam").GetComponent<Camera>();

        //×＋などの全てのオブジェクトを管理する
        actorObjs = new List<GameObject>();

        //地面と壁
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

        //障害物やゴールなどの要素をまずリスト化
        //各要素の数をリスト化した後int型の配列playersにコピーし処理しやすくする
        List<int> playersList = new List<int>();

        //配列mappingの内容をplayersListに入れることで改造する部分を可能な限り減らす
        for (int i = 0; i < mapping.GetLength(0); i++)
        {
            for(int j =0; i< mapping.GetLength(1); j++)
            {
                playersList.Add(mapping[i,j]);
            }
        }

        //各値に従い、〇＃×＋の配置数をリスト化する
        ////////////////////////////////////////////////////////////////////////////////////
        //
        /*
        for (int i = 0; i < (int)resetParameters["numExReword"]; i++)
        {
            playersList.Add(4);
        }

        for (int i = 0; i < (int)resetParameters["numWalls"]; i++)
        {
            playersList.Add(3);
        }
        ////////////////////////////////////////////////////////////////////////////////////
        
        for (int i = 0; i < (int)resetParameters["numObstacles"]; i++)
        {
            playersList.Add(2);
        }

        for (int i = 0; i < (int)resetParameters["numGoals"]; i++)
        {
            playersList.Add(1);
        }
 
        //リストを配列化
        players = playersList.ToArray();

        //5/10=0.5m,10/10=1m,つまりは1マス0.1m.
        plane.transform.localScale = new Vector3(gridSize / 10.0f, 1f, gridSize / 10.0f);
        //(5-1)/2=4,1マスの範囲は0.05～-0.05であり、それの中央にエージェントは置かれる。それに合わせるために間を取る
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
    /// actorObjsの配列を初期化し、エージェントなどの要素を削除
    /// その後SetEnvironmentを呼び出し環境を再構築する
    /// 
    /// Initialize actorObjs array and delete elements such as agents.
    /// Then call SetEnvironment to rebuild the environment.
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        ///各GameObjectをランダムな位置に配置するための処理

        HashSet<int> numbers = new HashSet<int>();

        //numbersの数がplayers＋1より大きくなるまで
        while (numbers.Count < players.Length + 1)
        {
            //0～gridSize^2の範囲でランダムな値をセット
            numbers.Add(Random.Range(0, gridSize * gridSize));
        }
        //int配列にコピー
        int[] numbersA = Enumerable.ToArray(numbers);
        
        for (int i = 0; i < players.Length; i++)
        {
            //gridSizeが5でnumbersA[i]が8なら[1,3]の位置に配置される
            //注意！ここでのyは画像処理での2Dの見方に従っている。Unity上ではyはz軸に代入する
            int x = (numbersA[i]) / gridSize;
            int y = (numbersA[i]) % gridSize;
            //インスタンス化
            GameObject actorObj = Instantiate(objects[players[i]]);
            actorObj.transform.position = new Vector3(x, -0.25f, y);
            actorObjs.Add(actorObj);
        }

        //環境の範囲に入るように0.05ずらす処理
        int x_a = (numbersA[players.Length]) / gridSize;
        int y_a = (numbersA[players.Length]) % gridSize;
        trueAgent.transform.position = new Vector3(x_a, -0.25f, y_a);

        ////////////////////////////////////////////////////////////////////////////////////////////////////

    }

    public override void AcademyStep()
    {

    }
}
