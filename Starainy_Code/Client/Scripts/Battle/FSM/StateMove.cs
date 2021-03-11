/****************************************************
	文件：StateMove.cs
	作者：Harmonie
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
