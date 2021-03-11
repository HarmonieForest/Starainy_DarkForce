/****************************************************
    文件：TriggerData.cs
	作者：Harmonie
	功能：地图触发数据类
*****************************************************/

using UnityEngine;

public class TriggerData : MonoBehaviour 
{
    public int triggerWave;
    public MapMng mapMng;
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (mapMng != null)
            {
                mapMng.TriggerMonsterBorn(this,triggerWave);
            }
        }
    }
}