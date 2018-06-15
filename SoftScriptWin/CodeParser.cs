using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoftScript.Program;

namespace SoftScript
{
    public class CodeParser
    {
        public static (List<Node> nodes, int variableCount) Parse(string source)
        {
            var Nodes = new List<Node>();
            var variables = new Dictionary<string, int>();

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

            }

            return (Nodes, variables.Count);
        }
    }
}
