using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] Animator anim;
    [SerializeField] TextMeshProUGUI coinTex;
    [SerializeField] TextMeshProUGUI diamondTex;
    [SerializeField] TextMeshProUGUI numberTex;
    [SerializeField] TextMeshProUGUI stackTex;
    [SerializeField] TextMeshProUGUI scenceIndex;

    // animationID
    int aniUIID;


    void Awake()
    {
        instance = this;
        AssignAnimation();
    }

    void Start()
    {
        OnInit();
    }

    public void OnInit()
    {
        scenceIndex.text = "LEVEL" + (LevelManager.instance.CurrentLevel + 1).ToString();
        anim.SetInteger(aniUIID, (int)AniUI.Idle);
        Invoke(nameof(ResetAniInt), 0.5f);
    }

    void AssignAnimation()
    {
        aniUIID = Animator.StringToHash("AniInt");
    }

    public void SetCoin(int coin)
    {
        coinTex.text = coin.ToString();
        stackTex.text = coinTex.text;
    }

    public void SetDiamond(int diamond)
    {
        diamondTex.text = diamond.ToString();
    }

    public void SetNumber(int number)
    {
        numberTex.text = "+" + number.ToString();
    }
    public void IdleUIAni()
    {
        anim.SetInteger(aniUIID, (int)AniUI.Idle);
        Invoke(nameof(ResetAniInt), 0.5f);
    }

    public void WinUIAni()
    {
        anim.SetInteger(aniUIID, (int)AniUI.Win);
        Invoke(nameof(ResetAniInt), 0.5f);
    }

    public void LoseUIAni()
    {
        anim.SetInteger(aniUIID, (int)AniUI.Lose);
        Invoke(nameof(ResetAniInt), 0.5f);
    }

    public void DiamondUIAni()
    {
        anim.SetInteger(aniUIID, (int)AniUI.Diamond);
        Invoke(nameof(ResetAniInt), 0.5f);
    }
    void ResetAniInt()
    {
        anim.SetInteger(aniUIID, -1);
    }

    public enum AniUI
    {
        Idle = 0,
        Lose = 1,
        Win = 2,
        Diamond = 3,
    }
}
