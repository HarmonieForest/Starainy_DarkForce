/****************************************************
    文件：IState.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/14 9:12:44
	功能：状态接口
*****************************************************/



public interface IState 
{
    void Enter(EntityBase entity,params object[] args);

    void Process(EntityBase entity, params object[] args);

    void Exit(EntityBase entity, params object[] args);
}

public enum AniState
{
    None,
    Idle,
    Move,
    Attack,
    Born,
    Die,
    Hit,
}
