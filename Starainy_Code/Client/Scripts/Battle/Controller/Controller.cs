/****************************************************
    文件：Controller.cs
	作者：Harmonie
	功能：表现实体控制器基类
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour 
{
    public Animator ani;
    public CharacterController ctrl;
    protected Transform camTrans;
    public Transform hpRootTrans;

    protected bool isMove = false;
    protected bool isAct = false;
    private Vector2 dir = Vector2.zero;

    protected bool skillMove = false;
    protected float skillMoveSpeed = 0f;

    protected Dictionary<string, GameObject> fxDict = new Dictionary<string, GameObject>();
    protected TimerSvc timerSvc;

    public virtual void Init()
    {
        timerSvc = TimerSvc.Instance;
    }
    public Vector2 Dir
    {
        get
        {
            return dir;
        }

        set
        {
            if (value == Vector2.zero)
            {
                isMove = false;
            }
            else
            {
                isMove = true;
            }
            dir = value;
        }
    }
    public virtual void SetBlend(float blend)
    {
        ani.SetFloat("Blend", blend);
    }
    public virtual void SetAction(int act)
    {
        isAct = true;
        ani.SetInteger("Action", act);
    }
    public virtual void SetFX(string name, float destroy)
    {
    
    }
    public void SetSkillMoveState(bool move,float speed=0)
    {
        skillMove = move;
        skillMoveSpeed = speed;
    }
    public void SetAtkRotationCam(Vector2 camDir)
    {
        float angle = Vector2.SignedAngle(camDir, new Vector2(0, 1)) + camTrans.eulerAngles.y;
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }
    public void SetAtkRotationLocal(Vector2 atkDir)
    {
        float angle = Vector2.SignedAngle(atkDir, new Vector2(0, 1));
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }
}