using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MikoVm
{
    /**
     * 逆ポーランド記法変換クラス
     */
    class RPN
    {
        /**
         * 中置記法で記載された数式を、逆ポーランド記法に変換する
         */
        public static List<string> Convert(string input)
        {
            var stack = new Stack<string>();
            var output = new List<string>();

            var input_split = Split(input);

            // 要素ごとに処理
            foreach (var e in input_split)
            {
                if (Regex.IsMatch(e, "^[0-9]+$"))
                {
                    // 数字ならそのまま出力
                    output.Add(e);
                }
                else if (e == "(")
                {
                    // 開き括弧ならばスタックにプッシュ
                    stack.Push(e);
                }
                else if (e == ")")
                {
                    // 閉じ括弧ならば開き括弧が出るまで出力
                    bool exit_loop = false;
                    while (stack.Count > 0 && !exit_loop)
                    {
                        var item = stack.Pop();

                        if (item == "(")
                        {
                            exit_loop = true;
                        }
                        else if (Regex.IsMatch(item, @"^[\+\-\*\/]{1}$") || Regex.IsMatch(item, "^[0-9]+$"))
                        {
                            output.Add(item);
                        }
                    }
                }
                else if (Regex.IsMatch(e, @"^[\+\-\*\/]{1}$"))
                {
                    // 四則演算子（＋ー×÷）の場合、スタックが空またはスタックトップの演算子より優先度が高いならプッシュ、それ以外
                    // ならスタックすべての要素をPopして出力
                    if (stack.Count  == 0)
                    {
                        stack.Push(e);
                    }
                    else{
                        var item = stack.Peek();
                        if (Regex.IsMatch(item, @"^[\+\-\*\/\(\)]{1}$"))
                        {
                            if (CompareOperatorPriority(e, item) > 0)
                            {
                                stack.Push(e);
                            }
                            else
                            {
                                while (stack.Count > 0)
                                {
                                    output.Add(stack.Pop());
                                }
                                stack.Push(e);
                            }
                        }
                    }
                }
            }

            // スタックに残ったデータをすべて出力
            while (stack.Count > 0)
            {
                var item = stack.Pop();
                output.Add(item);
            }

            return output;
        }

        /**
         * 中置記法の数式を記号、数字のリストに変換
         */
        private static List<string> Split(string input)
        {
            // 中置記法で書かれた式を記号（＋ー×÷or括弧）と数字に分割する。ただし数字の場合は2桁以上の整数の可能性があるのでバッファを使う
            var ret = new List<string>();
            var num_buffer = new List<char>();
            foreach (char c in input)
            {
                if (c == '+' || c == '-' || c == '*' || c == '/' || c == '(' || c == ')')
                {
                    if (num_buffer.Count > 0)
                    {
                        ret.Add(new string(num_buffer.ToArray()));
                        num_buffer.Clear();
                    }
                    ret.Add(c.ToString());
                }
                else if (c >= '0' && c <= '9')
                {
                    num_buffer.Add(c);
                }
            }
            if (num_buffer.Count > 0)
            {
                ret.Add(new string(num_buffer.ToArray()));
                num_buffer.Clear();
            }

            return ret;
        }

        /**
         * 演算子の優先度を比較
         */
        private static int CompareOperatorPriority(string op1, string op2)
        {
            // ×÷なら2点、＋ーなら１点、それ以外なら０点
            int point_1 = (op1 == "*" || op1 == "/") ? 2 : (op1 == "+" || op1 == "-") ? 1 : 0;
            int point_2 = (op2 == "*" || op2 == "/") ? 2 : (op2 == "+" || op2 == "-") ? 1 : 0;
            return point_1 - point_2;
        }
    }
}
