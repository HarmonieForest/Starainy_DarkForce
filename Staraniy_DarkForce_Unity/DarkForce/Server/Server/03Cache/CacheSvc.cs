/****************************************************
	文件：CacheSvc.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/04/28 17:54   	
	功能：缓存层
*****************************************************/
using PEProtocol;
using System.Collections.Generic;

public class CacheSvc
{
    private static CacheSvc instance = null;
    public static CacheSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CacheSvc();
            }
            return instance;
        }
    }
    //建立字典存储缓存所有在线用户
    private Dictionary<string, ServerSession> onLineUserDict = new Dictionary<string, ServerSession>();
    //建立字典存储在线用户的数据
    private Dictionary<ServerSession, PlayerData> onLineSessionDict = new Dictionary<ServerSession, PlayerData>();

    private DBMng dbMng;
    public void Init()
    {
        dbMng = DBMng.Instance;
        PECommon.Log("CacheSvc Init Done");
    }
    //判断是否在线
    public bool IsUserOnline(string userID)
    {
        return onLineUserDict.ContainsKey(userID);
    }
    ///<summary>
    ///根据账号密码获取用户数据,密码错误返回null,用户不存在则默认创建新的用户
    /// </summary>
    public PlayerData GetUserData(string userID,string password)
    {
        //从数据库中获取
        return dbMng.QueryPlayerData(userID,password);
    }
    ///<summary>
    ///账号上线,缓存数据
    /// </summary>
    public void UserDataOnline(string userID,ServerSession session,PlayerData playerData)
    {
        onLineUserDict.Add(userID, session);
        onLineSessionDict.Add(session, playerData);
    }

    //判断数据库中是否有该名字信息
    public bool IsNameExist(string name)
    {
        return dbMng.QueryNameData(name);
    }
    //根据session获取用户数据
    public PlayerData GetPlayerDataByServerSession(ServerSession session)
    {
        if (onLineSessionDict.TryGetValue(session,out PlayerData playerData))
        {//缓存中有数据
            return playerData;
        }
        else
        {
            //缓存中无数据
            return null;
        }
    }
    //检验数据库是否更新成功
    public bool IsUpdateSucc(int id,PlayerData playerData)
    {
        return dbMng.UpdatePlayerData(id, playerData);
        
    }
    //用户下线,缓存中清除用户数据
    public void UserOffLine(ServerSession session)
    {
        foreach(var item in onLineUserDict)
        {
            if (item.Value == session)
            {
                onLineUserDict.Remove(item.Key);
                break;
            }
        }
        bool succ= onLineSessionDict.Remove(session);
        PECommon.Log("OffLineResult:SessionID:" + session.sessionID + "-" + succ);
    }

    public List<ServerSession> GetOnlineSeverSessions()
    {
        List<ServerSession> lst = new List<ServerSession>();
        foreach(var item in onLineSessionDict)
        {
            lst.Add(item.Key);
        }
        return lst;
    }

    //获取所有在线用户数据
    public Dictionary<ServerSession,PlayerData> GetOnlineCache()
    {
        return onLineSessionDict;
    }

    public ServerSession GetOnlineServerSession(int ID)
    {
        ServerSession session = null;
        foreach(var item in onLineSessionDict)
        {
            if (item.Value.id == ID)
            {
                session = item.Key;
                break;
            }
        }
        return session;
    }
}
