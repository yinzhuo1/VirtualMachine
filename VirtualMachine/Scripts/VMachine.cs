using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace VirutalMachine
{
    public interface IVMEntity
    {
        //VMachine GetVMachine();
        GameObject GetAttachObj();
        void MoveToPos(Vector3 targetPos);

    }

    //虚拟机 整个虚拟机靠事件驱动
    public class VMachine : MonoBehaviour
    {
        public static int VMBranch_NONE = -1;
        public static int VMBranch_SUCCESS = 0;
        public static int VMBranch_FAILED = 1;
        public static int VMBranch_COUNT = 2;

        private VMData m_vmData;
        private IVMEntity m_owner;

        private int m_aiType;


        public VMState m_curState;

        //虚拟机专属的定时响应事件 目前定义三个 根据需求可以继续扩张
        private float firstTimerFrame = 0;
        private float secondTimerFrame = 0;
        private float thirdTimerFrame = 0;

        public VMachine()
        {

        }

        private void Awake()
        {

        }

        public void Init(IVMEntity owner, int nAIType)
        {
            GameDebug.Assert(owner != null);

            m_owner = owner;
            m_aiType = nAIType;

            m_vmData = VMManager.GetInstance().GetVMData(m_aiType);

            GameDebug.Assert(m_vmData != null, "VMachine Init Error m_aiType = " + m_aiType);

            m_curState = m_vmData.GetState(m_vmData.m_initStateID);
            GameDebug.Assert(m_curState != null, "VMachine Init Error Init state ID = " + m_vmData.m_initStateID);

            //默认从第一个定时器事件开始启动
            FireEvent((int)VMEventType.vmeFirstTimer);

        }

        private void Update()
        {
            //定时器检测
           if(firstTimerFrame != 0 && firstTimerFrame <= Time.realtimeSinceStartup)
           {
               firstTimerFrame = 0;
               FireEvent((int)VMEventType.vmeFirstTimer);
           }

           if (secondTimerFrame != 0 && secondTimerFrame <= Time.realtimeSinceStartup)
           {
               secondTimerFrame = 0;
               FireEvent((int)VMEventType.vmeSecondTimer);
           }

           if (thirdTimerFrame != 0 && thirdTimerFrame <= Time.realtimeSinceStartup)
           {
               thirdTimerFrame = 0;
               FireEvent((int)VMEventType.vmeThirdTimer);
           }
        }


        public void FireEvent(int nEvent)
        {
            GameDebug.Assert(m_vmData != null);
            GameDebug.Assert(m_curState != null);

            VMActionHandle actionHandle = m_curState.GetEventHandle(nEvent);
            if(actionHandle == null)
            {
                return;
            }

            int nBranchIndex = 0;
            int nCallCount = 0;
            while (actionHandle != null && actionHandle.m_actionID > VMAction.ACTION_ID_NONE)
            {
                VMActionHandle nextActionHandle = null;

                //防止进入递归
                if(nCallCount > 64)
                {
                    Logger.Error("VMachine FireEvent Error acitonID = ", actionHandle.m_actionID);
                    break;
                }

                nextActionHandle = m_vmData.CallAction(this, m_owner, actionHandle, ref nBranchIndex);

                if(nextActionHandle == null)
                {
                    actionHandle = null;
                }
                else
                {
                    actionHandle = nextActionHandle;
                    
                }

                nCallCount++;

            }

        }


        //状态切换
        public bool GoToState(int nState)
        {
            GameDebug.Assert(m_vmData != null);

            VMState state = m_vmData.GetState(nState);
            if(state == null)
            {
                Logger.Error("VMachine GoToState Error nState = {0}", nState);
                return false;
            }

            m_curState = state;

            return true;

        }

        //设置一个定时器事件
        public void SetFirstTimer(float fTime)
        {
            firstTimerFrame = fTime + Time.realtimeSinceStartup;
        }

        public void SetSecondTimer(float fTime)
        {
            secondTimerFrame = fTime + Time.realtimeSinceStartup;
        }

        public void SetThirdTimer(float fTime)
        {
            thirdTimerFrame = fTime + Time.realtimeSinceStartup;
        }
    }
}

