using System.Runtime.CompilerServices;

namespace Fenrus.Helpers;

/// <summary>
/// Helper for directory locations
/// </summary>
public class DirectoryHelper
{
    /// <summary>
    /// Gets the full path to the root Fenrus directory
    /// </summary>
    /// <returns>the full path to the root Fenrus directory</returns>
    public static string GetBaseDirectory()
    {
#if(DEBUG)
        string dir = AppDomain.CurrentDomain.BaseDirectory;
        dir = dir.Substring(0, dir.IndexOf("bin") - 1);
        return dir;
#else
        return AppDomain.CurrentDomain.BaseDirectory;
#endif
    }

    /// <summary>
    /// Gets the apps root directory
    /// </summary>
    /// <returns>the apps root directory</returns>
    public static string GetAppsDirectory()
        => Path.Combine(GetBaseDirectory(), "Apps");
    
    /// <summary>
    /// Gets the smart apps directory
    /// </summary>
    /// <returns>the smart apps directory</returns>
    public static string GetSmartAppsDirectory()
        => Path.Combine(GetAppsDirectory(), "Smart");
    
    /// <summary>
    /// Gets the basic apps directory
    /// </summary>
    /// <returns>the basic apps directory</returns>
    public static string GetBasicAppsDirectory()
        => Path.Combine(GetAppsDirectory(), "Basic");
    
    /// <summary>
    /// Gets the full path to wwwroot
    /// </summary>
    /// <returns>the full path to wwwroot</returns>
    public static string GetWwwRootDirectory()
        => Path.Combine(GetBaseDirectory(), "wwwroot");
}