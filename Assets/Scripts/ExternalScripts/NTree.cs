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