using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftScript
{
    public class Executor
    {
        public static void ExecuteNodes(List<Node> nodes, int localCount)
        {
            int i = 0;
            int length = nodes.Count;

            var stack = new Stack<dynamic>();
            dynamic[] vars = new dynamic[localCount];

            while (i < length)
            {
                int pi = i;
                var node = nodes[i];
                switch (node.NodeType)
                {
                    case NodeType.LoadLocal:
                        stack.Push(vars[node.A]);
                        break;
                    case NodeType.JumpNotTrue:
                        if (!stack.Pop())
                        {
                            i = node.A;
                        }
                        break;
                    case NodeType.NotEqual:
                        stack.Push(stack.Pop() != stack.Pop());
                        break;
                    case NodeType.Equal:
                        stack.Push(stack.Pop() == stack.Pop());
                        break;
                    case NodeType.LoadNumLiteral:
                        stack.Push(node.B);
                        break;
                    case NodeType.LoadNumLiteralAndStoreLocal:
                        vars[node.A] = node.B;
                        break;
                    case NodeType.LoadLocalAndStoreLocal:
                        vars[node.A] = vars[node.C];
                        break;
                    case NodeType.LoadLocalWriteConsole:
                        Console.WriteLine(vars[node.A]);
                        break;
                    case NodeType.LoadNumLiteralWriteConsole:
                        Console.WriteLine(node.B);
                        break;
                    case NodeType.Increment:
                        vars[node.A]++;
                        break;
                    case NodeType.Decrement:
                        vars[node.A]--;
                        break;
                    case NodeType.NoOperation:
                        break;
                    default:
                        break;
                }
                if (pi == i)
                {
                    i++;
                }
            }
        }
    }
}
