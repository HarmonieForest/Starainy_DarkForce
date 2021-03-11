/****************************************************
    文件：BattleMng.cs
	作者：Harmonie
	功能：战斗相关管理类
*****************************************************/


using PEProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleMng : MonoBehaviour 
{
    private AudioSvc audioSvc;
    private ResSvc resSvc;

    private MapMng mapMng;
    private SkillMng skillMng;
    private StateMng stateMng;

    private MapCfg mapCfg;

    public EntityPlayer entitySelfPlayer;

    public bool triggerCheck = true;
    public bool isPauseGame = false;

    private Dictionary<string, EntityMonster> monsterDic = new Dictionary<string, EntityMonster>();

    public void InitMission(int missionID,Action cb=null)
    {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;
        //初始化管理器        
        skillMng = gameObject.AddComponent<SkillMng>();
        skillMng.Init();
        stateMng = gameObject.AddComponent<StateMng>();
        stateMng.Init();

        

        //加载战场地图
        mapCfg = resSvc.GetMapCfg(missionID);
        resSvc.AsyncLoadScene(mapCfg.sceneName,()=> {
            //初始化地图数据
            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
            mapMng = map.GetComponent<MapMng>();
            mapMng.Init(this);

            map.transform.localPosition = Vector3.zero;
            map.transform.localScale = Vector3.one;

            Camera.main.transform.position = mapCfg.mainCamPos;
            Camera.main.transform.localEulerAngles = mapCfg.mainCamRote;

            LoadPlayer(mapCfg);
            entitySelfPlayer.Idle();
            //激活第一批怪物
            ActiveCurtWaveMonster();

            audioSvc.PlayBgMusic(Constants.BgHuangye);

            if (cb != null)
            {
                cb();
            }
        });
       
        PECommon.Log("BattleMng Init Done");
    }

    public void Update()
    {
        foreach(var item in monsterDic)
        {
            EntityMonster entityMonster = item.Value;
            entityMonster.TickAILogic();
        }
        //检测当前批次怪物是否成功清除
        if (mapMng != null)
        {
            if (monsterDic.Count == 0&&triggerCheck)
            {
                bool isExist= mapMng.SetNextTriggerOn();
                triggerCheck = false;
                if (!isExist)
                {
                    //战斗结束,进入下一关
                    EndBattle(true,entitySelfPlayer.HP);
                }
            }
        }
    }
    public void EndBattle(bool isWin,int restHP)
    {
        isPauseGame = true;
        audioSvc.StopBgMusic();
        BattleSys.Instance.EndBattle(isWin, restHP);
    }


    #region 人物加载
    private void LoadPlayer(MapCfg mapData)
    {
        
        GameObject player = resSvc.LoadPrefab(PathDefine.AssassinBattlePlayerPrefab);
        player.GetComponent<Animator>().applyRootMotion = false;

        player.transform.position = mapData.playerBornPos;
        player.transform.localEulerAngles = mapData.playerBornRote;
        player.transform.localScale = Vector3.one;

        PlayerData playerData = GameRoot.Instance.PlayerData;
        BattleProps props = new BattleProps
        {
            hp = playerData.hp,
            ad = playerData.ad,
            ap = playerData.ap,
            addef = playerData.addef,
            apdef = playerData.apdef,
            critical = playerData.critical,
            pierce = playerData.pierce,
            dodge = playerData.dodge,
        };
        entitySelfPlayer = new EntityPlayer
        {
            stateMng = stateMng,
            skillMng=skillMng,
            battleMng=this,
        };
        entitySelfPlayer.Name = "AssassinBattle";
        entitySelfPlayer.SetBattleProps(props);
        PlayerController playerCtrl = player.GetComponent<PlayerController>();
        playerCtrl.Init();
        entitySelfPlayer.SetController(playerCtrl);
    }
    public void LoadMonsterByWaveID(int wave)
    {
        for(int i = 0; i < mapCfg.monterLst.Count; i++)
        {
            MonsterData monsterData = mapCfg.monterLst[i];
            if (monsterData.mWave == wave)
            {
                GameObject monster = resSvc.LoadPrefab(monsterData.monsterCfg.resPath,true);
                monster.transform.localPosition = monsterData.mBornPos;
                monster.transform.localEulerAngles = monsterData.mBornRotate;
                monster.transform.localScale = Vector3.one;
                monster.name = "m" + monsterData.mWave + "_" + monsterData.mIndex;

                EntityMonster entityMonster = new EntityMonster
                {
                    stateMng = stateMng,
                    skillMng = skillMng,
                    battleMng = this,
                };
                entityMonster.md = monsterData;
                entityMonster.Name = monster.name;
                entityMonster.SetBattleProps(monsterData.monsterCfg.battleProps);
                MonsterController monsterCtrl = monster.GetComponent<MonsterController>();
                monsterCtrl.Init();
                entityMonster.SetController(monsterCtrl);
                monster.SetActive(false);
                monsterDic.Add(monster.name, entityMonster);

                if (monsterData.monsterCfg.monsterType == MonsterType.Normal)
                {
                    GameRoot.Instance.dynamicPanel.AddHPInfo(monster.name, monsterCtrl.hpRootTrans, entityMonster.HP);
                }else if(monsterData.monsterCfg.monsterType == MonsterType.Boss)
                {
                    BattleSys.Instance.playerCtrlPanel.SetBossHPBar(true);
                }
               
            }
        }
    }
    public void ActiveCurtWaveMonster()
    {
        TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            foreach(var item in monsterDic)
            {
                item.Value.SetActive(true);
                item.Value.Born();
                TimerSvc.Instance.AddTimeTask((int did) => {
                    item.Value.Idle();
                },1000);
            }
        },500);
    }

    public List<EntityMonster> GetEntityMonsters()
    {
        List<EntityMonster> monsterLst = new List<EntityMonster>();
        foreach(var item in monsterDic)
        {
            monsterLst.Add(item.Value);
        }
        return monsterLst;
    }
    public void RemoveMonster(string key)
    {
        EntityMonster monster;
        if(monsterDic.TryGetValue(key,out monster))
        {
            monsterDic.Remove(key);
            GameRoot.Instance.dynamicPanel.RemoveHPInfo(key);
        }
    }
    #endregion


    #region 技能释放与角色控制
    public void SetSelfPlayerMoveDirection(Vector2 dir)
    {
        if (entitySelfPlayer.canControl == false)
        {
            return;
        }
        if (entitySelfPlayer.curtState == AniState.Idle || entitySelfPlayer.curtState == AniState.Move)
        {
            if (dir == Vector2.zero)
            {
                entitySelfPlayer.Idle();
            }
            else
            {
                entitySelfPlayer.Move();
                entitySelfPlayer.SetDir(dir);
            }
        }
      
    }
    public void ReqReleaseSkill(int index)
    {
        switch (index)
        {
            case 0:
                ReleaseNormalAttk();
                break;
            case 1:
                ReleaseSkill1();
                break;
            case 2:
                ReleaseSkill2();
                break;
            case 3:
                ReleaseSkill3();
                break;
        }
    }
    public double lastAtkTime = 0;
    public int comboIndex = 0;
    private int[] comboArr = new int[] { 111,112,113,114,115};
    private void ReleaseNormalAttk()
    {
        if (entitySelfPlayer.curtState == AniState.Attack)
        {
            //500ms内点击,存数据
            double nowAtkTime = TimerSvc.Instance.GetNowTime();
            if (nowAtkTime - lastAtkTime < Constants.ComboSpace && lastAtkTime != 0)
            {
                if (comboArr[comboIndex]!=comboArr[comboArr.Length-1])
                {
                    comboIndex += 1;
                    entitySelfPlayer.comboQue.Enqueue(comboArr[comboIndex]);
                    lastAtkTime = nowAtkTime;
                }
                else
                {
                    lastAtkTime = 0;
                    comboIndex = 0;
                }
               
            }
        }
        else if(entitySelfPlayer.curtState==AniState.Idle||entitySelfPlayer.curtState==AniState.Move)
        {
            comboIndex = 0;
            lastAtkTime = TimerSvc.Instance.GetNowTime();
            entitySelfPlayer.Attack(comboArr[comboIndex]);
        }
       
    }
    private void ReleaseSkill1()
    {     
        entitySelfPlayer.Attack(101);
    }
    private void ReleaseSkill2()
    {
        entitySelfPlayer.Attack(102);
    }
    private void ReleaseSkill3()
    {
        entitySelfPlayer.Attack(103);
    }


    public Vector2 GetCurtDirInput()
    {
        return BattleSys.Instance.GetCurtDirInput();
    }

    public bool CanReleaseSkill()
    {
        return entitySelfPlayer.canReleaseSkill;
    }
    #endregion


}