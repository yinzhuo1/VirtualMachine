using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace VirutalMachine
{
    [Serializable]
    public class VMEventHandle
    {
        [SerializeField]
        private int m_event;
        [SerializeField]
        private int m_actionID;

        [NonSerialized]
        private VMActionHandle m_actoionHandle;

        public VMEventHandle(int nEvent, int actionID, VMAction action)
        {
            m_event = nEvent;
            m_actionID = actionID;

            m_actoionHandle = new VMActionHandle();

            m_actoionHandle.m_actionID = actionID;
            m_actoionHandle.m_action = action;
        }

        public void SetActionHandle(VMAction action)
        {
            if (m_actoionHandle == null)
            {
                m_actoionHandle = new VMActionHandle();
                m_actoionHandle.m_actionID = m_actionID;
            }
            m_actoionHandle.m_action = action;
        }

        public VMActionHandle GetActionHandle()
        {
            if (m_actoionHandle == null)
            {
                m_actoionHandle = new VMActionHandle();
                m_actoionHandle.m_actionID = m_actionID;
            }
            return m_actoionHandle;
        }

        public int GetEvent()
        {
            return m_event;
        }
    }

    //虚拟机的状态 一个状态可以包含多个 Action（具体要执行的动作）
    [Serializable]
    public class VMState
    {
        [NonSerialized]
        private VMData m_vmData;

        [SerializeField]
        private int m_stateID;

        //Only For Serialized
        [SerializeField]
        private List<VMEventHandle> m_eventHandleList = new List<VMEventHandle>();

        [NonSerialized]
        private Dictionary<int, VMEventHandle> m_eventHandleDic = new Dictionary<int, VMEventHandle>();

        public VMState(VMData data, int stateID)
        {
            m_vmData = data;
            m_stateID = stateID;
        }

        public int GetStateID()
        {
            return m_stateID;
        }

        public void SyncSerializedData()
        {
            m_eventHandleDic = new Dictionary<int, VMEventHandle>();
            foreach (VMEventHandle handle in m_eventHandleList)
            {
                m_eventHandleDic.Add(handle.GetEvent(), handle);
            }
        }


        //注册一个事件
        public void AddEventHandle(int nEvent, int nActionID)
        {
            //Logger.Assert(nEvent > 0);

            VMEventHandle eventHanlde = null;
            if (m_eventHandleDic.TryGetValue(nEvent, out eventHanlde))
            {
                Logger.Error("VMState AddEventHandle Error Already Register a nEvent = ", nEvent);
            }
            else
            {
                eventHanlde = new VMEventHandle(nEvent, nActionID, null);
                m_eventHandleDic.Add(nEvent, eventHanlde);

                m_eventHandleList.Add(eventHanlde);
            }
        }

        public VMActionHandle GetEventHandle(int nEvent)
        {
            VMEventHandle eventHanlde = null;
            if (m_eventHandleDic.TryGetValue(nEvent, out eventHanlde))
            {
                VMActionHandle actionHandle = eventHanlde.GetActionHandle();
                if (actionHandle == null)
                {
                    Logger.Error("GetEventHandle action Handle Is NIL nEvent = {0}", nEvent);
                }
                return actionHandle;
            }

            return null;
        }

       
    }
}

