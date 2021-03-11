
/****************************************************
	文件：DBMng.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/04/28 20:26   	
	功能：数据库管理
*****************************************************/
using MySql.Data.MySqlClient;
using PEProtocol;
using System;
using System.Configuration;
using System.Diagnostics;

public class DBMng
{
    private static DBMng instance = null;
    public static DBMng Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DBMng();
            }
            return instance;
        }
    }
    //定义一个数据库连接
    private MySqlConnection conn;
    public void Init()
    {
        conn = new MySqlConnection("server=172.31.107.175;User Id=harmonie;password=harmonie;Database=darkforce_server;Charset=utf8");
        conn.Open();
        PECommon.Log("DBMng Init Done");      
    }
    //查找数据库获得数据
    public PlayerData QueryPlayerData(string userID,string password) 
    {
        bool isNew = true;
        PlayerData playerData = null;
        MySqlDataReader reader = null;
        try
        {
            //查询
            MySqlCommand cmd = new MySqlCommand("select * from UserInfo where userID=@userID", conn);
            cmd.Parameters.AddWithValue("userID", userID);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                isNew = false;
                string _password = reader.GetString("password");
                if (_password.Equals(password))
                {
                    //密码正确,返回玩家数据
                    playerData = new PlayerData
                    {
                        id = reader.GetInt32("id"),
                        name = reader.GetString("name"),
                        gold = reader.GetInt32("gold"),
                        lv = reader.GetInt32("lv"),
                        power = reader.GetInt32("power"),
                        exp = reader.GetInt32("exp"),
                        diamond = reader.GetInt32("diamond"),
                        crystal = reader.GetInt32("crystal"),

                        hp = reader.GetInt32("hp"),
                        ad = reader.GetInt32("ad"),
                        ap = reader.GetInt32("ap"),
                        addef = reader.GetInt32("addef"),
                        apdef = reader.GetInt32("apdef"),
                        dodge = reader.GetInt32("dodge"),
                        pierce = reader.GetInt32("pierce"),
                        critical = reader.GetInt32("critical"),
                        guideid = reader.GetInt32("guideid"),
                        time=reader.GetInt64("time"),
                        mission=reader.GetInt32("mission"),

                    };
                    #region 强化信息
                    string[] strongStrArr=reader.GetString("strong").Split('#');
                    int[] _strongArr = new int[6];
                    for(int i = 0; i < strongStrArr.Length; i++)
                    {
                        if (strongStrArr[i] == "")
                        {
                            continue;
                        }
                        if(int.TryParse(strongStrArr[i],out int lilith))
                        {
                            _strongArr[i] = lilith;
                        }
                        else
                        {
                            PECommon.Log("Parse Strong Data Error", LogType.Error);
                        }
                    }
                    playerData.strongArr = _strongArr;
                    #endregion
                    #region 任务奖励信息
                    string[] taskStrArr = reader.GetString("task").Split('#');
                    playerData.taskArr = new string[6];
                    for(int i = 0; i< taskStrArr.Length; i++)
                    {
                        if (strongStrArr[i] == "")
                        {
                            continue;
                        }else if (taskStrArr[i].Length >= 5)
                        {
                            playerData.taskArr[i] = taskStrArr[i];
                        }
                        else
                        {
                            throw new Exception("Data Error");
                        }                       
                    }
                    #endregion
                }
            }
        }
        catch(Exception e)
        {
            PECommon.Log("Querry PlayerData by userID&&password error" + e, LogType.Error);
        }
        finally
        {//创建
            if (reader != null)
            {
                reader.Close();
            }
            
            //用户不存在,创建新用户
            if (isNew )
            {
                playerData = new PlayerData
                {
                    id = -1,
                    name = "",
                    gold = 5000,
                    lv = 1,
                    power = 50,
                    exp = 0,
                    diamond = 50,
                    crystal = 888,


                    hp = 2000,
                    ad = 275,
                    ap = 265,
                    addef = 67,
                    apdef = 43,
                    dodge = 7,
                    pierce = 5,
                    critical = 2,

                    guideid = 1001,

                    strongArr = new int[6],

                    time = TimerSvc.Instance.GetNowTime(),

                    taskArr = new string[6],

                    mission = 10001,

                };
                for(int i = 0; i < playerData.taskArr.Length; i++)
                {
                    playerData.taskArr[i] = (i + 1) + "|0|0";
                }



                //保证id不始终为-1,而是动态添加
                playerData.id= InsertNewUserData(userID, password, playerData);
            }
        }
        return playerData;
    }

    //输入新数据
    private int InsertNewUserData(string userID,string password,PlayerData playerData)
    {
        int id = -1;
        try
        {
            MySqlCommand cmd = new MySqlCommand("insert into UserInfo set userID=@userID,password =@password,name=@name,lv=@lv,exp=@exp,power=@power,gold=@gold,diamond=@diamond,crystal=@crystal,"+"hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef,dodge=@dodge,pierce=@pierce,critical=@critical,guideid=@guideid,strong=@strong,time=@time,task=@task,mission=@mission", conn);
            cmd.Parameters.AddWithValue("userID", userID);
            cmd.Parameters.AddWithValue("password", password);
            cmd.Parameters.AddWithValue("name", playerData.name);
            cmd.Parameters.AddWithValue("lv", playerData.lv);
            cmd.Parameters.AddWithValue("exp", playerData.exp);
            cmd.Parameters.AddWithValue("power", playerData.power);
            cmd.Parameters.AddWithValue("gold", playerData.gold);
            cmd.Parameters.AddWithValue("diamond", playerData.diamond);
            cmd.Parameters.AddWithValue("crystal", playerData.crystal);

            cmd.Parameters.AddWithValue("hp", playerData.hp);
            cmd.Parameters.AddWithValue("ad", playerData.ad);
            cmd.Parameters.AddWithValue("ap", playerData.ap);
            cmd.Parameters.AddWithValue("addef", playerData.addef);
            cmd.Parameters.AddWithValue("apdef", playerData.apdef);
            cmd.Parameters.AddWithValue("dodge", playerData.dodge);
            cmd.Parameters.AddWithValue("pierce", playerData.pierce);
            cmd.Parameters.AddWithValue("critical", playerData.critical);

            cmd.Parameters.AddWithValue("guideid", playerData.guideid);

            cmd.Parameters.AddWithValue("mission", playerData.mission);
         
            string strongInfo = "";
            for (int i = 0; i < playerData.strongArr.Length; i++)
            {
                strongInfo += playerData.strongArr[i];
                strongInfo += "#";
            }
            cmd.Parameters.AddWithValue("strong", strongInfo);
            //todo
            cmd.Parameters.AddWithValue("time", playerData.time);

            string taskInfo = "";
            for(int i = 0; i < playerData.taskArr.Length; i++)
            {
                taskInfo += playerData.taskArr[i];
                taskInfo += "#";
            }
            cmd.Parameters.AddWithValue("task", taskInfo);

            cmd.ExecuteNonQuery();
            id = (int)cmd.LastInsertedId;

        }
        catch (Exception e)
        {
            PECommon.Log("Insert NewUserData error" + e, LogType.Error);
        }
        return id;
    }

    //判断数据库中是否有名字
    public bool QueryNameData(string name)
    {
        bool exist = false;
        MySqlDataReader reader = null;
        try
        {
            MySqlCommand cmd = new MySqlCommand("select * from UserInfo where name= @name", conn);
            cmd.Parameters.AddWithValue("name", name);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                exist = true;
            }
        }
        catch(Exception e)
        {
            PECommon.Log("Query name data error" + e, LogType.Error);
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
        }
        return exist;

    }
    public bool UpdatePlayerData(int id,PlayerData playerData)
    {
        bool IsUpdate = false;
        
        try
        {
            MySqlCommand cmd = new MySqlCommand(
            "update UserInfo set name=@name,lv=@lv,exp=@exp,power=@power,gold=@gold,diamond=@diamond,crystal=@crystal,"+"hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef,dodge=@dodge,pierce=@pierce,critical=@critical,guideid=@guideid,strong=@strong,time=@time,task=@task,mission=@mission where id =@id", conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("name", playerData.name);
            cmd.Parameters.AddWithValue("lv", playerData.lv);
            cmd.Parameters.AddWithValue("exp", playerData.exp);
            cmd.Parameters.AddWithValue("power", playerData.power);
            cmd.Parameters.AddWithValue("gold", playerData.gold);
            cmd.Parameters.AddWithValue("diamond", playerData.diamond);
            cmd.Parameters.AddWithValue("crystal", playerData.crystal);

            cmd.Parameters.AddWithValue("apdef", playerData.apdef);
            cmd.Parameters.AddWithValue("dodge", playerData.dodge);
            cmd.Parameters.AddWithValue("pierce", playerData.pierce);
            cmd.Parameters.AddWithValue("critical", playerData.critical);
            cmd.Parameters.AddWithValue("hp", playerData.hp);
            cmd.Parameters.AddWithValue("ad", playerData.ad);
            cmd.Parameters.AddWithValue("ap", playerData.ap);
            cmd.Parameters.AddWithValue("addef", playerData.addef); ;
            cmd.Parameters.AddWithValue("guideid", playerData.guideid);
            cmd.Parameters.AddWithValue("mission", playerData.mission);

            string strongInfo = "";
            for (int i = 0; i < playerData.strongArr.Length; i++)
            {
                strongInfo += playerData.strongArr[i];
                strongInfo += "#";
            }
            cmd.Parameters.AddWithValue("strong", strongInfo);

            cmd.Parameters.AddWithValue("time", playerData.time);

            string taskInfo = "";
            for (int i = 0; i < playerData.taskArr.Length; i++)
            {
                taskInfo += playerData.taskArr[i];
                taskInfo += "#";
            }
            cmd.Parameters.AddWithValue("task", taskInfo);

            IsUpdate = true;

            cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            PECommon.Log("Update PlayerData by ID error" + e, LogType.Error);

        }
        return IsUpdate;
    }

}
