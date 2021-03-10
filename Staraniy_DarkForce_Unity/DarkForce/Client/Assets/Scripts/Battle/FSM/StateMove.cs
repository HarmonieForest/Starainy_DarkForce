/****************************************************
	文件：StateMove.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/05/14 9:19   	
	功能：行走状态
*****************************************************/

public class StateMove : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.curtState = AniState.Move;   
    }

    public void Exit(EntityBase entity, params object[] args)
    {
     
    }

    public void Process(EntityBase entity, params object[] args)
    {
        
        entity.SetBlend(Constants.BlendMove);
    } 
}
