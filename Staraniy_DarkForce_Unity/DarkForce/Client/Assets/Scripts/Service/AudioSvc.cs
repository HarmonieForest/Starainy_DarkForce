/****************************************************
    文件：AudioSvc.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/4/19 16:9:54
	功能：声音播放服务
*****************************************************/

using UnityEngine;

public class AudioSvc : MonoBehaviour 
{
    public static AudioSvc Instance = null;
    public AudioSource bgAudio;
    public AudioSource uiAudio;
    public void InitSvc()
    {
        Instance = this;
        PECommon.Log("Init AudioSvc Done");
    }
    //播放背景音乐
    public void PlayBgMusic(string name,bool isLoop=true)
    {
        //通过ResSvc里的资源加载服务获得声音资源
        AudioClip audio = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);
        if (bgAudio.clip == null||bgAudio.clip.name!=audio.name)
        {
            bgAudio.clip = audio;
            bgAudio.loop = isLoop;
            bgAudio.Play();
        }
    }
    //播放UI音乐
    public void PlayUIAudio(string name)
    {
        AudioClip audio = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);
        uiAudio.clip = audio;
        uiAudio.Play();
    }
    public void PlayCharAudio(string name,AudioSource audioChar)
    {
        AudioClip audio = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);
        audioChar.clip = audio;
        audioChar.Play();
    }
    public void StopBgMusic()
    {
        if (bgAudio != null)
        {
            bgAudio.Stop();
        }
    }
}