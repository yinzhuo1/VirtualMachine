using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace VirutalMachine
{
    public static class VMConfig
    {

        public static string VMDataPath = Application.dataPath + "/RawAssets/VirtualMachine/";
        //public static string VMDataAssetPath = "Assets/RawAssets/VirtualMachine/";
        public static string VMDataEditSuffix = "editor";
        public static string VMDataSuffix = "asset";

        public static string GetVMDataAbsolutePath(string name, bool bIsEditor)
        {
            if (bIsEditor)
            {
                return VMDataPath + name +  "." + VMDataEditSuffix;
            }
            return VMDataPath + name + "." + VMDataSuffix;
        }

        //public static string GetVMDataAssetsPath(string name, bool bIsEditor)
        //{
        //    if (bIsEditor)
        //    {
        //        return VMDataAssetPath + name + VMDataEditSuffix;
        //    }
        //    return VMDataAssetPath + name + VMDataSuffix;
        //}

    }

    public class VMWindow : EditorWindow
    {
        

        [SerializeField]
        public static VMWindow instance;

        private VMDataEidtor m_vmDataEditor = null;

        public static string m_vmDataName = "1";
        

        [MenuItem("Editor/AI状态机编辑器/Editor")]
        public static void ShowVMDataEidtorWindow()
        {
            instance = EditorWindow.GetWindow<VMWindow>(false, "VMWindow");

            instance.wantsMouseMove = true;
            instance.minSize = new Vector2(500f, 100f);

            instance.Init();

        }

         [MenuItem("Editor/AI状态机编辑器/添加状态机管理器")]
        public static void CreateVMManager()
        {
            VMManager.GetInstance();
        }

        public void Init()
        {
            
        }

        public void OnEnable()
        {
            if(m_vmDataEditor != null)
            {
                m_vmDataEditor.OnEnable();
            }
        }

        public void OnDestroy()
        {
            if(m_vmDataEditor != null)
            {
                SaveData();
            }
            
        }

        public static int InspectorWidth = 300;
        public static int HeaderHeight = 50;
        public static Rect m_inspectorRect = new Rect(0, 0, InspectorWidth, Screen.height);
        public static Rect m_graphRect = new Rect(300, HeaderHeight, Screen.width - InspectorWidth, Screen.height);
        public static Rect m_headerRect = new Rect(300, 0, Screen.width - InspectorWidth, HeaderHeight);

        private void SetUpSize()
        {
            m_inspectorRect.Set(0, 0, InspectorWidth, Screen.height);
            m_graphRect.Set(InspectorWidth, HeaderHeight, Screen.width - InspectorWidth, Screen.height);
            m_headerRect.Set(300, 0, Screen.width - InspectorWidth, HeaderHeight);
        }

        public void OnGUI()
        {
            GUI.enabled = !EditorApplication.isCompiling;

            SetUpSize();

            GUILayout.BeginArea(m_headerRect);

            GUILayout.BeginHorizontal();

            m_vmDataName = EditorGUILayout.IntField("状态机名称：", int.Parse(m_vmDataName)).ToString();
     
            EditorGUILayout.SelectableLabel(VMConfig.VMDataPath);

            if(GUILayout.Button("选择文件夹"))
            {
                VMConfig.VMDataPath = EditorUtility.OpenFolderPanel("Save File Path", VMConfig.VMDataPath, "VirtualMachine");
            }
            //string path = EditorUtility.OpenFilePanel("Save File Path", "", "editor");
            //Debug.LogError("FilePath" + path);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            if (GUILayout.Button("LoadVM"))
            {
                //if(m_vmDataEditor != null)
                //{
                //    SaveData();
                //}
                LoadData();
            }

            if(m_vmDataEditor != null)
            {
                if (GUILayout.Button("SaveVM"))
                {
                    SaveData();
                    m_vmDataEditor.SaveData();
                }
            }
            else
            {
                if (GUILayout.Button("CreateVM"))
                {
                    CreateVM();
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            Handles.DrawLine(new Vector2(m_graphRect.xMin, 0), new Vector2(m_graphRect.xMin, Screen.height));

            if(m_vmDataEditor != null)
            {
                m_vmDataEditor.OnGUI();
            }

        }

        public VMDataEidtor LoadData()
        {
            //string savePath = VMConfig.GetVMDataAbsolutePath(m_vmDataName, true);

            //if (!CheckFileExist(savePath))
            //{
            //     EditorUtility.DisplayDialog("Wanning", "文件不存在 路径= " + savePath, "OK");
            //     return null;
            //}


            //BinaryFormatter binaryFormate = new BinaryFormatter();
            //FileStream fileStream = File.OpenRead(savePath);
            //try
            //{
            //    VMDataSerialize s = binaryFormate.Deserialize(fileStream) as VMDataSerialize;

            //    m_vmDataEditor = new VMDataEidtor();
            //    m_vmDataEditor.Deserialize(s);

            //}
            //catch (System.Exception ex)
            //{
            //    Debug.LogError(ex);
            //}
            //finally
            //{
            //    fileStream.Close();
            //}

            //return m_vmDataEditor;

            VMDataSerialize s = LoadData(m_vmDataName, true) as VMDataSerialize;
            m_vmDataEditor = new VMDataEidtor();
            m_vmDataEditor.Deserialize(s);

            return m_vmDataEditor;

        }

        public void SaveData(bool bShowWanningIfExist = true)
        {
            if (m_vmDataEditor == null)
            {
                return;
            }

            //string savePath = VMConfig.GetVMDataAbsolutePath(m_vmDataName, true);

            //string assetPath = VMConfig.GetVMDataAssetsPath(m_vmDataName, true);

            //if (CheckFileExist(savePath))
            //{
            //    bool bConver = EditorUtility.DisplayDialog("Wanning", "已经存在" + savePath + "确定覆盖？", "OK", "Cancle");
            //    if (!bConver)
            //    {
            //        return;
            //    }
            //}


            //if(m_vmDataEditor == null)
            //{
            //    return;
            //}
            //VMDataSerialize s = new VMDataSerialize();
            //m_vmDataEditor.Serialize(s);

            //BinaryFormatter binaryFormate = new BinaryFormatter();


            //FileStream fileStream = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            //try
            //{
            //    binaryFormate.Serialize(fileStream, s);
            //}
            //catch (System.Exception ex)
            //{
            //    Debug.LogError(ex);
            //}
            //finally
            //{
            //    fileStream.Close();
            //}

            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();

            

            VMDataSerialize s = new VMDataSerialize();
            m_vmDataEditor.Serialize(s);
            SaveData(m_vmDataName, s, true, bShowWanningIfExist);


        }

        public VMDataEidtor CreateVM()
        {
            CheckParentDirectory();

            string savePath = VMConfig.GetVMDataAbsolutePath(m_vmDataName, true);
            
            if(CheckFileExist(savePath))
            {
                bool bExist = EditorUtility.DisplayDialog("Wanning", "已经存在" + savePath + "是否直接加载？", "OK", "Cancle");
                if (!bExist)
                {
                    return null;
                }
                else
                {
                    m_vmDataEditor = LoadData();
                }
            }

            m_vmDataEditor = new VMDataEidtor();

            VMStateEditor stateEditor = m_vmDataEditor.CreateState();
            m_vmDataEditor.m_initStateID = stateEditor.m_ID;
            stateEditor.name = "初始状态(入口)";
            //m_vmDataEditor = ScriptableObject.CreateInstance<VMDataEidtor>();


            //string assetPath = VMConfig.GetVMDataAssetsEditPath(m_vmDataName);
            //AssetDatabase.CreateAsset(m_vmDataEditor, assetPath);

            return m_vmDataEditor;
        }

        public static void CheckParentDirectory()
        {
            if(!Directory.Exists(VMConfig.VMDataPath))
            {
                Directory.CreateDirectory(VMConfig.VMDataPath);
            }
        }

        public static bool CheckFileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public static void SaveData(string name, System.Object s, bool isEditor, bool bShowWanningIfExist = true)
        {
            string savePath = VMConfig.GetVMDataAbsolutePath(name, isEditor);

            //string assetPath = VMConfig.GetVMDataAssetsPath(name, isEditor);
            if (bShowWanningIfExist)
            {
                if (CheckFileExist(savePath))
                {
                    bool bConver = EditorUtility.DisplayDialog("Wanning", "已经存在" + savePath + "确定覆盖？", "OK", "Cancle");
                    if (!bConver)
                    {
                        return;
                    }
                }
            }
            

            BinaryFormatter binaryFormate = new BinaryFormatter();

            FileStream fileStream = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            try
            {
                binaryFormate.Serialize(fileStream, s);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
            finally
            {
                fileStream.Close();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static System.Object LoadData(string name, bool isEditor) 
        {
            string savePath = VMConfig.GetVMDataAbsolutePath(name, isEditor);

            if (!CheckFileExist(savePath))
            {
                EditorUtility.DisplayDialog("Wanning", "文件不存在 路径= " + savePath, "OK");
                return null;
            }

            //string assetPath = VMConfig.GetVMDataAssetsEditPath(m_vmDataName);
            //Object o = AssetDatabase.LoadAssetAtPath(assetPath, typeof(VMDataEidtor));
            System.Object s = null;
            BinaryFormatter binaryFormate = new BinaryFormatter();
            FileStream fileStream = File.OpenRead(savePath);
            try
            {
                s = binaryFormate.Deserialize(fileStream);

            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
            finally
            {
                fileStream.Close();
            }



            //m_vmDataEditor = AssetDatabase.LoadAssetAtPath(assetPath, typeof(VMDataEidtor)) as VMDataEidtor;

            return s;
        }
    }

}
