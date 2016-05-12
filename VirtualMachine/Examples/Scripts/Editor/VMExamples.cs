using UnityEngine;
using System.Collections;
using UnityEditor;
using VirutalMachine;

public class VMExamples 
{

    [MenuItem("Editor/AI状态机编辑器/测试")]
    public static void CreateVMManager()
    {
        VMManager.GetInstance();
        GameObject testObj = new GameObject("VMExamples");
        GameObject NoramFishObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        NoramFishObj.name = "NormalFishAI";
        NoramFishObj.transform.parent = testObj.transform;
        NoramFishObj.transform.localPosition = Vector3.zero;
        NoramFishObj.transform.localEulerAngles = Vector3.zero;
        NormalFishAI normalFishAI = NoramFishObj.AddComponent<NormalFishAI>();

    }

    [MenuItem("Editor/AI状态机编辑器/测试选择")]
    public static void CreateWizard()
    {
        string path = EditorUtility.OpenFilePanel("Save File Path", "", "editor");
        Debug.LogError("FilePath" + path);
    }

}
