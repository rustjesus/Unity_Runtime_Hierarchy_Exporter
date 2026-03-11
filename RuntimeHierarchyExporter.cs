using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RuntimeHierarchyExporter : MonoBehaviour
{
    [System.Serializable]
    public class HierarchyNode
    {
        public string name;
        public bool active;
        public List<HierarchyNode> children = new List<HierarchyNode>();
    }

    [System.Serializable]
    public class HierarchySaveData
    {
        public List<HierarchyNode> roots = new List<HierarchyNode>();
    }
    string savePath => Application.isEditor
        ? Application.dataPath + "/Quests" + PlayerPrefs.GetString("SaveDir") + "/hierarchy.json"
        : Application.persistentDataPath + "/Saves" + PlayerPrefs.GetString("SaveDir") + "/Quests/hierarchy.json";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveHierarchy();
        }
    }

    public void SaveHierarchy()
    {
        HierarchySaveData data = new HierarchySaveData();

        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            data.roots.Add(BuildNode(root.transform));
        }

        string json = JsonUtility.ToJson(data, true);

        Directory.CreateDirectory(Path.GetDirectoryName(savePath));
        File.WriteAllText(savePath, json);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        Debug.Log("Hierarchy saved to: " + savePath);
    }

    HierarchyNode BuildNode(Transform t)
    {
        HierarchyNode node = new HierarchyNode();
        node.name = t.name;
        node.active = t.gameObject.activeSelf;

        foreach (Transform child in t)
        {
            node.children.Add(BuildNode(child));
        }

        return node;
    }
}