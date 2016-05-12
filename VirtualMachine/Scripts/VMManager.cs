using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;


namespace VirutalMachine
{
    public enum VMEventType
    {
        vmeFirstTimer = 0,
        vmeSecondTimer,
        vmeThirdTimer,
        vmeBeAttack,
        vmeTotal,

    }

    public enum VMActionType
    {
        vmaNone = 0,
        vmaGoToState = 1,
        vmaSetFirstTimer,
        vmaSetSencodTimer,
        vmaSetThirdTimer,
        vmaSetRandomTimer,
        vmaMoveToPos,
        vmaPatrol,

        vmaTotal,
    }


    //描述(编辑器中使用)
    public static class VMEventTypeDes
    {
        public static string [] DesArray = 
        {
            "第一个定时器事件",
            "第二个定时器事件",
            "第三个定时器事件",
            "被攻击"
        };

    }

    public static class VMActionTypeDes
    {
        public static string[] DesArray =
        {
            "无",
            "切换状态",
            "设置第一个定时器",
            "设置第二个定时器",
            "设置第三个定时器",
            "设置随机定时器",
            "移动到某个点",
            "徘徊"
        };
    }


    //具体的执行方法
    public class VMActioinFun
    {

        //执行Action的方法
        public delegate int ActionFunDelegate(VMachine machine, IVMEntity entity, VMAction action);

         public Dictionary<int, ActionFunDelegate> m_actionFunDic = new Dictionary<int, ActionFunDelegate>();

        private VMActioinFun()
        {
            AddFun(VMActionType.vmaNone, NonOp);
            AddFun(VMActionType.vmaGoToState, GoToState);
            AddFun(VMActionType.vmaSetFirstTimer, SetFirstTimer);
            AddFun(VMActionType.vmaSetSencodTimer, SetSecondTimer);
            AddFun(VMActionType.vmaSetThirdTimer, SetThirdTimer);
            AddFun(VMActionType.vmaSetRandomTimer, SetRandomTimer);
            AddFun(VMActionType.vmaMoveToPos, MoveToPos);
            AddFun(VMActionType.vmaPatrol, Patrol);

        }

      

        private static VMActioinFun _instance;
        public static VMActioinFun GetInstance()
        {
            if(_instance == null)
            {
                _instance = new VMActioinFun();
            }

            return _instance;
        }

        private void AddFun(VMActionType actionType, ActionFunDelegate d)
        {
            m_actionFunDic.Add((int)actionType, d);
        }

         private readonly string szErrorMessage = "ExeAction Error, Event:{0}, Error:{1}, {2}";
        public int ExeAction(VMachine machine, VMAction action, IVMEntity entity)
        {
            int nBranchIndex = VMachine.VMBranch_NONE;
            try
            {
                ActionFunDelegate handler = null;
                if (m_actionFunDic.TryGetValue(action.m_actionType, out handler))
                {
                    nBranchIndex = handler(machine, entity, action);
                }
            }
            catch (System.Exception ex)
            {
                Logger.Error(szErrorMessage, action.m_ID, ex.Message, ex.StackTrace);
            }

            return nBranchIndex;
        }

        public bool IsActionExist(int nActionKey)
        {
            ActionFunDelegate handler = null;
            if (m_actionFunDic.TryGetValue(nActionKey, out handler))
            {
                return true;
            }

            return false;
        }

        //空操作
        public static int NonOp(VMachine machine, IVMEntity entity, VMAction action)
        {
            return VMachine.VMBranch_NONE;
        }

        //切换状态
        public static int GoToState(VMachine machine, IVMEntity entity, VMAction action)
        {
            int nStateID = (int)action.m_params[0];
            machine.GoToState(nStateID);


            return VMachine.VMBranch_NONE;
        }

        //设置第一个定时器
        public static int SetFirstTimer(VMachine machine, IVMEntity entity, VMAction action)
        {
            int fTime = (int)action.m_params[0];
            machine.SetFirstTimer(fTime);

            return VMachine.VMBranch_SUCCESS;
        }

        //设置第二个定时器
        public static int SetSecondTimer(VMachine machine, IVMEntity entity, VMAction action)
        {
            float fTime = (float)action.m_params[0];
            machine.SetSecondTimer(fTime);

            return VMachine.VMBranch_SUCCESS;
        }

        //设置第三个定时器
        public static int SetThirdTimer(VMachine machine, IVMEntity entity, VMAction action)
        {
            float fTime = (float)action.m_params[0];
            machine.SetThirdTimer(fTime);

            return VMachine.VMBranch_SUCCESS;
        }

        //设置随机定时器
        public static int SetRandomTimer(VMachine machine, IVMEntity entity, VMAction action)
        {
            float fMin = (float)action.m_params[0];
            float fMax = (float)action.m_params[1];

            float fTime = UnityEngine.Random.Range(fMin, fMax);

            machine.SetFirstTimer(fTime);

            return VMachine.VMBranch_SUCCESS;
        }

        public static int MoveToPos(VMachine machine, IVMEntity entity, VMAction action)
        {
            float x = (int)action.m_params[0];
            float y = (int)action.m_params[1];
            float z = (int)action.m_params[2];

            entity.MoveToPos(new Vector3(x, y, z));

            return VMachine.VMBranch_SUCCESS;
        }

        public static int Patrol(VMachine machine, IVMEntity entity, VMAction action)
        {
            return VMachine.VMBranch_SUCCESS;
        }

        
    }

    //所有AI配置数据
    public class VMManager : MonoBehaviour
    {
        private static VMManager m_instance;
        private static readonly object m_lock = new object();
        public static VMManager GetInstance()
        {
            if (m_instance == null)
            {
                lock (m_lock)
                {
                    if (m_instance == null)
                    {

                        GameObject obj = GameObject.Find("_VMManager");
                        if(obj == null)
                        {
                            obj = new GameObject("_VMManager");

                            m_instance = obj.AddComponent<VMManager>();
                        }
                        else
                        {
                            m_instance = obj.GetOrAddComponent<VMManager>();
                        }
                       
                    }

                }
            }

            return m_instance;

        }

       

        

        public Dictionary<int, VMData> m_vmDataDic = new Dictionary<int, VMData>();
       

        public VMManager()
        {

        }

        private void Awake()
        {
            DontDestroyOnLoad(this);

            LoadVMData("1");
        }

        private void Start()
        {
            
        }

        public void initialize(XmlDocument xmlVM)
        {
            XmlNode root = xmlVM.SelectSingleNode("root");
            XmlNodeList nodelist = root.SelectNodes("item");

            for (int i = 0; i < nodelist.Count; ++i)
            {
                int id = int.Parse(nodelist[i].Attributes["id"].Value);
                string strPath = nodelist[i].Attributes["path"].Value;

                //ResourceManagerBase.instance.
            }
        }

        public VMData GetVMData(int nAIType)
        {
            VMData data = null;
            if(m_vmDataDic.TryGetValue(nAIType, out data))
            {
                return data;
            }

            return null;
        }

        

        public void LoadAllVMData()
        {

        }

        public void LoadVMData(string name)
        {
            string savePath =  Application.dataPath + "/RawAssets/VirtualMachine/" + name + ".asset";

            if (!File.Exists(savePath))
            {
                return;
            }

            //string assetPath = VMConfig.GetVMDataAssetsEditPath(m_vmDataName);
            //Object o = AssetDatabase.LoadAssetAtPath(assetPath, typeof(VMDataEidtor));
            VMData s = null;
            BinaryFormatter binaryFormate = new BinaryFormatter();
            FileStream fileStream = File.OpenRead(savePath);
            try
            {
                s = binaryFormate.Deserialize(fileStream) as VMData;
                s.SyncSerializedData();
                if (m_vmDataDic.ContainsKey(s.m_aiType))
                {
                    m_vmDataDic.Remove(s.m_aiType);
                }
                m_vmDataDic.Add(s.m_aiType, s);

            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            } 
            finally
            {
                fileStream.Close();
            }

           

            //m_vmDataEditor = AssetDatabase.LoadAssetAtPath(assetPath, typeof(VMDataEidtor)) as VMDataEidtor;
        }

        public void CreateVMachine(IVMEntity entity, int aiType)
        {
            VMData s = null;
            if(m_vmDataDic.TryGetValue(aiType, out s))
            {
                GameObject obj = entity.GetAttachObj();
                if(obj != null)
                {
                    VMachine machine = obj.GetOrAddComponent<VMachine>();
                    machine.Init(entity, aiType);
                }
                else
                {
                    Logger.Error("VMManager CreateVMachine entity Attack GameObject is nil ");
                }
                
            }
            else
            {
                Logger.Error("VMManager CreateVMachine is have no aiType = {0}", aiType.ToString());
            }
        }

       
    }
}

