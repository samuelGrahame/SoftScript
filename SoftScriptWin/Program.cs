using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoftScript
{


    public class Program
    {
        

        

        

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
        [STAThread]
        public static void Main()
        {
            DevExpress.Skins.SkinManager.EnableFormSkins();
            Application.EnableVisualStyles();
            Application.Run(new frmStudio());


//            string source =
//@"
//a equals 10
//b equals 15

//decrement b
//decrement b
//decrement b
//decrement b
//decrement b

//if a variable named a is not equal to a variable named b then 
//    a equals b
//end

//write a to the console
//";
//            Console.WriteLine(source);
//            
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
