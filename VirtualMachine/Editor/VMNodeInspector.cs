using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace VirutalMachine
{
    [Serializable]
    public class VMNodeInspector : ScriptableObject
    {
        [SerializeField]
        private Vector3 m_scrollPosition = Vector3.zero;

        //private Rect m_inpectorRect = new Rect(0, 0, 1000, 1000);

        

        public bool DrawNodeInspector(VMBaseNodeEditor node)
        {
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition);

            EditorGUI.BeginChangeCheck();

            if(node != null)
            {
                node.OnInspector();
            }
            else
            {
                GUILayout.Label("请选中一个节点进行编辑");
            }

            //VMStateEditor state = node as VMStateEditor;
            //if(state != null)
            //{
                
            //}

            //VMActionEditor action = node as VMActionEditor;
            //if(action != null)
            //{
                
            //}


            EditorGUI.EndChangeCheck();

            GUILayout.EndScrollView();

            return true;
        }
    }

}
