using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftScript
{
    public class Node
    {
        public NodeType NodeType;
        public int A;
        public decimal B;
        public int C;
        public Node(NodeType nodeType)
        {
            NodeType = nodeType;
        }
        public Node(NodeType nodeType, int a) : this(nodeType)
        {
            A = a;
        }
        public Node(NodeType nodeType, decimal b) : this(nodeType)
        {
            B = b;
        }
        public Node(NodeType nodeType, int a, decimal b) : this(nodeType)
        {
            A = a;
            B = b;
        }
        public Node(NodeType nodeType, int a, int c) : this(nodeType)
        {
            A = a;
            C = c;
        }
    }

    public enum NodeType
    {
        LoadLocal,
        JumpNotTrue,
        NotEqual,
        Equal,
        LoadNumLiteral,
        LoadNumLiteralAndStoreLocal,
        LoadLocalAndStoreLocal,
        LoadLocalWriteConsole,
        LoadNumLiteralWriteConsole,
        Increment,
        Decrement,
        NoOperation
    }
}
