using System;
using System.Collections.Generic;
using System.Linq;

namespace DiskTree;

public static class DiskTreeTask
{
    public static List<string> Solve(List<string> directoryPaths)
    {
        var rootDirectoryNode = new TreeNode { IsRootNode = true };
        foreach (var directory in directoryPaths)
            rootDirectoryNode.Add(directory.Split('\\'), 0);
        return rootDirectoryNode.GetDirectories(0).ToList();
    }
}

public class TreeNode
{
    private string directoryName;
    private readonly List<TreeNode> subdirectories = new List<TreeNode>();
    private bool isRootNode;

    public string DirectoryName
    {
        get { return directoryName; }
        private set { directoryName = value; }
    }

    public bool IsRootNode
    {
        get { return isRootNode; }
        set { isRootNode = value; }
    }

    public void Add(string[] directoryPathParts, int currentIndex)
    {
        if (currentIndex == directoryPathParts.Length)
            return;
        var folder = subdirectories.Find(x => x.DirectoryName == directoryPathParts[currentIndex]);
        if (folder == null)
        {
            folder = new TreeNode { DirectoryName = directoryPathParts[currentIndex] };
            subdirectories.Add(folder);
        }

        folder.Add(directoryPathParts, currentIndex + 1);
    }

    public IEnumerable<string> GetDirectories(int currentOffset = 0)
    {
        if (!IsRootNode)
        {
            yield return new string(' ', currentOffset) + DirectoryName;
            currentOffset++;
        }

        var subDirectoryPaths = subdirectories
            .OrderBy(childNode => childNode.DirectoryName, StringComparer.Ordinal)
            .SelectMany(childNode => childNode.GetDirectories(currentOffset));
        foreach (var directoryPath in subDirectoryPaths)
            yield return directoryPath;
    }
}

