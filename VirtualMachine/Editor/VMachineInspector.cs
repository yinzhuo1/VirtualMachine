using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VirutalMachine
{
    [CustomEditor(typeof(VMachine), true)]
    public class VMachineInspector : Editor
    {
        public VMEventType eventType = VMEventType.vmeFirstTimer;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            eventType = (VMEventType)EditorGUILayout.EnumPopup("事件类型:", eventType);

            if (GUILayout.Button("发射事件"))
            {
                VMachine machine = this.target as VMachine;
                machine.FireEvent((int)eventType);
            }

        }
    }
}

