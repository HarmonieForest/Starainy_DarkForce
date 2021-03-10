/****************************************************
    文件：MapMng.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/13 17:44:18
	功能：地图管理器
*****************************************************/

using UnityEngine;

public class MapMng : MonoBehaviour 
{
    private int wavIndex = 1;
    private BattleMng battleMng;
    public TriggerData[] triggerArr;
    public void Init(BattleMng battle)
    {
        battleMng = battle;
        //第一批怪物刷新
        battleMng.LoadMonsterByWaveID(wavIndex);
        PECommon.Log("Init MapMng Done");
    }

    public void TriggerMonsterBorn(TriggerData triggerData,int waveIndex)
    {
        if (battleMng != null)
        {
            BoxCollider bo = triggerData.gameObject.GetComponent<BoxCollider>();
            bo.isTrigger = false;
            battleMng.LoadMonsterByWaveID(waveIndex);
            battleMng.ActiveCurtWaveMonster();
            battleMng.triggerCheck = true;
        }
    }
    public bool SetNextTriggerOn()
    {
        wavIndex += 1;
        for(int i = 0; i < triggerArr.Length; i++)
        {
            if (triggerArr[i].triggerWave == wavIndex)
            {
                BoxCollider bo = triggerArr[i].GetComponent<BoxCollider>();
                bo.isTrigger = true;
                return true;
            }         
        }
        return false;
    }
}