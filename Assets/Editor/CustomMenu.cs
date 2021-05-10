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
        List<string> m2 = ListFiles(@"Assets\Resources\character", ".m2");
        List<string> skin = ListFiles(@"Assets\Resources\character", ".skin");
        List<string> skel = ListFiles(@"Assets\Resources\character", ".skel");
        foreach (string file in m2)
        {
            File.Copy(file, Path.GetDirectoryName(file) + "\\data.bytes", true);
        }
        foreach (string file in skin)
        {
            File.Copy(file, Path.GetDirectoryName(file) + "\\skin.bytes", true);
        }
        foreach (string file in skel)
        {
            File.Copy(file, Path.GetDirectoryName(file) + "\\skel.bytes", true);
        }
    }

    //Menu option that clears *.bytes files created fpr import process.
    [MenuItem("WoW Character Viewer/Cleanup *.bytes files")]
    public static void CleanupBytesFiles()
    {
        List<string> files = ListFiles(@"Assets\Resources\character", ".bytes");
        foreach (string file in files)
        {
            File.Delete(file);
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
