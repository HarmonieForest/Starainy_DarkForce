/****************************************************
    文件：EntityBase.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/14 9:14:55
	功能：逻辑实体基类
*****************************************************/




using System.Collections.Generic;
using UnityEngine;

public class EntityBase 
{
    public AniState curtState = AniState.None;

    public StateMng stateMng = null;
    protected Controller controller = null;
    public SkillMng skillMng = null;
    public BattleMng battleMng = null;
    public bool canControl = true;
    public bool canReleaseSkill = true;
    public SkillCfg curtSkillCfg;

    public EntityType entityType = EntityType.None;
    public EntityState entityState = EntityState.None;

    private BattleProps props;
    private int hp;
    public BattleProps Props { get => props;protected set => props = value; }
    public int HP { 
        get{
            return hp; 
        }  
        set
        {
            PECommon.Log("hp change" + hp + "to" + value);
            SetHPVal(hp, value);
            hp = value;
        }
    }
    private string name;
    public string Name { 
        get{ return name;} 
        set{ name = value;} 
    }

    public Queue<int> comboQue = new Queue<int>();
    public int nextSkID = 0;


    public virtual void SetBattleProps(BattleProps props)
    {
        HP = props.hp;
        Props= props;
    }
    public void Born()
    {
        stateMng.ChangeStatus(this, AniState.Born, null);
    }
    public void Die()
    {
        stateMng.ChangeStatus(this, AniState.Die, null);
    }
    public void Move()
    {
        stateMng.ChangeStatus(this, AniState.Move,null);
    }
    public void Idle()
    {
        stateMng.ChangeStatus(this, AniState.Idle,null);
    }
    public  void Attack(int skillID)
    {
        stateMng.ChangeStatus(this, AniState.Attack,skillID);
    }
    public void Hit()
    {
        stateMng.ChangeStatus(this, AniState.Hit, null);
    }

    public virtual void SetBlend(float blend)
    {
        if (controller != null)
        {
            controller.SetBlend(blend);
        }
    }
    public virtual void SetDir(Vector2 dir)
    {
        if (controller != null)
        {
            controller.Dir = dir;
        }
    }
    public virtual void SetAction(int act)
    {
        if (controller != null)
        {
            controller.SetAction(act);
        }
    }
    public virtual void SetFX(string name, float destroy)
    {
        if (controller != null)
        {
            controller.SetFX(name, destroy);
        }
    }
    public virtual void SetSkillMoveState(bool move,float speed=0)
    {
        if (controller != null)
        {
            controller.SetSkillMoveState(move, speed);
        }
    }
    public virtual void SetAtkRotation(Vector2 dir,bool offset=false)
    {
        if (controller != null)
        {
            if (offset)
            {
                controller.SetAtkRotationCam(dir);
            }
            else
            {
                controller.SetAtkRotationLocal(dir);
            }
            
        }
    }
    public List<int> skMoveCBLst = new List<int>();
    public List<int> skActionCBLst = new List<int>();
    public int skEndCB = -1;

    public virtual void SetDodge()
    {
        if (controller != null)
        {
            GameRoot.Instance.dynamicPanel.SetDodge(Name);
        }
    }
    public virtual void SetCritical(int critical)
    {
        if (controller != null)
        {
            GameRoot.Instance.dynamicPanel.SetCritical(Name, critical);
        }
    }
    public virtual void SetHurt(int damage)
    {
        if (controller != null)
        {
            GameRoot.Instance.dynamicPanel.SetHurt(Name, damage);
        }
    }
    public virtual void SetHPVal(int oldVal,int newVal)
    {
        if (controller != null)
        {
            GameRoot.Instance.dynamicPanel.SetHPVal(Name, oldVal, newVal);
        }
        
    }


    public virtual void SkillAttack(int skillID)
    {
        skillMng.SkillAttack(this, skillID);            
    }  
    public virtual Vector2 GetCurtIptDir()
    {
        return Vector2.zero;
    } 
    public virtual Vector3 GetPos()
    {
        return controller.transform.position;
    }
    public virtual Transform GetTrans()
    {
        return controller.transform;
    }
    public AnimationClip[] GetAniClips()
    {
        if (controller != null)
        {
            return controller.ani.runtimeAnimatorController.animationClips;
        }
        return null;
    }
    public void SetController(Controller ctrl)
    {
        controller = ctrl;
    }
    public void SetActive(bool isActive = true)
    {
        if (controller != null)
        {
            controller.gameObject.SetActive(isActive);
        }      
    }
    public void ExitCurtSkill()
    {
        if (curtSkillCfg != null)
        {
            if (!curtSkillCfg.isBreak)
            {
                entityState = EntityState.None;
            }
            canControl = true;
            if (curtSkillCfg.isCombo)
            {
                if (comboQue.Count > 0)
                {
                    nextSkID = comboQue.Dequeue();
                }
                else
                {
                    nextSkID = 0;
                }
            }
            curtSkillCfg = null;
        }
    
        SetAction(Constants.ActionDefault);
    }
    
    public virtual Vector2 CalTargetDir()
    {
        return Vector2.zero;
    }


    public virtual void TickAILogic()
    {

    }
    public AudioSource GetAudio()
    {
        return controller.GetComponent<AudioSource>();
    }
    public CharacterController GetCharacterCtrl()
    {
        return controller.GetComponent<CharacterController>();
    }
    public void RemoveMoveCB(int tid)
    {
        int index = -1;
        for(int i = 0; i < skMoveCBLst.Count; i++)
        {
            if (skMoveCBLst[i] == tid)
            {
                index = i;
                break;
            }
        }
        if (index != -1)
        {
            skMoveCBLst.RemoveAt(index);
        }
    }
    public void RemoveActionCB(int tid)
    {
        int index = -1;
        for (int i = 0; i < skActionCBLst.Count; i++)
        {
            if (skActionCBLst[i] == tid)
            {
                index = i;
                break;
            }
        }
        if (index != -1)
        {
            skActionCBLst.RemoveAt(index);
        }
    }

    public virtual bool GetBreakState()
    {
        return true;
    }

    public void RemoveSkillCB()
    {
        SetDir(Vector2.zero);
        SetSkillMoveState(false);

        for (int i = 0; i < skMoveCBLst.Count; i++)
        {
            int tid = skMoveCBLst[i];
            TimerSvc.Instance.DeleteTask(tid);
        }
        for (int i = 0; i < skActionCBLst.Count; i++)
        {
            int tid = skActionCBLst[i];
            TimerSvc.Instance.DeleteTask(tid);
        }
        //技能被中断,删除定时回调
        if (skEndCB != -1)
        {
            TimerSvc.Instance.DeleteTask(skEndCB);
            skEndCB = -1;
        }
        skMoveCBLst.Clear();
       skActionCBLst.Clear();

        if (nextSkID != 0 ||comboQue.Count > 0)
        {
            nextSkID = 0;
            comboQue.Clear();

            battleMng.lastAtkTime = 0;
            battleMng.comboIndex = 0;
        }
    }
    
}