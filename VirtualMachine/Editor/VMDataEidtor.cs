using UnityEngine;
using System.Collections;
using VirutalMachine;
using UnityEditor;
using System.Collections.Generic;
using System;


namespace VirutalMachine
{
    [Serializable]
    public class VMDataEidtor
    {
        //[SerializeField]
        //public VMData m_vmData;
        public int globalIndex = 0;
        
        public Dictionary<int, VMStateEditor> m_vmStateDic = new Dictionary<int, VMStateEditor>();

        public Dictionary<int, VMActionEditor> m_vmActionDic = new Dictionary<int, VMActionEditor>();

        public int m_initStateID = 0;


        private VMBaseNodeEditor m_selectNode = null;

        private VMNodeInspector m_vmNodeInstector;

       

        //public string m_vmDataName = "1";

        public VMDataEidtor()
        {
            m_vmNodeInstector = new VMNodeInspector();
        }

        private int GetUniqID()
        {
            globalIndex++;
            return globalIndex;
        }
        
        public VMActionEditor GetAction(int id)
        {
            VMActionEditor action = null;
            if(m_vmActionDic.TryGetValue(id, out action))
            {
                return action;
            }

            return null;
        }

        public VMStateEditor GetState(int id)
        {
            VMStateEditor state = null;
            if(m_vmStateDic.TryGetValue(id, out state))
            {
                return state;
            }
            return state;

        }

        public VMActionEditor CreateAction()
        {
            int curentIndex = GetUniqID();
            VMActionEditor action = new VMActionEditor(this, curentIndex);
             
             m_vmActionDic.Add(curentIndex, action);


            return action;
        }

        public void DeleteAction(VMActionEditor action)
        {
            m_vmActionDic.Remove(action.m_ID);
            foreach (VMStateEditor state in m_vmStateDic.Values)
            {
                foreach (KeyValuePair<VMEventType, int> kvp in state.m_eventHandleDic)
                {
                    if(kvp.Value == action.m_ID)
                    {
                        state.m_eventHandleDic.Remove(kvp.Key);
                        break;
                    }
                }
                
            }
        }

        public VMStateEditor CreateState()
        {
            int curentIndex = GetUniqID();
            VMStateEditor state = new VMStateEditor(this, curentIndex);

           
            m_vmStateDic.Add(curentIndex, state);

            return state;
        }

        public void DeleteState(VMStateEditor state)
        {
            m_vmStateDic.Remove(state.m_ID);

            foreach (KeyValuePair<VMEventType, int> kvp in state.m_eventHandleDic)
            {
                m_vmActionDic.Remove(kvp.Value);
            }
        }

        //序列化
        public void Serialize(VMDataSerialize s)
        {
            s.Serialize(this);
        }

        public void Deserialize(VMDataSerialize s)
        {
            s.Deserialize(this);

            //更新globalIndex
            foreach (VMStateEditor state in m_vmStateDic.Values)
            {

                if(state.m_ID > globalIndex)
                {
                    globalIndex = state.m_ID + 1;
                }
            }

            foreach (VMActionEditor action in m_vmActionDic.Values)
            {
                if(action.m_ID > globalIndex)
                {
                    globalIndex = action.m_ID + 1;
                }
            }
        }

        //存储数据
        public void SaveData()
        {
            //VMData m_vmData = ScriptableObject.CreateInstance<VMData>();

            VMData data = new VMData();

            data.m_aiType = int.Parse(VMWindow.m_vmDataName);
            data.m_initStateID = m_initStateID;

            foreach (VMStateEditor state in m_vmStateDic.Values)
            {

                VMState s = data.AddState(state.m_ID);
                state.SaveData(s);
            }

            foreach(VMActionEditor action in m_vmActionDic.Values)
            {
                VMAction a = data.AddAction(action.m_ID, (int)action.actionType);
                action.SaveData(a);
            }

            VMWindow.SaveData(VMWindow.m_vmDataName, data, false);

        }

        //读取数据
        public void LoadData()
        {
            //VMData data = VMWindow.LoadData(m_vmDataName, false) as VMData;
        }

        public void OnEnable()
        {

            if(m_vmNodeInstector == null)
            {
                m_vmNodeInstector = ScriptableObject.CreateInstance<VMNodeInspector>();
            }

        }

       

        public void OnGUI()
        {
            GUI.enabled = !EditorApplication.isCompiling;

            //Handles.DrawLine(new Vector3(100, 100, 0), Event.current.mousePosition)

            //m_vmDataName = GUILayout.TextField("名称：", m_vmDataName);
            //if(GUILayout.Button("LoadVM"))
            //{
            //    LoadData();
            //}

            //if(GUILayout.Button("SaveVM"))
            //{
            //    SaveData();
            //}

            //if(GUILayout.Button("AddState"))//, new GUILayoutOption[]{GUILayout.Width(140f)}))
            //{
            //    CreateState();
            //}

            //初始状态(为了方便起见，初始状态在一开始创建就指定死了， 不给接口修改)
            //DrawInitState();
            //SetUpSize();
            GUILayout.BeginArea(VMWindow.m_graphRect);

            //GUI.BeginScrollView(m_graphRect, new Vector2(m_graphRect.x, m_graphRect.y), m_graphRect, true, true);

            foreach (VMStateEditor state in m_vmStateDic.Values)
            {
                state.Draw();
            }

            foreach (VMActionEditor action in m_vmActionDic.Values)
            {
                action.Draw();
            }

            //GUI.EndScrollView();
            GUILayout.EndArea();

            

            //GUI.BeginScrollView(m_inspectorRect, new Vector2(m_inspectorRect.x, m_inspectorRect.y), m_inspectorRect, true, true);
            GUILayout.BeginArea(VMWindow.m_inspectorRect);

            m_vmNodeInstector.DrawNodeInspector(m_selectNode);

            GUILayout.EndArea();
            //GUI.EndScrollView();



            HandleEvents();
        }

        private int m_selectInitStateIndex = 0;
        private void DrawInitState()
        {

            //GUILayout.Label("成功分支:");

            List<int> stateIDList = GetStateIDList(0);
            //actionIDList.Insert(0, VMAction.ACTION_ID_NONE);
            int count = stateIDList.Count;

            if (count > 0)
            {
                string[] stateNameList = new string[count];
                for (int i = 0; i < count; i++)
                {
                    if (stateIDList[i] == m_initStateID)
                    {
                        m_selectInitStateIndex = i;
                    }

                    VMStateEditor state = GetState(stateIDList[i]);
                    stateNameList[i] = state.GetName();


                }

                //容错处理
                if (m_selectInitStateIndex >= count)
                {
                    Debug.LogError("selected is to large = " + m_selectInitStateIndex + "count = " + count);
                    m_selectInitStateIndex = 0;

                }

                m_selectInitStateIndex = EditorGUILayout.Popup("初始状态", m_selectInitStateIndex, stateNameList);
                m_initStateID = stateIDList[m_selectInitStateIndex];
            }
        }


        private void HandleEvents()
        {
            switch(Event.current.type)
            {
                case EventType.MouseDown:
                    {
                        if(Event.current.button == 0)
                        {
                            if(LeftmouseDown())
                            {
                                Event.current.Use();
                            }
                        }
                        else
                        {
                            if(RightMouseDown())
                            {
                                Event.current.Use();
                            }
                        }
                        break;
                    }
                case EventType.MouseDrag:
                    {
                        if(Event.current.button == 0)
                        {
                            //Left Button
                            if(LeftMouseDragged())
                            {
                                Event.current.Use();
                            }

                        }
                        else
                        {
                            //right Button
                        }
                        break;
                    }
            }
        }

        private bool GetMousePositionInGraph(out Vector2 mousePostion)
        {
            mousePostion = Event.current.mousePosition;
            if (!VMWindow.m_graphRect.Contains(mousePostion))
            {
                return false;
            }
            else
            {
                mousePostion -= new Vector2(VMWindow.m_graphRect.xMin, VMWindow.m_graphRect.yMin);
                return true;
            }
        }

        private bool LeftmouseDown()
        {
            if (m_selectNode != null)
            {
                m_selectNode.UnSelect();
            }
            m_selectNode = null;
            Vector2 mousePosition = Vector3.zero;
            if(GetMousePositionInGraph(out mousePosition))
            {
                foreach (VMStateEditor state in m_vmStateDic.Values)
                {
                    if (state.Select(mousePosition))
                    {
                        m_selectNode = state;
                        break;
                    }
                }

                foreach (VMActionEditor action in m_vmActionDic.Values)
                {
                    if (action.Select(mousePosition))
                    {
                        m_selectNode = action;
                        break;
                    }
                }
            }
           

            return m_selectNode != null;
        }

       private bool LeftMouseDragged()
       {
           if(m_selectNode == null)
           {
               return false;
           }

           Vector3 dragDelta = Event.current.delta;
           if (m_selectNode.DragNode(dragDelta))
           {
               return true;
           }

           return true;
       }

       private bool RightMouseDown()
       {
           //Vector3 mousePosition = Event.current.mousePosition;

           Vector2 mousePosition = Vector3.zero;
           if (GetMousePositionInGraph(out mousePosition))
           {
               foreach (VMStateEditor state in m_vmStateDic.Values)
               {
                   if (state.Select(mousePosition))
                   {
                       DeleteNode(state);
                       break;
                   }
               }

               foreach (VMActionEditor action in m_vmActionDic.Values)
               {
                   if (action.Select(mousePosition))
                   {
                       DeleteNode(action);
                       break;
                   }
               }
           }

           

           return true;
       }

        private void DeleteNode(VMBaseNodeEditor node)
        {
            bool bDelete = EditorUtility.DisplayDialog("Warning", "是否确定删除当前节点 " + node.GetName(), "OK", "Cancle");
            if(bDelete)
            {
                VMStateEditor state = node as VMStateEditor;
                if(state != null)
                {
                    DeleteState(state);
                }

                VMActionEditor action = node as VMActionEditor;
                if(action != null)
                {
                    DeleteAction(action);
                }
            }
        }

        
        public List<int> GetActionIDList(int expectID)
        {
            List<int> actionIDList = new List<int>();

            foreach(int key in m_vmActionDic.Keys)
            {
                if(key != expectID)
                {
                    actionIDList.Add(key);
                }
            }

            return actionIDList;
        }

        public List<int> GetStateIDList(int expectID)
        {

            List<int> stateIDList = new List<int>();

            foreach (int key in m_vmStateDic.Keys)
            {
                if (key != expectID)
                {
                    stateIDList.Add(key);
                }
            }

            return stateIDList;
        }

    }


    //辅助 用来序列化得类
    [Serializable]
    public class VMActionSerialize
    {
        [SerializeField]
        public int m_ID;
        [SerializeField]
        public string name = "NodeName";

        [SerializeField]
        public float m_vTop;
        [SerializeField]
        public float m_vLeft;
        [SerializeField]
        public float m_width = 100;
        [SerializeField]
        public float m_height = 100;

        [SerializeField]
        public VMActionType actionType = VMActionType.vmaNone;
        [SerializeField]
        public int success = VMAction.ACTION_ID_NONE;
         [SerializeField]
        public int failed = VMAction.ACTION_ID_NONE;
         //参数列表
         [SerializeField]
         public List<int> m_params = new List<int>();

        public void Serialize(VMActionEditor e)
        {
            m_ID = e.m_ID;
            name = e.name;
            m_vTop = e.m_vTop;
            m_vLeft = e.m_vLeft;
            m_width = e.m_width;
            m_height = e.m_height;

            actionType = e.actionType;
            success = e.success;
            failed = e.failed;

            m_params = e.m_params;
            //m_contentArea = e.m_contentArea;
            //m_DrawLineArea = e.m_DrawLineArea;

            //e.m_contentArea = new Rect(m_vLeft, m_vTop, m_width, m_height);
            //e.m_DrawLineArea = new Rect(m_vLeft + m_width / 4, m_vTop + m_height, m_width / 2, 20);
        }

        public void Deserialize(VMActionEditor e)
        {
            e.m_ID = m_ID;
            e.name = name;
            e.m_vLeft = m_vLeft;
            e.m_vTop = m_vTop;
            e.m_width = m_width;
            e.m_height = m_height;
            e.actionType = actionType;

            e.success = success;
            e.failed = failed;

            if(m_params == null)
            {
                m_params = new List<int>();
            }
            e.m_params = m_params;

            e.UpdateRect();
            //e.m_contentArea = e.GetContentRect();
            //e.m_DrawLineArea = e.GetDrawLineRect();
        }

    }

    [Serializable]
    public class VMSateSerialize
    {
        [Serializable]
        public class StateEventHandleSerialize
        {
            [SerializeField]
            public VMEventType eventType;
            [SerializeField]
            public int m_ActionID;
        }

        [SerializeField]
        public int m_ID;
        [SerializeField]
        public string name = "NodeName";

        [SerializeField]
        public float m_vTop;
        [SerializeField]
        public float m_vLeft;
        [SerializeField]
        public float m_width = 100;
        [SerializeField]
        public float m_height = 100;

        [SerializeField]
        public List<StateEventHandleSerialize> eventHandleList = new List<StateEventHandleSerialize>();

        public void Serialize(VMStateEditor e)
        {
            m_ID = e.m_ID;
            name = e.name;
            m_vTop = e.m_vTop;
            m_vLeft = e.m_vLeft;
            m_width = e.m_width;
            m_height = e.m_height;
            //m_contentArea = e.m_contentArea;
            //m_DrawLineArea = e.m_DrawLineArea;
            foreach(KeyValuePair<VMEventType, int> kvp in e.m_eventHandleDic)
            {
                StateEventHandleSerialize s = new StateEventHandleSerialize();
                s.eventType = kvp.Key;
                s.m_ActionID = kvp.Value;
                eventHandleList.Add(s);

            }
        }

        public void Deserialize(VMStateEditor e)
        {
            e.m_ID = m_ID;
            e.name = name;

            e.m_vLeft = m_vLeft;
            e.m_vTop = m_vTop;
            e.m_width = m_width;
            e.m_height = m_height;

            e.UpdateRect();
            //e.m_contentArea = e.GetContentRect();
            //e.m_DrawLineArea = e.GetDrawLineRect();

            e.m_eventHandleDic.Clear();
            foreach(StateEventHandleSerialize s in eventHandleList)
            {
                e.m_eventHandleDic.Add(s.eventType, s.m_ActionID);
            }
        }

    }

    [Serializable]
    public class VMDataSerialize
    {
        [SerializeField]
        public List<VMActionSerialize> actionList = new List<VMActionSerialize>();
        [SerializeField]
        public List<VMSateSerialize> stateList = new List<VMSateSerialize>();
        public string m_vmDataName = "1";

        public void Serialize(VMDataEidtor data)
        {
            m_vmDataName = VMWindow.m_vmDataName;

            foreach (VMStateEditor state in data.m_vmStateDic.Values)
            {
                VMSateSerialize s = new VMSateSerialize();
                s.Serialize(state);
                stateList.Add(s);
            }

            foreach (VMActionEditor action in data.m_vmActionDic.Values)
            {
                VMActionSerialize s = new VMActionSerialize();
                s.Serialize(action);

                actionList.Add(s);
            }

        }

        public void Deserialize(VMDataEidtor data)
        {
            //VMWindow.m_vmDataName = m_vmDataName;

            data.m_vmActionDic.Clear();

            foreach(VMActionSerialize s in actionList)
            {
                VMActionEditor action = new VMActionEditor(data, s.m_ID);
                s.Deserialize(action);
                data.m_vmActionDic.Add(s.m_ID, action);
            }

            foreach (VMSateSerialize s in stateList)
            {
                VMStateEditor state = new VMStateEditor(data, s.m_ID);
                s.Deserialize(state);
                data.m_vmStateDic.Add(s.m_ID, state);
            }
        }


    }

}


