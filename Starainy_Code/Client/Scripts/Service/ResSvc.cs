/****************************************************
    文件：ResSvc.cs
	作者：Harmonie
	功能：资源加载服务
*****************************************************/

using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ResSvc : MonoBehaviour 
{ 
    public static ResSvc Instance = null;

    private Action prgCB = null;
    public void InitSvc()
    {
        Instance = this;
        InitRdNameCfgs(PathDefine.RdNameCfg);
        InitMonsterCfg(PathDefine.MonsterCfg);
     
        InitMapCfg(PathDefine.MapCfg);
       
        InitAutoGuideCfg(PathDefine.GuideCfg);
        InitStrongCfg(PathDefine.StrongCfg);
        InitTaskRewardCfg(PathDefine.TaskRewardCfg);

        InitSkillCfg(PathDefine.SkillCfg);
        InitSkillMoveCfg(PathDefine.SkillMoveCfg);
        InitSkillActionCfg(PathDefine.SkillActionCfg);
        PECommon.Log("Init ResSvc.........");       
    }

    public void ResetCfg()
    {
        skillCfgDic.Clear();
        InitSkillCfg(PathDefine.SkillCfg);
        skillMoveDic.Clear();
        InitSkillMoveCfg(PathDefine.SkillMoveCfg);

        PECommon.Log("Init ResetCfg.........");
    }
    #region 资源加载
    //异步加载资源
    public void AsyncLoadScene(string sceneName,Action loaded)//action loaded属于回调,用于不同的类进行加载资源场景的调用
    {
        //进入加载界面
        GameRoot.Instance.loadingPanel.SetPanelState();
      
        //加载场景并获取该操作
        AsyncOperation sceneAsync= SceneManager.LoadSceneAsync(sceneName);

        //对操作prgCB赋值
        prgCB = () =>
        {
            //获得当前加载进度
            float val = sceneAsync.progress;
            //调用LoadingPanel中的setProgress方法
            GameRoot.Instance.loadingPanel.SetProgress(val);
            //加载完成后的操作
            if (val == 1)
            {
                if (loaded != null)
                {
                    loaded();
                }
                prgCB = null;
                sceneAsync = null;
                GameRoot.Instance.loadingPanel.SetPanelState(false);//关闭加载页面
            }
        };
       
    }
    private void Update()
    {
        if (prgCB != null)
        {
            prgCB();
        }
    }
    //定义一个字典存储缓存的声音资源
    private Dictionary<string, AudioClip> adDict = new Dictionary<string, AudioClip>();
    public AudioClip LoadAudio(string path,bool cache = false)
    {
        AudioClip audioClip = null;
        if(!adDict.TryGetValue(path,out audioClip))
        {
            audioClip = Resources.Load<AudioClip>(path);
            if (cache)
            {
                adDict.Add(path, audioClip);
            }
        }
       
        return audioClip;
    }

    //人物资源的加载
    private Dictionary<string, GameObject> goDic = new Dictionary<string, GameObject>();
    public GameObject LoadPrefab(string path,bool cache = false)
    {
        GameObject prefab;
        if (!goDic.TryGetValue(path,out prefab))
        {
            prefab = Resources.Load<GameObject>(path);
            if (cache)
            {
                goDic.Add(path, prefab);
            }
        }
        GameObject go = null;
        if (prefab != null)
        {
            go = Instantiate(prefab);
        }
        return go;
    }

    //图片加载
    private Dictionary<string, Sprite> spDic = new Dictionary<string, Sprite>();
    public Sprite LoadSprite(string path,bool cache=false)
    {
        Sprite sp = null;
        if(!spDic.TryGetValue(path,out sp))
        {
            sp = Resources.Load<Sprite>(path);
            if (cache)
            {
                spDic.Add(path, sp);
            }
        }
        return sp;

    }
    #endregion
    #region InitCfgs
    #region 随机名字
    private List<string> surnameLst = new List<string>();
    private List<string> manLst = new List<string>();
    private List<string> womanLst = new List<string>();
    //初始化xml数据信息
    private void InitRdNameCfgs(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml File" + path + "not exist",LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;
            //遍历获取ID号
            for (int i= 0;i < xmlNodeList.Count; i++){
                XmlElement xmlElement = xmlNodeList[i] as XmlElement;
                if (xmlElement.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32( xmlElement.GetAttributeNode("ID").InnerText);

                //遍历获取每个节点的数据
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "surname":
                            surnameLst.Add(e.InnerText);
                            break;
                        case "man":
                            manLst.Add(e.InnerText);
                            break;
                        case "woman":
                            womanLst.Add(e.InnerText);
                            break;
                    }
                }
            }

        }
    }
    //获取相应信息
    public string GetNameData(bool isMan=true)
    {
        System.Random rd = new System.Random();
        string rdName = surnameLst[PETools.RDInt(0, surnameLst.Count - 1)];
        if (isMan == true)
        {
            rdName += manLst[PETools.RDInt(0, manLst.Count - 1)];
        }
        else
        {
            rdName += womanLst[PETools.RDInt(0, womanLst.Count - 1)];
        }
        return rdName;
    }
    #endregion
    #region 地图信息
    private Dictionary<int, MapCfg> mapCfgDataDict = new Dictionary<int, MapCfg>();
    private void InitMapCfg(string path)
    {
      
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml File" + path + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

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
                MapCfg mc = new MapCfg
                {
                    ID = ID,
                    monterLst = new List<MonsterData>(),
                };
                //遍历获取每个节点的数据
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "mapName":
                            mc.mapName = e.InnerText;
                            break;
                        case "sceneName":
                            mc.sceneName = e.InnerText;
                            break;
                        case "power":
                            mc.power = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            mc.gold = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            mc.exp = int.Parse(e.InnerText);
                            break;
                        case "crystal":
                            mc.crystal = int.Parse(e.InnerText);
                            break;
                        case "mainCamPos":
                            {
                                string[] valAttr = e.InnerText.Split(',');
                                mc.mainCamPos = new Vector3(float.Parse(valAttr[0]), float.Parse(valAttr[1]), float.Parse(valAttr[2]));
                            }
                            break;
                        case "mainCamRote":
                            {
                                string[] valAttr = e.InnerText.Split(',');
                                mc.mainCamRote = new Vector3(float.Parse(valAttr[0]), float.Parse(valAttr[1]), float.Parse(valAttr[2]));
                            }
                            break;
                        case "playerBornPos":
                            {
                                string[] valAttr = e.InnerText.Split(',');

                                mc.playerBornPos = new Vector3(float.Parse(valAttr[0]), float.Parse(valAttr[1]), float.Parse(valAttr[2]));
                               

                            }
                            break;
                        case "playerBornRote":
                            {
                                string[] valAttr = e.InnerText.Split(',');
                                mc.playerBornRote = new Vector3(float.Parse(valAttr[0]), float.Parse(valAttr[1]), float.Parse(valAttr[2]));
                            }
                            break;
                        case "monsterLst":
                            {
                                string[] valArr = e.InnerText.Split('#');
                                for(int wavIndex = 0; wavIndex<valArr.Length;wavIndex++)
                                {
                                    if (wavIndex == 0)
                                    {
                                        continue;
                                    }
                                    string[] tempArr = valArr[wavIndex].Split('|');
                                    for(int j = 0; j < tempArr.Length; j++)
                                    {
                                        if (j == 0)
                                        {
                                            continue;
                                        }
                                        string[] arr = tempArr[j].Split(',');
                                        MonsterData monsterData = new MonsterData
                                        {
                                            ID = int.Parse(arr[0]),
                                            mWave = wavIndex,
                                            mIndex = j,
                                            monsterCfg = GetMonsterCfg(int.Parse(arr[0])),
                                            mBornPos = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3])),
                                            mBornRotate=new Vector3(0, float.Parse(arr[4]),0),
                                            mLevel=int.Parse(arr[5]),
                                        };
                                        mc.monterLst.Add(monsterData);                                       
                                    }
                                }
                                
                                    
                            }
                            break;
                    }
                }

                mapCfgDataDict.Add(ID, mc);
            }

        }
    }
    public MapCfg GetMapCfg(int ID)
    {
        MapCfg data;
        if(mapCfgDataDict.TryGetValue(ID,out data))
        {
            return data;
        }
        else
        {
            return null;
        }
    }
    #endregion
    #region 自动任务导航信息
    private Dictionary<int, AutoGuideCfg> autoGuideCfgDict = new Dictionary<int, AutoGuideCfg>();
    private void InitAutoGuideCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml File" + path + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

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
                AutoGuideCfg guideCfg = new AutoGuideCfg
                {
                    ID = ID
                };
                //遍历获取每个节点的数据
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "npcID":
                            guideCfg.npcID = int.Parse(e.InnerText);
                            break;
                        case "dilogArr":
                            guideCfg.dilogArr = e.InnerText;
                            break;
                        case "actID":
                            guideCfg.actID = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            guideCfg.exp = int.Parse(e.InnerText);
                            break;
                        case "gold":
                            guideCfg.gold = int.Parse(e.InnerText);
                            break;
                    }
                }

                autoGuideCfgDict.Add(ID, guideCfg);
            }

        }
    }
    public AutoGuideCfg GetAutoGuideCfg(int ID)
    {
        AutoGuideCfg data;
        if(autoGuideCfgDict.TryGetValue(ID,out data))
        {
            return data;
        }
        else 
        {
            return null;
        }
    }
    #endregion
    #region 强化升级信息
    private Dictionary<int, Dictionary<int, StrongCfg>> strongDic = new Dictionary<int, Dictionary<int, StrongCfg>>();
    private void InitStrongCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml File" + path + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

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

        }
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
    public int GetPropAddValPreLv(int pos,int startLv,int type)
    {
        Dictionary<int, StrongCfg> posDic = null;
        int val = 0;
        if(strongDic.TryGetValue(pos,out posDic))
        {
            for(int i = 0; i< startLv; i++)
            {
                StrongCfg sd;
                if(posDic.TryGetValue(i,out sd))
                {
                    switch (type)
                    {
                        case 1://hp
                            val += sd.addhp;
                            break;
                        case 2://hurt
                            val += sd.addhurt;
                            break;
                        case 3://def
                            val += sd.adddef;
                            break;
                    }
                }
            }
        }
        return val;
    }
    #endregion
    #region 任务奖励信息
    private Dictionary<int, TaskRewardCfg> taskRewardDic = new Dictionary<int, TaskRewardCfg>();
    private void InitTaskRewardCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml File" + path + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

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
                TaskRewardCfg taskCfg = new TaskRewardCfg
                {
                    ID = ID
                };
                //遍历获取每个节点的数据
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "taskName":
                            taskCfg.taskName = e.InnerText.ToString();
                            break;
                        case "gold":
                            taskCfg.gold =int.Parse(e.InnerText);
                            break;
                        case "count":
                            taskCfg.count = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            taskCfg.exp = int.Parse(e.InnerText);
                            break;
                       
                    }
                }

                taskRewardDic.Add(ID, taskCfg);
            }

        }
    }
    public TaskRewardCfg GetTaskRewardCfg(int ID)
    {
        TaskRewardCfg data;
        if (taskRewardDic.TryGetValue(ID, out data))
        {
            return data;
        }
        else
        {
            return null;
        }
    }
    #endregion
    #region 技能配置信息
    private Dictionary<int, SkillCfg> skillCfgDic = new Dictionary<int, SkillCfg>();
    private void InitSkillCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml File" + path + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

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
                SkillCfg skillCfg = new SkillCfg
                {
                    ID = ID,
                    skillMoveLst = new List<int>(),
                    skillActionLst=new List<int>(),
                    skillDamageLst=new List<int>(),
                };
                //遍历获取每个节点的数据
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "skillName":
                            skillCfg.skillName = e.InnerText.ToString();
                            break;
                        case "skillTime":
                            skillCfg.skillTime = int.Parse(e.InnerText);
                            break;
                        case "aniAction":
                            skillCfg.aniAction = int.Parse(e.InnerText);
                            break;
                        case "cdTime":
                            skillCfg.cdTime = int.Parse(e.InnerText);
                            break;
                        case "fx":
                            skillCfg.fx = e.InnerText.ToString();
                            break;
                        case "isCombo":
                            if (e.InnerText.Equals("0"))
                            {
                                skillCfg.isCombo = false;
                            }
                            else if (e.InnerText.Equals("1"))
                            {
                                skillCfg.isCombo = true;
                            }
                            else
                            {
                                PECommon.Log("damage 设置错误");
                            }
                            break;
                        case "isCollide":
                            if (e.InnerText.Equals("0"))
                            {
                                skillCfg.isCollide = false;
                            }
                            else if (e.InnerText.Equals("1"))
                            {
                                skillCfg.isCollide = true;
                            }                     
                            break;
                        case "isBreak":
                            if (e.InnerText.Equals("0"))
                            {
                                skillCfg.isBreak = false;
                            }
                            else if (e.InnerText.Equals("1"))
                            {
                                skillCfg.isBreak = true;
                            }                      
                            break;
                        case "dmgType":
                            if (e.InnerText.Equals("1"))
                            {
                                skillCfg.damageType = DamageType.AD;
                            }else if (e.InnerText.Equals("2"))
                            {
                                skillCfg.damageType = DamageType.AP;
                            }
                            else
                            {
                                PECommon.Log("damage 设置错误");
                            }
                            break;
                        case "skillMoveLst":
                            string[] skMovArr = e.InnerText.Split('|');
                            for(int j = 0; j < skMovArr.Length; j++)
                            {
                                if (skMovArr[j] != null)
                                {
                                    skillCfg.skillMoveLst.Add(int.Parse(skMovArr[j]));
                                }
                            }
                            break;
                        case "skillActionLst":
                            string[] skActArr = e.InnerText.Split('|');
                            for (int j = 0; j < skActArr.Length; j++)
                            {
                                if (skActArr[j] != null)
                                {
                                    skillCfg.skillActionLst.Add(int.Parse(skActArr[j]));
                                }
                            }
                            break;
                        case "skillDamageLst":
                            string[] skDamageArr = e.InnerText.Split('|');
                            for (int j = 0; j < skDamageArr.Length; j++)
                            {
                                if (skDamageArr[j] != null)
                                {
                                    skillCfg.skillDamageLst.Add(int.Parse(skDamageArr[j]));
                                }
                            }
                            break;
                    }
                }

                skillCfgDic.Add(ID, skillCfg);
            }

        }
    }
    public SkillCfg GetSkillCfg(int ID)
    {
        SkillCfg data;
        if (skillCfgDic.TryGetValue(ID, out data))
        {
            return data;
        }
        else
        {
            return null;
        }
    }
    #endregion
    #region 技能效果配置
    private Dictionary<int, SkillActionCfg> skillActionCfgDic = new Dictionary<int, SkillActionCfg>();
    private void InitSkillActionCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml File" + path + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;       
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement xmlElement = xmlNodeList[i] as XmlElement;
                if (xmlElement.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(xmlElement.GetAttributeNode("ID").InnerText);
                SkillActionCfg data = new SkillActionCfg
                {
                    ID = ID,                   
                };              
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "delayTime":
                            data.delayTime = int.Parse(e.InnerText);
                            break;
                        case "radius":
                            data.radius = float.Parse(e.InnerText);
                            break;
                        case "angle":
                            data.angle = float.Parse(e.InnerText);
                            break;                        
                    }
                }

                skillActionCfgDic.Add(ID, data);
            }

        }
    }
    public SkillActionCfg GetSkillActionCfg(int ID)
    {
        SkillActionCfg data;
        if (skillActionCfgDic.TryGetValue(ID, out data))
        {
            return data;
        }
        else
        {
            return null;
        }
    }
    #endregion
    #region 技能位移信息
    private Dictionary<int, SkillMoveCfg> skillMoveDic = new Dictionary<int, SkillMoveCfg>();
    private void InitSkillMoveCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml File" + path + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

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
                SkillMoveCfg data = new SkillMoveCfg
                {
                    ID = ID
                };
                //遍历获取每个节点的数据
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "moveTime":
                            data.moveTime = int.Parse(e.InnerText);
                            break;
                        case "moveDis":
                            data.moveDis = float.Parse(e.InnerText);
                            break;
                        case "delayTime":
                            data.delayTime = int.Parse(e.InnerText);
                            break;
                    
                    }
                }

                skillMoveDic.Add(ID, data);
            }

        }
    }
    public SkillMoveCfg GetSkillMoveCfg(int ID)
    {
        SkillMoveCfg data;
        if (skillMoveDic.TryGetValue(ID, out data))
        {
            return data;
        }
        else
        {
            return null;
        }
    }
    #endregion
    #region 怪物配置信息
    private Dictionary<int, MonsterCfg> monsterDic = new Dictionary<int, MonsterCfg>();
    private void InitMonsterCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml File" + path + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

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
                MonsterCfg data = new MonsterCfg
                {
                    ID = ID,
                    battleProps=new BattleProps { },
                };
                //遍历获取每个节点的数据
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "mName":
                            data.mName = e.InnerText.ToString();
                            break;
                        case "resPath":
                            data.resPath = e.InnerText.ToString();
                            break;
                        case "mType":
                            if (e.InnerText.Equals("1"))
                            {
                                data.monsterType = MonsterType.Normal;
                            }else if (e.InnerText.Equals("2"))
                            {
                                data.monsterType = MonsterType.Boss;
                            }
                            break;
                        case "isStop":
                            data.isStop = e.InnerText.Equals("1");
                            break;

                        case "hp":
                            data.battleProps.hp = int.Parse(e.InnerText);
                            break;
                        case "ad":
                            data.battleProps.ad = int.Parse(e.InnerText);
                            break;
                        case "ap":
                            data.battleProps.ap = int.Parse(e.InnerText);
                            break;
                        case "addef":
                            data.battleProps.addef = int.Parse(e.InnerText);
                            break;
                        case "apdef":
                            data.battleProps.apdef = int.Parse(e.InnerText);
                            break;
                        case "crirical":
                            data.battleProps.critical = int.Parse(e.InnerText);
                            break;
                        case "pierce":
                            data.battleProps.pierce = int.Parse(e.InnerText);
                            break;
                        case "dodge":
                            data.battleProps.dodge = int.Parse(e.InnerText);
                            break;
                        case "atkDis":
                            data.atkDis = float.Parse(e.InnerText);
                            break;
                        case "skillID":
                            data.skillID = int.Parse(e.InnerText);
                            break;
                    }
                }

                monsterDic.Add(ID, data);
            }

        }
    }
    public MonsterCfg GetMonsterCfg(int ID)
    {
        MonsterCfg data;
        if (monsterDic.TryGetValue(ID, out data))
        {
            return data;
        }
        else
        {
            return null;
        }
    }
    #endregion
    #endregion
}