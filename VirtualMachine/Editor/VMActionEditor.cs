using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace VirutalMachine
{
    public enum ParamType
    {
        ptInt,
        ptFloat,
        ptString,
        ptVector3D
    }

    public struct ActionParam
    {
        public ParamType paramType;
        public object obj;
    }
     //[Serializable]
    public class VMActionEditor : VMBaseNodeEditor
    {
         [SerializeField]
        public VMActionType actionType = VMActionType.vmaNone;
        //参数列表
        [SerializeField]
        public List<int> m_params = new List<int>();

        //执行的分支 成功、失败、错误
        [SerializeField]
        public int success = VMAction.ACTION_ID_NONE;
        public int failed = VMAction.ACTION_ID_NONE;
        //public int error = VMAction.ACTION_ID_NONE;

        public Rect m_successRect;
        public Rect m_failedRect;
        //public Rect m_errorRect;

        SerializedObject m_obj = null;

        public VMActionEditor(VMDataEidtor data, int id) : base(data, id)
        {
            name = "Action";

            m_obj = new SerializedObject(this);

           

            m_successRect = new Rect(m_vLeft + m_width / 4, m_vTop + m_height, m_width / 4, 20);

            m_failedRect = new Rect(m_vLeft + m_width / 2, m_vTop + m_height, m_width / 4, 20);

            m_params = new List<int>();
            
        }

         public void SaveData(VMAction action)
         {
             action.m_params = new List<object>();
             for (int i = 0; i < m_params.Count; i++)
             {
                 action.m_params.Add(m_params[i]);
             }

             action.m_branchIDs = new List<int>();
             action.m_branchIDs.Add(success);
             action.m_branchIDs.Add(failed);
         }

        public override void Draw()
        {
            //base.Draw();

            GUI.Box(m_contentArea, "");

            Color preColor = Handles.color;
            Handles.color = Color.yellow;
            Handles.RectangleCap(1, m_contentArea.center, Quaternion.EulerRotation(0, 0, 45), m_contentArea.size.x / 2 - 14);
            //Handles.SphereCap(1, m_contentArea.center, Quaternion.identity, m_contentArea.size.x / 2);
            Handles.color = preColor;
            // 可连线区域
            //GUI.Box(m_DrawLineArea, "fdsfdasfdas");

            if (m_selectLineArea)
            {
                //Draw Line
                Vector2 startPos = GetDrawLinePostion();
                Vector3 dstPos = Event.current.mousePosition;
                Handles.DrawLine(startPos, dstPos);
                //Handles.DrawPolyLine(
                //Debug.DrawLine(startPos, dstPos);
                //Debug.Log("From Pos " + startPos.x + " Dst " + dstPos.x);
            }

            GUILayout.BeginArea(m_contentArea);

            GUILayout.Label(name + m_ID);

            GUILayout.Space(20);
            EditorGUILayout.LabelField(VMActionTypeDes.DesArray[(int)actionType]);

            GUILayout.EndArea();

            GUI.Box(m_successRect, "Success");
            GUI.Box(m_failedRect, "Failed");
            //GUI.Box(new Rect(m_contentArea.x, m_contentArea.y + 20, 50, 50), "Action" + m_ID);

            DrawBranchLine(success, GetSuccePos());
            DrawBranchLine(failed, GetFailedPos());

            //切换状态的Action 绘制连接状态线
            if(actionType == VMActionType.vmaGoToState)
            {
                int selectStateID = 0;
                if (m_params.Count > 0)
                {
                    selectStateID = m_params[0];
                }
                VMStateEditor state = m_vmData.GetState(selectStateID);
                if (state != null)
                {
                    
                    Vector3 endPos = state.GetDrawLinePostion();
                    Vector3 startPos = GetSuccePos();

                    Handles.DrawLine(startPos, endPos);

                    //Vector3 direction = (startPos - endPos).normalized;
                    //Vector3 arrowPos = endPos + direction * 100;

                    //Handles.DrawArrow
                    //Handles.RectangleCap(1, endPos, Quaternion.LookRotation(endPos - startPos), 100);
                    //float distance = Vector3.Distance(startPos, endPos);
                    //Handles.color = Color.blue;
                   // Handles.ArrowCap(1, arrowPos, Quaternion.LookRotation(endPos - startPos), 100);
                    //Handles.DrawArrow(1, startPos, Quaternion.LookRotation(endPos - startPos), distance);
                }
            }

            //if(success != VMAction.ACTION_ID_NONE)
            //{
            //    //绘制事件的连线
            //    VMActionEditor action = m_vmData.GetAction(success);
            //    Vector3 startPos = action.GetDrawLinePostion();
            //    Vector3 endPos = GetSuccePos();

            //    Handles.DrawLine(startPos, endPos);

            //}
        }

        private void DrawBranchLine(int id, Vector3 endPos)
        {
            if (id != VMAction.ACTION_ID_NONE)
            {
                //绘制事件的连线
                VMActionEditor action = m_vmData.GetAction(id);
                if(action != null)
                {
                    Vector3 startPos = action.GetDrawLinePostion();
                    //Vector3 endPos = GetSuccePos();

                    Handles.DrawLine(startPos, endPos);
                }
                

            }
        }

        int m_selectSuccessIndex = 0;
        int m_selectFailedIndex = 0;
        int m_selectActionTypeIndex = 0;
        int m_selectStateTypeIndex = 0;
        public override void OnInspector()
        {
            base.OnInspector();

            //actionType = (VMActionType)EditorGUILayout.EnumPopup("执行操作", actionType);
            m_selectActionTypeIndex = (int)actionType;
            m_selectActionTypeIndex = EditorGUILayout.Popup("执行操作", m_selectActionTypeIndex, VMActionTypeDes.DesArray);
            actionType = (VMActionType)m_selectActionTypeIndex;

            //参数
            if (actionType == VMActionType.vmaGoToState)
            {
                //切换状态操作 只能选状态
                int selectStateID = 0;
                if(m_params.Count > 0)
                {
                    selectStateID = m_params[0];
                }

                m_selectStateTypeIndex = DrawStateBranch("转换到状态:", m_selectStateTypeIndex, ref selectStateID);

                if(m_params.Count <= 0)
                {
                    m_params.Add(selectStateID);
                }
                else if(m_params.Count == 1)
                {
                    m_params[0] = selectStateID;
                }
                else
                {
                    m_params = new List<int>(1);
                    m_params.Add(selectStateID);
                }

               
                
            }
            else
            {
                if (NGUIEditorTools.DrawHeader("参数"))
                {
                    NGUIEditorTools.BeginContents();

                    int paramsCount = m_params.Count;
                    paramsCount = EditorGUILayout.IntField("参数个数:", paramsCount);
                    int beforeCount = m_params.Count;
                    if (paramsCount > beforeCount)
                    {
                        for (int i = beforeCount; i < paramsCount; i++)
                        {
                            m_params.Add(0);
                        }
                    }
                    else if (paramsCount < beforeCount)
                    {
                        m_params.RemoveRange(paramsCount - 1, beforeCount - paramsCount);
                    }

                    for (int i = 0; i < paramsCount; i++)
                    {
                        m_params[i] = EditorGUILayout.IntField("参数" + i + ":", m_params[i]);
                    }

                    NGUIEditorTools.EndContents();
                }
            }
           
           

            //切换状态操作比较特殊 不会在往下执行了
            if(actionType == VMActionType.vmaGoToState)
            {
                success = (int)VMActionType.vmaNone;
                //failed = (int)VMActionType.vmaNone;
                m_selectFailedIndex = DrawActionBranch("失败分支", m_selectFailedIndex, ref failed);
            }
            else
            {
                m_selectSuccessIndex = DrawActionBranch("成功分支", m_selectSuccessIndex, ref success);
                m_selectFailedIndex = DrawActionBranch("失败分支", m_selectFailedIndex, ref failed);
            }
           

            //if (m_obj == null)
            //{
            //    return;
            //}
            //EditorGUI.BeginChangeCheck();
            //m_obj.Update();
            //SerializedProperty iterator = m_obj.GetIterator();
            //for (bool flag = true; iterator.NextVisible(flag); flag = false)
            //{
            //    EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
            //}
            //m_obj.ApplyModifiedProperties();


            //EditorGUI.EndChangeCheck();

        }

        private int DrawActionBranch(string text, int selecteIndex, ref int selectID)
        {
            GUILayout.BeginHorizontal();

            //GUILayout.Label("成功分支:");

            List<int> actionIDList = m_vmData.GetActionIDList(m_ID);
            actionIDList.Insert(0, VMAction.ACTION_ID_NONE);
            int count = actionIDList.Count;

            if (count > 0)
            {
                string[] actionNameList = new string[count];
                for (int i = 0; i < count; i++)
                {
                    if (actionIDList[i] == selectID)
                    {
                        selecteIndex = i;
                    }

                    if (actionIDList[i] == VMAction.ACTION_ID_NONE)
                    {
                        actionNameList[i] = "无";
                    }
                    else
                    {
                        VMActionEditor action = m_vmData.GetAction(actionIDList[i]);
                        actionNameList[i] = action.GetName();
                    }


                }

                //容错处理
                if (selecteIndex >= count)
                {
                    Debug.LogError("selected is to large = " + selecteIndex + "count = " + count);
                    selecteIndex = 0;

                }

                selecteIndex = EditorGUILayout.Popup(text, selecteIndex, actionNameList);
                selectID = actionIDList[selecteIndex];
            }


            if (GUILayout.Button("增加操作"))
            {
                VMActionEditor action = m_vmData.CreateAction();
                selectID = action.m_ID;

                actionIDList = m_vmData.GetActionIDList(m_ID);
                selecteIndex = actionIDList.IndexOf(action.m_ID) + 1;
            }

            GUILayout.EndHorizontal();

            return selecteIndex;
        }

        private int DrawStateBranch(string text, int selecteIndex, ref int selectID)
        {
            GUILayout.BeginHorizontal();

            //GUILayout.Label("成功分支:");

            List<int> stateIDList = m_vmData.GetStateIDList(0);
            //actionIDList.Insert(0, VMAction.ACTION_ID_NONE);
            int count = stateIDList.Count;

            if (count > 0)
            {
                string[] stateNameList = new string[count];
                for (int i = 0; i < count; i++)
                {
                    if (stateIDList[i] == selectID)
                    {
                        selecteIndex = i;
                    }

                    VMStateEditor state = m_vmData.GetState(stateIDList[i]);
                    stateNameList[i] = state.GetName();


                }

                //容错处理
                if (selecteIndex >= count)
                {
                    Debug.LogError("selected is to large = " + selecteIndex + "count = " + count);
                    selecteIndex = 0;

                }

                selecteIndex = EditorGUILayout.Popup(text, selecteIndex, stateNameList);
                selectID = stateIDList[selecteIndex];
            }


            if (GUILayout.Button("增加状态"))
            {
                VMStateEditor state = m_vmData.CreateState();
                selectID = state.m_ID;

                stateIDList = m_vmData.GetStateIDList(0);
                selecteIndex = stateIDList.IndexOf(state.m_ID) + 1;
            }

            GUILayout.EndHorizontal();

            return selecteIndex;
        }

        public override bool DragNode(Vector3 delta)
        {
            base.DragNode(delta);
            if (m_selected)
            {

                m_successRect.Set(m_vLeft + m_width / 4, m_vTop + m_height, m_width / 4, 20);

                m_failedRect.Set(m_vLeft + m_width / 2, m_vTop + m_height, m_width / 4, 20);
            }



            return true;
        }

        public override void UpdateRect()
        {
            base.UpdateRect();

            m_successRect = new Rect(m_vLeft + m_width / 4, m_vTop + m_height, m_width / 4, 20);

            m_failedRect = new Rect(m_vLeft + m_width / 2, m_vTop + m_height, m_width / 4, 20);
        }

        public Vector3 GetSuccePos()
        {
            return m_successRect.center;
        }

        public Vector3 GetFailedPos()
        {
            return m_failedRect.center;
        }

    }
}

