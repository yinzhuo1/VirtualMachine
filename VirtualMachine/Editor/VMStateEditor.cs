using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace VirutalMachine
{
    //[Serializable]
    public class VMStateEditor : VMBaseNodeEditor
    {
        //事件类型->ActionID
        public Dictionary<VMEventType, int> m_eventHandleDic = new Dictionary<VMEventType, int>();

        public VMStateEditor(VMDataEidtor data, int id) : base(data, id)
        {
            name = "State";
        }

        public void SaveData(VMState state)
        {
            foreach (KeyValuePair<VMEventType, int> kvp in m_eventHandleDic)
            {
                state.AddEventHandle((int)kvp.Key, kvp.Value);
            }
        }

        
        public override void Draw()
        {
            base.Draw();

            Color preColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            GUI.Box(m_contentArea, "");
            GUI.backgroundColor = preColor;

            GUILayout.BeginArea(m_contentArea);

            //GUILayout.BeginHorizontal();
           
            //GUILayout.EndHorizontal();

            GUILayout.EndArea();

            if (m_eventHandleDic != null)
            {
                //绘制事件的连线
                foreach (KeyValuePair<VMEventType, int> kvp in m_eventHandleDic)
                {
                    VMActionEditor action = m_vmData.GetAction(kvp.Value);
                    Vector3 startPos = action.GetDrawLinePostion();
                    Vector3 endPos = GetDrawLinePostion();

                    Handles.DrawLine(startPos, endPos);

                    Vector3 centerPos = new Vector3((startPos.x + endPos.x) / 2, (startPos.y + endPos.y) / 2, 0);

                    Handles.Label(centerPos, VMEventTypeDes.DesArray[(int)kvp.Key]);
                }
            }
        }

        private VMEventType m_selectEventType = VMEventType.vmeFirstTimer;

        private int m_selectEvenTypeIndex = 0;
        public override void OnInspector()
        {
            base.OnInspector();
            //m_selectEventType = (VMEventType)EditorGUILayout.EnumPopup("事件类型", m_selectEventType, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });

            m_selectEvenTypeIndex = (int)m_selectEventType;
            m_selectEvenTypeIndex = EditorGUILayout.Popup("事件类型", m_selectEvenTypeIndex, VMEventTypeDes.DesArray);
            m_selectEventType = (VMEventType)m_selectEvenTypeIndex;

            if (GUILayout.Button("AddEvent"))
            {
                VMActionEditor action = null;
                if(m_eventHandleDic.ContainsKey(m_selectEventType))
                {
                    EditorUtility.DisplayDialog("Error", "VMStateEditor OnInspector Duplicate Event = " + m_selectEventType.ToString(), "OK");
                    Debug.LogError("VMStateEditor OnInspector Duplicate Event = " + m_selectEventType.ToString());
                    return;
                }

                action = m_vmData.CreateAction();
                m_eventHandleDic.Add(m_selectEventType, action.m_ID);

            }

            if (m_eventHandleDic != null)
            {
                if (NGUIEditorTools.DrawHeader("执行操作"))
                {
                    NGUIEditorTools.BeginContents();

                    GUILayout.BeginHorizontal();

                    GUILayout.Label("响应事件类型:");
                    GUILayout.Label("执行操作名称:");

                    GUILayout.EndHorizontal();

                    GUILayout.Space(10);

                    Color srcColor = GUI.backgroundColor;

                    GUI.backgroundColor = Color.white;//new Color(0.8f, 0.8f, 0.8f);
                    foreach (KeyValuePair<VMEventType, int> kvp in m_eventHandleDic)
                    {
                        //EditorGUILayout.ObjectField(kvp.Value, typeof(VMActionEditor));
                        GUILayout.BeginHorizontal();

                        VMActionEditor action = m_vmData.GetAction(kvp.Value);

                        GUILayout.Label(VMEventTypeDes.DesArray[(int)kvp.Key]);
                        GUILayout.Label(action.GetName());

                        GUILayout.EndHorizontal();
                    }

                    GUI.backgroundColor = srcColor;

                    NGUIEditorTools.EndContents();
                }

                
            }
               
            
        }

        public void AddEventHandle(VMEventType eventType)
        {

        }
    }
}


