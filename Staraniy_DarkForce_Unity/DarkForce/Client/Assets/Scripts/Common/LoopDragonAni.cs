/****************************************************
    文件：LoopDragonAni.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/4/21 18:7:53
	功能：循环播放飞龙动画
*****************************************************/

using UnityEngine;

public class LoopDragonAni : MonoBehaviour 
{
    private Animation ani;
    private void Awake()
    {
        ani = transform.GetComponent<Animation>();
    }
    private void Start()
    {
        if (ani != null)
        {
            InvokeRepeating("PlayDragonAni", 0, 10);
        }
    }
    private void PlayDragonAni()
    {
        if (ani != null)
        {
            ani.Play();
        }
     
    }
}