/****************************************************
	文件：ChatSys.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/05/05 15:01   	
	功能：服务器端聊天系统开发
*****************************************************/

using PEProtocol;
using System.Collections.Generic;

public class ChatSys
{
    private static ChatSys instance = null;
    public static ChatSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ChatSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;

        PECommon.Log("ChatSys Init Done");
    }


    public void ReqChat(MsgPack pack)
    {
        ReqChat data = pack.msg.reqChat;
        PlayerData playerData = cacheSvc.GetPlayerDataByServerSession(pack.session);

        TaskSys.Instance.CalTaskPrgs(playerData, 6);
        GameMsg msg = new GameMsg
        {
            cmd=(int)CMD.RspChat,
            rspChat=new RspChat
            {
                name=playerData.name,
                chat=data.chat,
            }
        };
        //提前序列化减少消耗
        byte[] bytes = PENet.PETool.PackNetMsg(msg);
        //广播数据
        List<ServerSession> lst = cacheSvc.GetOnlineSeverSessions();
        for(int i = 0; i < lst.Count; i++)
        {
            lst[i].SendMsg(bytes);
        }
    }
}

