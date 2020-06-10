using System.Collections;
using System.Collections.Generic;

public class TreeNode<T1, T2>
{
    public KeyValuePair<T1, T2> NodeKeyValue { get; set; }
    public int nodeDepth { get; }
    public TreeNode<T1, T2> ParentNode { get; }

    public TreeNode(KeyValuePair<T1, T2> nKV, int d, TreeNode<T1, T2> parent)
    {
        NodeKeyValue = nKV;
        nodeDepth = d;
        ParentNode = parent;
    }

    public TreeNode(T1 key, T2 value, int d, TreeNode<T1, T2> parent): this(new KeyValuePair<T1, T2>(key, value), d, parent){}
}


public class TreeNodeComplete<T1, T2>
{
    public KeyValuePair<T1, T2> NodeKeyValue { get; set; }
    public int nodeDepth { get; }
    public TreeNodeComplete<T1, T2> ParentNode { get; }
    public List<TreeNodeComplete<T1, T2>> ChildNodes { get; set; }

    public TreeNodeComplete(T1 key, T2 value, int d, TreeNodeComplete<T1, T2> parent) : this(new KeyValuePair<T1, T2>(key, value), d, parent) { }

    public TreeNodeComplete(KeyValuePair<T1, T2> nKV, int d, TreeNodeComplete<T1, T2> parent)
    {
        NodeKeyValue = nKV;
        nodeDepth = d;
        ParentNode = parent;
        ChildNodes = null;
    }

    public TreeNodeComplete(KeyValuePair<T1, T2> nKV, int d, TreeNodeComplete<T1, T2> parent, List<TreeNodeComplete<T1, T2>> cN)
    {
        NodeKeyValue = nKV;
        nodeDepth = d;
        ParentNode = parent;
        ChildNodes = cN;
    }
}
/*
public class NTree<T1, T2>
{
    public int nodesCount { get; set; }
    public TreeNode<T1, T2> root { get; }
    public HashSet<T1> NodesSet { get; set; }
    public HashSet<T1> LeavesSet { get; set; }



    public NTree(TreeNode<T1, T2> rootNode)
    {
        root = rootNode;
        nodesCount = 1;
        NodesSet = new HashSet<T1>();
        NodesSet.Add(rootNode.NodeKeyValue.Key);
    }

    public bool isNodeInTree(TreeNode<T1, T2> parentNode)
    {
        return NodesSet.Contains(parentNode.NodeKeyValue.Key);
    }

    public void addLeafNode(TreeNode<T1, T2> parent, TreeNode<T1, T2> child)
    {
        
    }
}*/
