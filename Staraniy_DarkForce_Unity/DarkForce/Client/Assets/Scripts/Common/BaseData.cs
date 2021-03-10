/****************************************************
    文件：BaseData.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/2 9:40:22
	功能：数据配置类
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public class BaseData<T>
{
    public int ID;
}

public class MapCfg : BaseData<MapCfg>
{
    public string mapName;
    public string sceneName;
    public int power;
    public Vector3 mainCamPos;
    public Vector3 mainCamRote;
    public Vector3 playerBornPos;
    public Vector3 playerBornRote;
    public List<MonsterData> monterLst;
    public int gold;
    public int exp;
    public int crystal;
}

public class AutoGuideCfg : BaseData<AutoGuideCfg>
{
    public int npcID;//触发任务目标NPC索引号
    public string dilogArr;//
    public int actID;//action行为ID
    public int gold;
    public int exp;
}

public class StrongCfg : BaseData<StrongCfg>
{
    public int pos;
    public int starlv;
    public int addhp;
    public int addhurt;
    public int adddef;
    public int minlv;
    public int gold;
    public int crystal;
}

public class TaskRewardCfg : BaseData<TaskRewardCfg>
{
    public string taskName;
    public int gold;
    public int exp;
    public int count;
}
public class TaskRewardState : BaseData<TaskRewardState>
{
    public int prgs;
    public bool taked;
}

public class SkillCfg : BaseData<SkillCfg>
{
    public string skillName;
    public int skillTime;
    public int aniAction;
    public string fx;
    public int cdTime;
    public bool isCombo;
    public bool isCollide;
    public bool isBreak;
    public DamageType damageType;
    public List<int> skillMoveLst;
    public List<int> skillActionLst;
    public List<int> skillDamageLst;
}
public class SkillActionCfg : BaseData<SkillActionCfg>
{
    public int delayTime;
    public float radius;
    public float angle;
}
public class SkillMoveCfg : BaseData<SkillMoveCfg>
{
    public int moveTime;
    public float moveDis;
    public int delayTime;
}
public class MonsterCfg : BaseData<MonsterCfg>
{
    public string mName;
    public string resPath;
    public MonsterType monsterType;
    public bool isStop;
    public BattleProps battleProps;
    public int skillID;
    public float atkDis;
}

public class MonsterData : BaseData<MonsterData>
{
    public int mWave;//批号
    public int mIndex;//序号
    public MonsterCfg monsterCfg;
    public Vector3 mBornPos;
    public Vector3 mBornRotate;
    public int mLevel;
}
public class BattleProps
{
    public int hp;
    public int ad;
    public int ap;
    public int addef;
    public int apdef;
    public int dodge;
    public int pierce;
    public int critical;

}
