using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    Scene sceneLoaded;
    public static LevelManager instance;
    [SerializeField] List<TextAsset> levelData = new List<TextAsset>();
    [SerializeField] int levelCount;
    public int CurrentLevel => levelCount;

    [SerializeField] GameObject pivoteStart;
    [SerializeField] GameObject pivoteBrick;
    [SerializeField] GameObject pivoteWall;
    [SerializeField] GameObject pivoteUnBrick;
    [SerializeField] GameObject pivotePush;
    [SerializeField] GameObject pivoteDiamond;
    [SerializeField] GameObject winArea;
    [SerializeField] List<GameObject> contructionList;

    [SerializeField] Player player;
    [SerializeField] Transform levelTransform;
    [SerializeField] List<ParticleSystem> fireWorks;
    [SerializeField] GameObject chest;
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
        levelCount = 0;
        if (SceneManager.GetActiveScene().name != "LEVEL_EDITOR")
        {
            CreateLevel(0);
        }
        Oninit();
    }

    void Oninit()
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
        player.ContinuePlayer();
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

    void NewLevel(int levelIndex)
    {
        ClearLevel();
        CreateLevel(levelIndex);
        player.OnInit();
        UIManager.instance.OnInit();
    }

    public void PlayAgain()
    {
        NewLevel(levelCount);
    }

    public void NextLevel()
    {
        if (levelCount < levelData.Count - 1)
        {
            NewLevel(++levelCount);
        }
        else
        {
            levelCount = 0;
            NewLevel(0);
        }
    }


    void CreateLevel(int levelIndex)
    {
        string data = levelData[levelIndex].text.Replace(Environment.NewLine, string.Empty);
        string[] mapData = data.Split('-');

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
                if (map2D[i, j] == (int)ConstrucIndex.WinPos) // dong nay tim goc xoay WinPos
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
                if (map2D[i, j] == (int)ConstrucIndex.PivotePush) // dong nay tim goc xoay PivotePush
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

    void ClearLevel()
    {
        for (int i = contructionList.Count -1; i >= 0; i--)
        {
            Destroy(contructionList[i]);
            contructionList.Remove(contructionList[i]);
        }

    }

    void SpawnConstruction(int constructionIndex, int xNum, int yNum, float rotate)
    {
        if (constructionIndex != -1)
        {
            GameObject tempConstruction = Instantiate(ConstructionType(constructionIndex), levelTransform);
            contructionList.Add(tempConstruction);

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
}
