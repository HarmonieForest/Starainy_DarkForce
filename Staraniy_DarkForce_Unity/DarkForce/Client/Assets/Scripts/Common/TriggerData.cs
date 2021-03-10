/****************************************************
    文件：TriggerData.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/16 17:15:28
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