using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace VirutalMachine
{
    //虚拟机的 配置数据
    [Serializable]
    public class VMData// : ScriptableObject
    {
        //Only For Serialized
        [SerializeField]
        private List<VMAction> m_actionList = new List<VMAction>();

        [NonSerialized]
        public Dictionary<int, VMAction> m_actionDic = new Dictionary<int, VMAction>();

        //Only For Serialized
        [SerializeField]
        public List<VMState> m_stateList = new List<VMState>();

        [NonSerialized]
        public Dictionary<int, VMState> m_stateDic = new Dictionary<int, VMState>();

        [SerializeField]
        public int m_aiType;

        [SerializeField]
        public int m_initStateID;

        public VMData()
        {
            
        }


        public VMAction GetAction(int nActionID)
        {
            VMAction action = null;
            if(m_actionDic.TryGetValue(nActionID, out action))
            {
                return action;
            }

            return null;
        }

        public VMState GetState(int nState)
        {
            VMState state = null;
            if(m_stateDic.TryGetValue(nState, out state))
            {
                return state;
            }

            return null;
        }

        public VMActionHandle CallAction(VMachine machine, IVMEntity entity, VMActionHandle handle, ref int nBranchIndex)
        {
            VMActionHandle nextActionHandle = new VMActionHandle();

             nBranchIndex = -1;
            VMAction action = handle.m_action;
            if(action == null)
            {
                action = GetAction(handle.m_actionID);
                handle.m_action = action;
            }

            if (action == null)
            {
                Logger.Error("Invalid Action Call actionID = {0}, aiType = {1}", handle.m_actionID, m_aiType);
                return nextActionHandle;
            }

            nBranchIndex = VMActioinFun.GetInstance().ExeAction(machine, action, entity);
            int nBranchCount = action.m_branchIDs.Count;
            if (nBranchIndex > VMachine.VMBranch_NONE && nBranchIndex < nBranchCount)
            {
                int nNextActionID = action.m_branchIDs[nBranchIndex];
                VMAction nextAction = null;
                if(nNextActionID > 0)
                {
                    if (action.m_branchActions == null)
                    {
                        action.m_branchActions = new List<VMAction>(nBranchCount);
                        for(int i = 0; i < nBranchCount; i++)
                        {
                            action.m_branchActions.Add(GetAction(action.m_branchIDs[i]));
                        }
                    }

                    nextAction = action.m_branchActions[nBranchIndex];
                }
                 

                if(nextAction != null)
                {
                    nextActionHandle.m_actionID = nNextActionID;
                    nextActionHandle.m_action = nextAction;
                }

                

            }

            return nextActionHandle;
        }

        public VMAction AddAction(int nActionID, int nActionKey)
        {
            bool bExist = VMActioinFun.GetInstance().IsActionExist(nActionKey);
            if(!bExist)
            {
                Logger.Error("VMData AddAction Error nActionKey = {0}", nActionKey);
                return null;
            }

            if(m_actionDic.ContainsKey(nActionID))
            {
                Logger.Error("VMData Already Has A ActionID = {0}", nActionID);
                return null;
            }

            VMAction action = new VMAction();
            action.m_ID = nActionID;
            action.m_actionType = nActionKey;

            m_actionDic.Add(nActionID, action);

            //For Serialized Only
            m_actionList.Add(action);

            return action;

        }

        public VMState AddState(int stateID)
        {
            if(m_stateDic.ContainsKey(stateID))
            {
                Logger.Error("VMData AddState Duplicate StateID = {0}", stateID);
                return null;
            }

            VMState state = new VMState(this, stateID);

            m_stateDic.Add(stateID, state);

            //For Serialized Only
            m_stateList.Add(state);

            return state;

        }

        //将序列化的数据同步到 对象上(因为Dictionary 不能序列化 所以用List 代替 所以反序列化时候需要恢复)
        public void SyncSerializedData()
        {
            m_stateDic = new Dictionary<int, VMState>();
            m_actionDic = new Dictionary<int, VMAction>();

            int actionID = 0;
            foreach (VMAction action in m_actionList)
            {
                actionID = action.m_ID;
                m_actionDic.Add(actionID, action);
            }

            int stateID = 0;
            foreach (VMState state in m_stateList)
            {
                stateID = state.GetStateID();
                state.SyncSerializedData();

                m_stateDic.Add(stateID, state);
            }
        }

    }
}

