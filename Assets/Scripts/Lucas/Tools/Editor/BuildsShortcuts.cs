using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class BuildsShortcuts : MonoBehaviour
{
    /* OpenLastBuildFolder :
	*
	* A series of shortcuts integrated in the Unity's top menu bar
    * that have for purpose to facilitate the access to builds
    * (Folders & .exe)
    * 
    * Contains 3 shortucts :
    *       - Launch the last build
    *       - Open the builds folder
    *       - Open the last build directory
	*/

    #region Fields
    // The default builds folder
    public static string buildsFolder = (Application.dataPath.Replace("/Assets", string.Empty) + "/Build");
    #endregion

    #region Methods

    #region Menu Shortcuts
    // Launches the last build made (.exe file)
    [MenuItem("Tools/Builds Shortcuts/Launch Last Build")]
    public static void LaunchLastBuild()
    {
        // Creates the builds folder if it does not already exists
        CreateDirectory(buildsFolder);

        // Get all builds folder
        string[] _buildFolders = Directory.GetDirectories(buildsFolder);

        // If at least one folder is found, try to find the .exe file in the last edited one
        if (_buildFolders != null && _buildFolders.Length > 0)
        {
            // Get the last edited folder
            string _buildFolder = _buildFolders.OrderBy(f => Directory.GetLastWriteTime(f)).Last();

            // Get the .exe file path
            string _exeFilePath = _buildFolder + '/' + Application.productName + ".exe";

            // If the .exe file exists, launch it
            if (File.Exists(_exeFilePath))
            {
                Process.Start(_exeFilePath);
            }
            // If it does not exist, debug it and open the build directory
            else
            {
                Debug.LogWarning("There is no .exe file in the last build folder or it has a different name !");

                Process.Start(_buildFolder);
            }
        }
        // If there's no folder, debug it
        else
        {
            Debug.LogWarning("There is no build in the default build folder !");
        }
    }

    // Opens the default build folder
    [MenuItem("Tools/Builds Shortcuts/Open Builds Folder")]
    public static void OpenBuildsFolder()
    {
        // Creates the builds folder if it does not already exists
        CreateDirectory(buildsFolder);

        // Open the builds folder
        Process.Start(buildsFolder);
    }

    // Opens the last build made directory
    [MenuItem("Tools/Builds Shortcuts/Open Last Build Directory")]
	public static void OpenLastBuildDirectory()
    {
        // Creates the builds folder if it does not already exists
        CreateDirectory(buildsFolder);

        // Get all builds folder
        string[] _buildFolders = Directory.GetDirectories(buildsFolder);

        // If at least one folder is found, open the last edited one
        if (_buildFolders != null && _buildFolders.Length > 0)
        {
            Process.Start(_buildFolders.OrderBy(f => Directory.GetLastWriteTime(f)).Last());
        }
        // If there's no folder, debug it
        else
        {
            Debug.LogWarning("There is no build in the default build folder !");
        }
    }
    #endregion

    // Creates a directory path if it does not already exists
    private static void CreateDirectory(string _path)
    {
        if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);
    }

    #endregion
}
