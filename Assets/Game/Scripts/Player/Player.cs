using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Animator anim;

    [Header("GeneralSettings")]
    [SerializeField] Direct directPlayer;
    [SerializeField] Transform modelTransform;
    [SerializeField] float speed;
    [SerializeField] Transform frontRayTransform;

    [Header("Status")]
    [SerializeField] bool isMoving;
    [SerializeField] bool isStop;

    [Header("BrickSettings")]
    [SerializeField] int brickCount;
    [SerializeField] LayerMask brickLayer;
    [SerializeField] List<GameObject> BricksList;
    public int brickLeft => BricksList.Count;
    [SerializeField] GameObject brickAddPrefab;
    [SerializeField] float brickOffset = 0.5f;
    [SerializeField] float offsetGiveBackBrick;
    float brickWallHeight;

    [Header("DiamondSettings")]
    [SerializeField] int diamondCount;

    // Private property
    public Vector3 targetPosition;
    RaycastHit hitFront;

    // Animation
    string currentAnim;
    const string animIdle = "Idle";
    const string animWin = "Win";

    void Start()
    {
        OnInit();
    }

    void Update()
    {
        modelTransform.forward = Vector3.forward;
        if (isStop) return;
        ControlPlayer.instance.FindDirection(ref directPlayer);
        Control();     
    }

    public void OnInit()
    {
        directPlayer = Direct.None;
        brickWallHeight = -0.5f;
        Invoke(nameof(DelayStatus), 0.5f);
        brickCount = 0;
        diamondCount = 0;
        UIManager.instance.SetDiamond(diamondCount);
        ChangeAnim(animIdle);
    }

    void DelayStatus()
    {
        isStop = false;
        isMoving = false;
    }

    void OnDespawn()
    {

    }

    void Control()
    {
        if (FrontCheck())
        {
            targetPosition = new Vector3(hitFront.collider.transform.position.x, transform.position.y, hitFront.collider.transform.position.z);
            if (directPlayer != Direct.None)
            {
                isMoving = true;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
            }
        }

        if (!isMoving)
        {
            switch (directPlayer)
            {
                case Direct.Forward:
                    transform.forward = Vector3.forward;
                    break;
                case Direct.Back:
                    transform.forward = -Vector3.forward;
                    break;
                case Direct.Right:
                    transform.forward = Vector3.right;
                    break;
                case Direct.Left:
                    transform.forward = -Vector3.right;
                    break;
                case Direct.None:
                    break;
            }
        }
        else
        {
            directPlayer = Direct.None;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
        }
    }
    void ChangeAnim(string animID)
    {
        if (currentAnim != animID)
        {
            anim.ResetTrigger(currentAnim);
            currentAnim = animID;
            anim.SetTrigger(currentAnim);
        }
    }

    bool FrontCheck()
    {
        Physics.Raycast(frontRayTransform.position, Vector3.down, out hitFront, 10f, brickLayer);
        return hitFront.collider != null;
    }

    public void AddBrick()
    {
        brickCount++;
        GameObject brickClone = Instantiate(brickAddPrefab, transform);
        brickClone.transform.localPosition = new Vector3(0, brickWallHeight, 0);
        BricksList.Add(brickClone);
        brickWallHeight += brickOffset;
        modelTransform.localPosition = new Vector3(0, brickWallHeight, 0);
        UIManager.instance.SetCoin(brickCount);
    }

    public void RemoveBrick()
    {
        if (BricksList.Count > 0)
        {
            Destroy(BricksList[BricksList.Count - 1]);
            BricksList.Remove(BricksList[BricksList.Count - 1]);
        }
        else
        {
            isStop = true;
            LevelManager.instance.Lose();
        }
        brickWallHeight -= brickOffset;
        modelTransform.localPosition = new Vector3(0, brickWallHeight, 0);
    }

    public void ClearBrick()
    {
        isStop = true;
        for(int i = BricksList.Count -1; i >= 0; i--)
        {
            Destroy(BricksList[i]);
            BricksList.Remove(BricksList[i]);
        }
        modelTransform.localPosition = new Vector3(0, -0.5f, 0);
        ChangeAnim(animWin);
    }

    public void AddDiamond()
    {
        isStop = true;
        int diamondThisReceive = LevelManager.instance.DiamondPoint();
        diamondCount += diamondThisReceive;
        UIManager.instance.SetDiamond(diamondCount);
        UIManager.instance.SetNumber(diamondThisReceive);
        UIManager.instance.DiamondUIAni();
    }
    public void ContinuePlayer()
    {
        isStop = false;
    }
}



