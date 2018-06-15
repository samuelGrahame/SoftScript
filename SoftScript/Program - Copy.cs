using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftScript
{
    public class Program
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
                        if(!stack.Pop())
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
                if(pi == i)
                {
                    i++;
                }
            }
        }

        public static int GetVarIndex(string name, Dictionary<string, int> valuePairs)
        {
            if (valuePairs.ContainsKey(name))
            {
                return valuePairs[name];
            }
            else
            {
                return valuePairs[name] = valuePairs.Count;
            }
        }

        public static void Main()
        {
            var Nodes = new List<Node>();
            var variables = new Dictionary<string, int>();
            string source =
@"
a equals 10
b equals 15

decrement b
decrement b
decrement b
decrement b
decrement b

if a variable named a is not equal to a variable named b then 
    a equals b
end

write a to the console
";
            Console.WriteLine(source);
            bool inExpression = false;
            NodeType currentMod = NodeType.NoOperation;

            Queue<Node> BlockLevel = new Queue<Node>();
            
            using (TokenReader tr = new TokenReader(source))
            {
                do
                {
                    if (string.IsNullOrWhiteSpace(tr.Current()))
                        continue;
                    if (tr.EqualTo("if"))
                    {
                        BlockLevel.Enqueue(new Node(NodeType.JumpNotTrue));
                        inExpression = true;
                    }
                    else if (tr.EqualTo("then"))
                    {
                        Nodes.Add(BlockLevel.Peek());
                        inExpression = false;
                    }
                    else if (tr.EqualTo("end"))
                    {
                        BlockLevel.Dequeue().A = Nodes.Count;
                        Nodes.Add(new Node(NodeType.NoOperation));

                    }
                    else if (tr.EqualTo("increment"))
                    {
                        if (tr.MoveNext() && !tr.IsNumberLiteral())
                        {                                                        
                            Nodes.Add(new Node(NodeType.Increment, 
                                GetVarIndex(tr.Current(), variables)));
                        }
                        else
                        {
                            throw new Exception("Expected a variable after increment");
                        }
                    }
                    else if (tr.EqualTo("decrement"))
                    {
                        if (tr.MoveNext() && !tr.IsNumberLiteral())
                        {                            
                            Nodes.Add(new Node(NodeType.Decrement,
                                GetVarIndex(tr.Current(), variables)));
                        }
                        else
                        {
                            throw new Exception("Expected a variable after increment");
                        }
                    }
                    else if (tr.EqualTo("write"))
                    {
                        if (tr.MoveNext())
                        {
                            int index = -1;
                            decimal value = 0;
                            if (tr.IsNumberLiteral())
                            {
                                value = tr.GetValue();
                            }
                            else
                            {
                                index = GetVarIndex(tr.Current(), variables);                               
                            }
                            if (tr.MoveNext())
                            {
                                if (tr.EqualTo("to", "the", "console"))
                                {
                                    if (index == -1)
                                    {
                                        Nodes.Add(new Node(NodeType.LoadNumLiteralWriteConsole, value));
                                    }
                                    else
                                    {
                                        Nodes.Add(new Node(NodeType.LoadLocalWriteConsole, index));
                                    }

                                    tr.MoveNext(2);
                                }
                            }
                            else
                            {
                                throw new Exception("Expected what to write to, after value!");
                            }
                        }
                        else
                        {
                            throw new Exception("Expected a value to write");
                        }
                    }
                    else if (tr.EqualTo("is", "not", "equal", "to"))
                    {
                        //   Nodes.Add(new Node(NodeType.NotEqual));
                        currentMod = NodeType.NotEqual;
                        tr.MoveNext(3);
                    }
                    else if (tr.EqualTo("is", "equal", "to"))
                    {
                        currentMod = NodeType.Equal;
                        //Nodes.Add(new Node(NodeType.Equal));
                        tr.MoveNext(2);
                    }
                    else if (tr.EqualTo("a", "variable", "named"))
                    {
                        if (tr.MoveNext(3))
                        {                            
                            Nodes.Add(new Node(NodeType.LoadLocal,
                                GetVarIndex(tr.Current(), variables)));

                            if (currentMod != NodeType.NoOperation)
                            {
                                Nodes.Add(new Node(currentMod));
                                currentMod = NodeType.NoOperation;
                            }
                        }
                        else
                        {
                            throw new Exception("Expected name after a variable named");
                        }
                    }
                    else
                    {
                        if (inExpression)
                        {
                            if (tr.IsNumberLiteral())
                            {
                                Nodes.Add(new Node(NodeType.LoadNumLiteral, tr.GetValue()));
                            }
                            else
                            {                                
                                Nodes.Add(new Node(NodeType.LoadLocal,
                                    GetVarIndex(tr.Current(), variables)));
                            }
                            if (currentMod != NodeType.NoOperation)
                            {
                                Nodes.Add(new Node(currentMod));
                                currentMod = NodeType.NoOperation;
                            }
                        }
                        else
                        {
                            int index = GetVarIndex(tr.Current(), variables);
                            if (tr.MoveNext() && tr.EqualTo("equals"))
                            {
                                tr.MoveNext();
                                if (tr.IsNumberLiteral())
                                {
                                    Nodes.Add(new Node(NodeType.LoadNumLiteralAndStoreLocal, index, tr.GetValue()));
                                }
                                else
                                {
                                    Nodes.Add(new Node(NodeType.LoadLocalAndStoreLocal, index,
                                        GetVarIndex(tr.Current(), variables)));
                                }
                            }
                            else
                            {
                                throw new Exception("Expected something after the variable");
                            }
                        }

                    }
                } while (tr.MoveNext());

                ExecuteNodes(Nodes, variables.Count);
            }
        }

        public class TokenReader : IDisposable
        {
            public string[] Words;
            private int _pos;
            public TokenReader(string source)
            {
                Words = source.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);

            }

            public bool MoveNext(int count = 1)
            {
                _pos+= count;
                return _pos < Words.Length;
            }

            public string Current()
            {
                return Words[_pos];
            }

            public bool IsCurrency()
            {
                return Words[_pos].StartsWith("$");
            }

            public decimal GetValue()
            {
                if(IsCurrency())
                {
                    return decimal.Parse(Words[_pos].Substring(1));
                }
                else
                {
                    return decimal.Parse(Words[_pos]);
                }
            }

            public bool IsNumberLiteral()
            {
                string x = Words[_pos];
                if(x.StartsWith("$"))
                {
                    x = x.Substring(1);
                }
                if (x.Length == 0)
                    return false;
                int TotalDots = 0;
                for (int i = 0; i < x.Length; i++)
                {
                    if(!char.IsNumber(x[i]))
                    {
                        if(x[i] == '.')
                        {
                            TotalDots++;
                            if (TotalDots == 1)
                                continue;                                
                        }
                        return false;
                    }
                }

                return true;

            }

            public bool EqualTo(params string[] words)
            {
                int Total = 0;
                int index = 0;
                for (int i = _pos; i < _pos+ words.Length && i < Words.Length; i++)
                {
                    if (words[index].ToLower() == Words[i].ToLower())
                        Total++;
                    else
                        return false;
                    index++;
                }
                return Total == words.Length;
            }

            public void Dispose()
            {
            }
        }
    }
}
