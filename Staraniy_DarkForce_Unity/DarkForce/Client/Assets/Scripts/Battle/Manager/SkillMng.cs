/****************************************************
    文件：SkillMng.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/13 17:43:11
	功能：技能管理器
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public class SkillMng : MonoBehaviour 
{
    private ResSvc resSvc ;
    private TimerSvc timerSvc;
    
    public void Init()
    {
        resSvc = ResSvc.Instance;
        timerSvc = TimerSvc.Instance;
        PECommon.Log("SkillMng Init Done");
    }
    public void SkillAttack(EntityBase entity, int skillID)
    {
        entity.skMoveCBLst.Clear();
        entity.skActionCBLst.Clear();
        SkillEffect(entity, skillID);
        SkillDamage(entity, skillID);
    }
    private void SkillEffect(EntityBase entity,int skillID)
    {
        SkillCfg skillData = resSvc.GetSkillCfg(skillID);
        if (!skillData.isCollide)
        {
            //忽略刚体碰撞
            Physics.IgnoreLayerCollision(9, 10);
            timerSvc.AddTimeTask((int tid) =>
            {
                Physics.IgnoreLayerCollision(9, 10,false);
            },skillData.skillTime);
        }

        if (entity.entityType == EntityType.Player)
        {
            if (entity.GetCurtIptDir() == Vector2.zero)
            {
                //搜索最近怪物
                Vector2 dir = entity.CalTargetDir();
                if (dir != Vector2.zero)
                {
                    entity.SetAtkRotation(dir);
                }
            }
            else
            {
                entity.SetAtkRotation(entity.GetCurtIptDir(), true);
            }

        }

        entity.SetAction(skillData.aniAction);
        entity.SetFX(skillData.fx, skillData.skillTime);

        CalSkillMove(entity, skillData);
        
        entity.canControl = false;
        entity.SetDir(Vector2.zero);

        if (!skillData.isBreak)
        {
            entity.entityState = EntityState.BatiState;
        }
        entity.skEndCB=timerSvc.AddTimeTask((int tid) =>
        {
            entity.Idle();
        }, skillData.skillTime);
    }  
   
    private void SkillDamage(EntityBase entity, int skillID)
    {
      
        SkillCfg skillData = resSvc.GetSkillCfg(skillID);
        List<int> actionLst = skillData.skillActionLst;
        int sum = 0;
        for(int i = 0; i < actionLst.Count; i++)
        {
            SkillActionCfg skillActionCfg = resSvc.GetSkillActionCfg(actionLst[i]);
            sum += skillActionCfg.delayTime;
            int index = i;
            if (sum > 0)
            {
                int actID=timerSvc.AddTimeTask((int tid) =>
                {
                    if (entity != null)
                    {
                        SkillAction(entity, skillData, index);
                        entity.RemoveActionCB(tid);
                    }
                  
                },sum);
                entity.skActionCBLst.Add(actID);
            }
            else
            {
                SkillAction(entity, skillData,index);
            }
        }
    }
    private void SkillAction(EntityBase caster, SkillCfg skillCfg,int index)
    {
        SkillActionCfg skillActionCfg = resSvc.GetSkillActionCfg(skillCfg.skillActionLst[index]);
        int damage = skillCfg.skillDamageLst[index];
        if (caster.entityType == EntityType.Monster)
        {
            EntityPlayer target = caster.battleMng.entitySelfPlayer;
            if (target == null)
            {
                return;
            }
            if (InRange(caster.GetPos(), target.GetPos(), skillActionCfg.radius) && InAngle(caster.GetTrans(), target.GetPos(), skillActionCfg.angle))
            {
                CalDamage(caster, target, skillCfg, damage);
            }


        }
        else if (caster.entityType==EntityType.Player)
        {
            //获取场景中所有的怪物实体
            List<EntityMonster> monsterLst = caster.battleMng.GetEntityMonsters();

            for (int i = 0; i < monsterLst.Count; i++)
            {
                EntityMonster target = monsterLst[i];
                //判断距离和角度
                if (InRange(caster.GetPos(), target.GetPos(), skillActionCfg.radius) && InAngle(caster.GetTrans(), target.GetPos(), skillActionCfg.angle))
                {
                    CalDamage(caster, target, skillCfg, damage);
                }

            }
        }

    }
    System.Random rd = new System.Random();

    private void CalDamage(EntityBase caster,EntityBase target,SkillCfg skillCfg,int damage)
    {
        int damageSum = damage;
        if (skillCfg.damageType == DamageType.AD)
        {         
            int dodgeNum = PETools.RDInt(0, 100, rd);
            if (dodgeNum <= target.Props.dodge)
            {             
                target.SetDodge();
                return;
            }
            damageSum += caster.Props.ad;
            int criticalNum = PETools.RDInt(0, 100, rd);
            if (criticalNum <= caster.Props.critical)
            {
                float criticalRate = PETools.RDInt(1, 100, rd) / 100 + 1;                
                damageSum =(int)criticalRate*damageSum;
                target.SetCritical(damageSum);
            }
            int addef = (int)((1 - caster.Props.pierce / 100.0f) * target.Props.addef);
            damageSum -= addef;
           
        }
        else if(skillCfg.damageType==DamageType.AP)
        {         
            damageSum += caster.Props.ap;
            damageSum -= target.Props.apdef;
        }
        else
        {
        }
        
        if (damageSum < 0)
        {
            damageSum = 0;
        }
        if (target.HP <= damageSum)
        {
            target.HP = 0;    
            target.Die();
            if (target.entityType == EntityType.Monster)
            {
                target.battleMng.RemoveMonster(target.Name);
            }else if (target.entityType == EntityType.Player)
            {
                target.battleMng.EndBattle(false, 0);
                target.battleMng.entitySelfPlayer = null;
                
            }
            
        }
        else
        {
            target.HP -= damageSum;
            if (target.entityState == EntityState.None&&target.GetBreakState())
            {
                target.Hit();
            }
            
        }
        
        target.SetHurt(damageSum);

    }
    private bool InRange(Vector3 from,Vector3 to,float radius)
    {
        from.y = 0;
        to.y = 0;
        float dis = Vector3.Distance(from, to);
        if (dis <= radius)
        {
            return true;
        }
        return false;
    }
    private bool InAngle(Transform trans,Vector3 to,float angle)
    {
        if (angle == 360)
        {
            return true;
        }
        else
        {
            Vector3 start = trans.forward;
            Vector3 dir = (to - trans.position).normalized;
            float ang = Vector3.Angle(start, dir);
            if (ang < angle / 2)
            {
                return true;
            }
            return false;
        }
    }

private void CalSkillMove(EntityBase entity,SkillCfg skillData)
    {
        List<int> skillMoveLst = skillData.skillMoveLst;

        int sum = 0;
        for (int i = 0; i < skillMoveLst.Count; i++)
        {
            SkillMoveCfg skillMoveData = resSvc.GetSkillMoveCfg(skillMoveLst[i]);
            float speed = skillMoveData.moveDis / (skillMoveData.moveTime / 1000f);
            sum += skillMoveData.delayTime;
            if (sum > 0)
            {
                int moveID=timerSvc.AddTimeTask((int tid) =>
                {
                    entity.SetSkillMoveState(true, speed);
                    entity.RemoveMoveCB(tid);
                }, sum);
                entity.skMoveCBLst.Add(moveID);
            }
            else
            {
                entity.SetSkillMoveState(true, speed);
            }
            sum += skillMoveData.moveTime;
            int stopID= timerSvc.AddTimeTask((int tid) =>
            {
                entity.SetSkillMoveState(false);
                entity.RemoveMoveCB(tid);
            }, sum);
            entity.skMoveCBLst.Add(stopID);
        }
    }

}