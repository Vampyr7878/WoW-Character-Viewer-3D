using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

//Add Custom Menu to Unity Editor
public class CustomMenu : MonoBehaviour
{
    //Menu option that generates *.bytes files for import process.
    [MenuItem("WoW Character Viewer/Generate *.bytes files")]
    public static void GenerateBytesFiles()
    {
        List<string> m2 = ListFiles(@"Assets\Resources\", ".m2");
        List<string> skin = ListFiles(@"Assets\Resources\", ".skin");
        List<string> skel = ListFiles(@"Assets\Resources\", ".skel");
        foreach (string file in m2)
        {
            File.Copy(file, $"{Path.GetDirectoryName(file)}\\data.bytes", true);
        }
        foreach (string file in skin)
        {
            File.Copy(file, $"{Path.GetDirectoryName(file)}\\skin.bytes", true);
        }
        foreach (string file in skel)
        {
            File.Copy(file, $"{Path.GetDirectoryName(file)}\\skel.bytes", true);
        }
    }

    //Menu option that clears *.bytes files created fpr import process.
    [MenuItem("WoW Character Viewer/Cleanup *.bytes files")]
    public static void CleanupBytesFiles()
    {
        List<string> files = ListFiles(@"Assets\Resources\", ".bytes");
        foreach (string file in files)
        {
            File.Delete(file);
        }
    }

    [MenuItem("Assets/Generate *.bytes files")]
    public static void GenerateBytesFilesForFolder()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!Directory.Exists(path))
        {
            path = Path.GetDirectoryName(path);
        }
        List<string> m2 = ListFiles(path, ".m2");
        List<string> skin = ListFiles(path, ".skin");
        List<string> skel = ListFiles(path, ".skel");
        foreach (string file in m2)
        {
            File.Copy(file, $"{Path.GetDirectoryName(file)}\\data.bytes", true);
        }
        foreach (string file in skin)
        {
            File.Copy(file, $"{Path.GetDirectoryName(file)}\\skin.bytes", true);
        }
        foreach (string file in skel)
        {
            File.Copy(file, $"{Path.GetDirectoryName(file)}\\skel.bytes", true);
        }
    }

    //Recursively search for all files with given extension in given directory.
    private static List<string> ListFiles(string path, string extension)
    {
        List<string> files = new List<string>();
        DirectoryInfo dir = new DirectoryInfo(path);
        foreach (FileInfo file in dir.GetFiles())
        {
            if (file.Extension == extension)
            {
                files.Add(file.FullName);
            }
        }
        foreach(DirectoryInfo d in dir.GetDirectories())
        {
            files.AddRange(ListFiles(d.FullName, extension));
        }
        return files;
    }
}
