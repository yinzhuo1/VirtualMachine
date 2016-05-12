using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VirutalMachine
{
    [CustomEditor(typeof(VMManager), true)]
    public class VMManagerInspector : Editor
    {
        public int loadVMDataName = 1;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            loadVMDataName = EditorGUILayout.IntField("要加载的资源名称:", loadVMDataName);

            if(GUILayout.Button("加载"))
            {
                VMManager.GetInstance().LoadVMData(loadVMDataName.ToString());
            }

        }
    }
}

