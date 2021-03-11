/****************************************************
	文件：CfgSvc.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/05/04 10:02   	
	功能：服务器数据配置服务
*****************************************************/
using System.Xml;
using System.Collections.Generic;
using System;

public class CfgSvc
{
    private static CfgSvc instance = null;
    public static CfgSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CfgSvc();
            }
            return instance;
        }
    }
    public void Init()
    {
        InitGuideCfg();
        InitStrongCfg();
        InitTaskRewardCfg();
        InitMapCfg();
        PECommon.Log("CfgSvc Init Done.");
    }

    #region 自动引导配置
    private Dictionary<int, GuideCfg> guideDic = new Dictionary<int, GuideCfg>();
    private void InitGuideCfg()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(@"C:\ResCfgs\guide.xml");

        XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

        for (int i = 0; i < nodLst.Count; i++)
        {
            XmlElement ele = nodLst[i] as XmlElement;

            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
            GuideCfg mc = new GuideCfg
            {
                ID = ID
            };

            foreach (XmlElement e in nodLst[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "gold":
                        mc.gold = int.Parse(e.InnerText);
                        break;
                    case "exp":
                        mc.exp = int.Parse(e.InnerText);
                        break;
                }
            }
            guideDic.Add(ID, mc);
        }
        PECommon.Log("GuideCfg InitDone");
    }
    public GuideCfg GetGuideData(int id)
    {
        GuideCfg agc = null;
        if (guideDic.TryGetValue(id, out agc))
        {
            return agc;
        }
        return null;
    }

    #endregion

    #region 强化升级配置
    private Dictionary<int, Dictionary<int, StrongCfg>> strongDic = new Dictionary<int, Dictionary<int, StrongCfg>>();
    private void InitStrongCfg() 
    {   
            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:\ResCfgs\strong.xml");
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;
            //遍历获取ID号
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement xmlElement = xmlNodeList[i] as XmlElement;
                if (xmlElement.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(xmlElement.GetAttributeNode("ID").InnerText);
                StrongCfg strongCfg = new StrongCfg
                {
                    ID = ID
                };
                //遍历获取每个节点的数据
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    int val = int.Parse(e.InnerText);
                    switch (e.Name)
                    {
                        case "pos":
                            strongCfg.pos = val;
                            break;
                        case "starlv":
                            strongCfg.starlv = val;
                            break;
                        case "addhp":
                            strongCfg.addhp = val;
                            break;
                        case "addhurt":
                            strongCfg.addhurt = val;
                            break;
                        case "adddef":
                            strongCfg.adddef = val;
                            break;
                        case "minlv":
                            strongCfg.minlv = val;
                            break;
                        case "gold":
                            strongCfg.gold = val;
                            break;
                        case "crystal":
                            strongCfg.crystal = val;
                            break;
                    }
                }

                Dictionary<int, StrongCfg> dic = null;
                if (strongDic.TryGetValue(strongCfg.pos, out dic))
                {
                    dic.Add(strongCfg.starlv, strongCfg);
                }
                else
                {
                    dic = new Dictionary<int, StrongCfg>();
                    dic.Add(strongCfg.starlv, strongCfg);
                    strongDic.Add(strongCfg.pos, dic);
                }
            }
            PECommon.Log("StrongCfg InitDone");
    }
    public StrongCfg GetStrongCfg(int pos, int startlv)
    {
        StrongCfg data = null;
        Dictionary<int, StrongCfg> dic = null;
        if (strongDic.TryGetValue(pos, out dic))
        {
            if (dic.ContainsKey(startlv))
            {
                data = dic[startlv];
            }
        }
        return data;
    }
    #endregion

    #region 任务奖励配置
    private Dictionary<int, TaskRewardCfg> taskRewardDic = new Dictionary<int, TaskRewardCfg>();
    private void InitTaskRewardCfg()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(@"C:\ResCfgs\taskreward.xml");

        XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

        for (int i = 0; i < nodLst.Count; i++)
        {
            XmlElement ele = nodLst[i] as XmlElement;

            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
            TaskRewardCfg trc = new TaskRewardCfg
            {
                ID = ID
            };

            foreach (XmlElement e in nodLst[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "gold":
                        trc.gold = int.Parse(e.InnerText);
                        break;
                    case "exp":
                        trc.exp = int.Parse(e.InnerText);
                        break;
                    case "count":
                        trc.count = int.Parse(e.InnerText);
                        break;
                }
            }
            taskRewardDic.Add(ID, trc);
        }
        PECommon.Log("TaskRewardCfg InitDone");
    }
    public TaskRewardCfg GetTaskRewardCfg(int id)
    {
        TaskRewardCfg trc = null;
        if (taskRewardDic.TryGetValue(id, out trc))
        {
            return trc;
        }
        return null;
    }

    #endregion

    #region 副本战斗配置
    private Dictionary<int,MapCfg > mapCfgDic = new Dictionary<int, MapCfg>();
    private void InitMapCfg()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(@"C:\ResCfgs\map.xml");

        XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

        for (int i = 0; i < nodLst.Count; i++)
        {
            XmlElement ele = nodLst[i] as XmlElement;

            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
            MapCfg data = new MapCfg
            {
                ID = ID
            };

            foreach (XmlElement e in nodLst[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "power":
                        data.power = int.Parse(e.InnerText);
                        break;
                    case "coin":
                        data.gold = int.Parse(e.InnerText);
                        break;
                    case "exp":
                        data.exp = int.Parse(e.InnerText);
                        break;
                    case "crystal":
                        data.crystal = int.Parse(e.InnerText);
                        break;                
                }
            }
            mapCfgDic.Add(ID, data);
        }
        PECommon.Log("MapCfg InitDone");
    }
    public MapCfg GetMapCfg(int id)
    {
        MapCfg data = null;
        if (mapCfgDic.TryGetValue(id, out data))
        {
            return data;
        }
        return null;
    }

    #endregion
}






#region Data Define
public class BaseData<T>
{
    public int ID;
}
public class GuideCfg : BaseData<GuideCfg>
{
    public int gold;
    public int exp;
}
public class StrongCfg : BaseData<StrongCfg>
{
    public int pos;
    public int starlv;
    public int addhp;
    public int addhurt;
    public int adddef;
    public int minlv;
    public int gold;
    public int crystal;
}
public class TaskRewardCfg : BaseData<TaskRewardCfg>
{
    public int gold;
    public int exp;
    public int count;
}
public class TaskRewardState : BaseData<TaskRewardState>
{
    public int prgs;
    public bool taked;
}

public class MapCfg : BaseData<MapCfg>
{
    public int power;
    public int gold;
    public int exp;
    public int crystal;
}

#endregion

