using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    Scene sceneLoaded;
    public static LevelManager instance;

    [SerializeField] GameObject pivoteStart;
    [SerializeField] GameObject pivoteBrick;
    [SerializeField] GameObject pivoteWall;
    [SerializeField] GameObject pivoteUnBrick;
    [SerializeField] GameObject pivotePush;
    [SerializeField] GameObject pivoteDiamond;
    [SerializeField] GameObject winArea;

    GameObject player;
    GameObject levelGroup;
    List<ParticleSystem> fireWorks;
    GameObject chest;
    int[,] map2D;

    const int diamondPerGet = 10;
    const int numberScene = 4;
    const float offsetStartPos = 3;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        levelGroup = GameObject.Find("Level");
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
        Oninit();
        CreateLevel();
    }

    public void Oninit()
    {
        sceneLoaded = SceneManager.GetActiveScene();
    }

    public int DiamondPoint()
    {
        return UnityEngine.Random.Range(diamondPerGet, diamondPerGet + 10) * 10;
    }

    public void Win()
    {
        player.transform.position = new Vector3(chest.transform.position.x, player.transform.position.y, chest.transform.position.z) - chest.transform.up;

        for (int i = 0; i < fireWorks.Count; i++)
        {
            fireWorks[i].Play();
        }
        Invoke(nameof(WinUI), 2f);
    }

    public void Lose()
    {
        Invoke(nameof(LoseUI), 1f);
    }

    public void ContinueGame()
    {
        player.GetComponent<Player>().ContinuePlayer();
        IdleUI();
    }

    public void NextLevel()
    {
        if (sceneLoaded.buildIndex < numberScene)
        {
            SceneManager.LoadScene(sceneLoaded.buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(sceneLoaded.buildIndex);
    }

    void CreateLevel()
    {
        string[] mapData = ReadLevelText(SceneManager.GetActiveScene().name);

        int line = mapData.Length;
        int column = mapData[0].ToCharArray().Length;
        map2D = new int[line, column];

        for (int i = 0; i < line; i++)
        {
            char[] index = mapData[i].ToCharArray();
            for (int j = 0; j < column; j++)
            {
                if (index[j] != ' ') map2D[i, j] = Convert.ToInt32(index[j].ToString());
                else map2D[i, j] = -1;
            }
        }

        for (int i = 0; i < map2D.GetLength(0); i++)
        {
            for (int j = 0; j < map2D.GetLength(1); j++)
            {
                float rotate = 0;
                if (map2D[i, j] == (int)ConstrucIndex.WinPos)
                {
                    if (i < map2D.GetLength(0) -1)
                    {
                        // neu phia tren dung thi xoay 0 do
                        if (map2D[i + 1, j] == (int)ConstrucIndex.PivoteUnBrick || map2D[i + 1, j] == (int)ConstrucIndex.PivoteBrick)
                        {
                            rotate = 0;
                        }
                    }
                    if (i > 0)
                    {
                        // neu phia duoi dung thi xoay 180 do
                        if (map2D[i - 1, j] == (int)ConstrucIndex.PivoteUnBrick || map2D[i - 1, j] == (int)ConstrucIndex.PivoteBrick)
                        {
                            rotate = 180;
                        }
                    }
                    if (j < map2D.GetLength(1) -1)
                    {
                        // neu ben phai dung thi xoay -90 do
                        if (map2D[i, j + 1] == (int)ConstrucIndex.PivoteUnBrick || map2D[i, j + 1] == (int)ConstrucIndex.PivoteBrick)
                        {
                            rotate = -90;
                        }
                    }
                    if(j > 0)
                    {
                        // neu ben trai dung thi xoay 90 do
                        if (map2D[i, j - 1] == (int)ConstrucIndex.PivoteUnBrick || map2D[i, j - 1] == (int)ConstrucIndex.PivoteBrick)
                        {
                            rotate = 90;
                        }
                    }
                }
                if (map2D[i, j] == (int)ConstrucIndex.PivotePush)
                {
                    if ((i < map2D.GetLength(0) - 1) && (j < map2D.GetLength(1) - 1))
                    {
                        // neu phia duoi va ben phai dung thi xoay -90 do
                        if (map2D[i + 1, j] == (int)ConstrucIndex.PivoteUnBrick || map2D[i + 1, j] == (int)ConstrucIndex.PivoteBrick)
                        {
                            if (map2D[i, j + 1] == (int)ConstrucIndex.PivoteUnBrick || map2D[i, j + 1] == (int)ConstrucIndex.PivoteBrick)
                            {
                                rotate = -90;
                            }
                        }
                    }

                    if ((i < map2D.GetLength(0) - 1) && (j > 0))
                    {
                        // neu phia duoi va ben trai dung thi xoay 0 do
                        if (map2D[i + 1, j] == (int)ConstrucIndex.PivoteUnBrick || map2D[i + 1, j] == (int)ConstrucIndex.PivoteBrick)
                        {
                            if (map2D[i, j - 1] == (int)ConstrucIndex.PivoteUnBrick || map2D[i, j - 1] == (int)ConstrucIndex.PivoteBrick)
                            {
                                rotate = 0;
                            }
                        }
                    }

                    if ((i > 0) && (j < map2D.GetLength(1) - 1))
                    {
                        // neu phia tren va ben phai dung thi xoay 180 do
                        if (map2D[i - 1, j] == (int)ConstrucIndex.PivoteUnBrick || map2D[i - 1, j] == (int)ConstrucIndex.PivoteBrick)
                        {
                            if (map2D[i, j + 1] == (int)ConstrucIndex.PivoteUnBrick || map2D[i, j + 1] == (int)ConstrucIndex.PivoteBrick)
                            {
                                rotate = 180;
                            }
                        }
                    }

                    if ((i > 0) && (j > 0))
                    {
                        // neu phia tren va ben trai dung thi xoay 90 do
                        if (map2D[i - 1, j] == (int)ConstrucIndex.PivoteUnBrick || map2D[i - 1, j] == (int)ConstrucIndex.PivoteBrick)
                        {
                            if (map2D[i, j - 1] == (int)ConstrucIndex.PivoteUnBrick || map2D[i, j - 1] == (int)ConstrucIndex.PivoteBrick)
                            {
                                rotate = 90;
                            }
                        }
                    }
                }
                SpawnConstruction(map2D[i, j], j, -i, rotate);
            }
        }
    }

    void SpawnConstruction(int constructionIndex, int xNum, int yNum, float rotate)
    {
        if (constructionIndex != -1)
        {
            GameObject tempConstruction = Instantiate(ConstructionType(constructionIndex), levelGroup.transform);

            tempConstruction.transform.localPosition = new Vector3(xNum, 0, yNum);
            tempConstruction.transform.localRotation = Quaternion.Euler(0, rotate, 0);

            if (constructionIndex == (int)ConstrucIndex.StartPos)
            {
                player.transform.position = tempConstruction.transform.position + new Vector3(0, offsetStartPos, 0);
            }
            else if (constructionIndex == (int)ConstrucIndex.WinPos)
            {
                WinArea tmp = tempConstruction.GetComponent<WinArea>();
                chest = tmp.chest;
                fireWorks = tmp.fireWorks;
            }
        }
    }

    string[] ReadLevelText(string levelName)
    {
        TextAsset tempData = Resources.Load("MapData/" + levelName) as TextAsset;
        string data = tempData.text.Replace(Environment.NewLine, string.Empty);
        return data.Split('-');
    }

    GameObject ConstructionType(int number)
    {
        GameObject spawnObj = null;
        switch (number)
        {
            case (int)ConstrucIndex.StartPos:
                spawnObj = pivoteStart;
                break;
            case (int)ConstrucIndex.PivoteBrick:
                spawnObj = pivoteBrick;
                break;
            case (int)ConstrucIndex.PivoteWall:
                spawnObj = pivoteWall;
                break;
            case (int)ConstrucIndex.PivoteUnBrick:
                spawnObj = pivoteUnBrick;
                break;
            case (int)ConstrucIndex.PivotePush:
                spawnObj = pivotePush;
                break;
            case (int)ConstrucIndex.WinPos:
                spawnObj = winArea;
                break;
            case (int)ConstrucIndex.PivoteDiamond:
                spawnObj = pivoteDiamond;
                break;
        }
        return spawnObj;
    }

    enum ConstrucIndex
    {
        StartPos = 0,
        PivoteBrick = 1,
        PivoteWall = 2,
        PivoteUnBrick = 3,
        PivotePush = 4,
        WinPos = 5,
        PivoteDiamond  = 6
    }

    void IdleUI()
    {
        UIManager.instance.IdleUIAni();
    }
    void WinUI()
    {
        UIManager.instance.WinUIAni();
    }
    void LoseUI()
    {
        UIManager.instance.LoseUIAni();
    }
}
