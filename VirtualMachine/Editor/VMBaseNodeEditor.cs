using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace VirutalMachine
{
    [Serializable]
    public class VMBaseNodeEditor : ScriptableObject
    {
        public VMDataEidtor m_vmData;

        [SerializeField]
        public int m_ID;
        [SerializeField]
        public string name = "NodeName";

        public string GetName()
        {
            return name + m_ID;
        }

        [SerializeField]
        public float m_vTop;
        [SerializeField]
        public float m_vLeft;
        [SerializeField]
        public float m_width = 100;
        [SerializeField]
        public float m_height = 100;
        [SerializeField]
        public Rect m_contentArea = new Rect(0, 0, 100, 100);
        [SerializeField]
        public Rect m_DrawLineArea = new Rect(25, 100, 50, 20);

        [SerializeField]
        public bool m_selected = false;
        [SerializeField]
        public bool m_selectLineArea = false;

        public VMBaseNodeEditor(VMDataEidtor data, int id)
        {
            m_vmData = data;
            m_ID = id;

            m_vLeft = VMWindow.m_graphRect.xMin;
            m_vTop = VMWindow.m_graphRect.yMin;
        }

      
        public virtual void Serialize()
        {

        }

        public virtual void Draw()
        {
            GUI.Box(m_contentArea, "");

            // 可连线区域
            GUI.Box(m_DrawLineArea, "State");

            if(m_selectLineArea)
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

            GUILayout.EndArea();

            
            //GUI.Label(new Rect(m_vLeft, m_vTop, 100, 100), name);

        }

        //在Inspector面板中绘制
        public virtual void OnInspector()
        {
            name = EditorGUILayout.TextField("节点名称", name);
        }

        public bool Select(Vector3 mousePosition)
        {
            if (Contains(mousePosition))
            {
                m_selected = true;
                return true;
            }

            if(ContainsInLineArea(mousePosition))
            {
                m_selectLineArea = true;
                return true;
            }
            m_selected = false;
            m_selectLineArea = false;

            return m_selected;
        }

        public void UnSelect()
        {
            m_selected = false;
            m_selectLineArea = false;
        }

        public virtual bool DragNode(Vector3 delta)
        {
            if (m_selected)
            {
                m_vLeft += delta.x;
                m_vTop += delta.y;

                m_contentArea.Set(m_vLeft, m_vTop, m_width, m_height);

                m_DrawLineArea.Set(m_vLeft + m_width / 4, m_vTop + m_height, m_width / 2, 20);
            }



            return true;
        }


        public bool Contains(Vector3 point)
        {
            return m_contentArea.Contains(point);
        }

        //是否在连线区域
        public bool ContainsInLineArea(Vector3 point)
        {
            return m_DrawLineArea.Contains(point);
        }

        public Vector3 GetDrawLinePostion()
        {
            //Vector2 startPos = new Vector2(m_DrawLineArea.min.x + m_DrawLineArea.width / 2, m_DrawLineArea.min.y);
            //return startPos;

            return m_contentArea.center;
        }

        public Rect GetDrawLineRect()
        {
            return new Rect(m_vLeft + m_width / 4, m_vTop + m_height, m_width / 2, 20);
        }

        public Rect GetContentRect()
        {
            return new Rect(m_vLeft, m_vTop, m_width, m_height);
        }

        public virtual void UpdateRect()
        {
            m_contentArea = new Rect(m_vLeft, m_vTop, m_width, m_height);
            m_DrawLineArea = new Rect(m_vLeft + m_width / 4, m_vTop + m_height, m_width / 2, 20);
        }
    }
}

