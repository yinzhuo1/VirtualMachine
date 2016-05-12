using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace VirutalMachine
{
    //虚拟机具体执行的原子操作
    [Serializable]
    public class VMAction
    {
        public static int ACTION_ID_NONE = 0;

        //Actoin 的唯一Key值(在一个虚拟机实例中保证唯一性)
        [SerializeField]
        public int m_ID;
        [SerializeField]
        public int m_actionType = (int)VMActionType.vmaNone;
        //参数列表
        [SerializeField]
        public List<object> m_params;
        //执行的分支
        [SerializeField]
        public List<int> m_branchIDs;

        [NonSerialized]
        public List<VMAction> m_branchActions;
    }

    //TODO: 以后如无必要可以用Struct代替
    public class VMActionHandle
    {
        public VMActionHandle()
        {
            m_actionID = VMAction.ACTION_ID_NONE;
            m_action = null;
        }

        public int m_actionID;
        public VMAction m_action;
    }

   
}

