using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using GraphLibrary;
using UnityEngine.UI;

public class PopulateUI : MonoBehaviour
{
    //public GameObject TitleText;

    string folderType = "application/vnd.google-apps.folder";
    string fileType = "application/vnd.google-apps.file";

    public void PopulateDatasetList(object[] parameters)
    {
        print("Message broadcasted.");
        if(transform.childCount != 0)
        {
            //transform.gameObject.DestroyChildren();
            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }   
        }

        //TitleText.GetComponent<Text>().text = 

        Dictionary<string, string> fileNames = (Dictionary<string, string>)parameters[0];
        Dictionary<string, string> mimeTypes = (Dictionary<string, string>)parameters[1];
        //Dictionary<string, string> fileAndParent = (Dictionary<string, string>)parameters[2];

        print("Here.");

        foreach (var kvp in fileNames)
        {
            //if (rootId == "" && !fileNames.ContainsKey(fileAndParent[kvp.Key])) // if the parent of kvp doesn't exist in the fileNames dictionary, then this parent is the root directory.
            //    rootId = fileAndParent[kvp.Key];
            print(kvp.Key);
            GameObject option;
            if (mimeTypes[kvp.Key] == folderType)
                option = Instantiate(Resources.Load<GameObject>("FolderOption"));
            else
                option = Instantiate(Resources.Load<GameObject>("FileOption"));

            option.name = kvp.Key;
            option.GetComponentInChildren<Text>().text = kvp.Value;
            option.transform.SetParent(this.transform, false);

            //if(fileAndParent[kvp.Key] != rootId)
            //    Debug.Log("file name '" + kvp.Value + "' has parent '"+fileNames[fileAndParent[kvp.Key]]+"'");
            //else
            //    Debug.Log("file name '" + kvp.Value + "' is in the root directory");
        }
    }
}
