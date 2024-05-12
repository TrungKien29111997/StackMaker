using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    Scene sceneLoaded;
    public static ScenesManager instance;
    [SerializeField] GameObject player;
    [SerializeField] Transform startPoint;
    [SerializeField] List<ParticleSystem> fireWorks;
    [SerializeField] GameObject chest;
    const int diamondPerGet = 10;
    const int numberScene = 4;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Oninit();
    }

    public void Oninit()
    {
        player.transform.position = startPoint.position;
        sceneLoaded = SceneManager.GetActiveScene();
    }

    public int DiamondPoint()
    {
        return Random.Range(diamondPerGet, diamondPerGet + 10) * 10;
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
}
