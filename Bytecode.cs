using System;
using System.Collections.Generic;
using System.Text;

namespace MikoVm
{
    class Bytecode
    {
        public enum EnumOpecode
        {
            opcPUSH,            // 指定した整数値をスタックにPush：　オペランド(1)=>整数値
            opcPUSHV,           // 指定した変数の内容をスタックにPush：　オペランド(1)=>変数ID
            opcPOPV,            // スタックから値を１つPopして指定した変数にセット：　オペランド(1)=>変数ID
            opcADD,             // スタックから２つの値をポップして和を計算、結果をスタックにPush：　オペランドなし
            opcSUB,             // スタックから２つの値をポップして差を計算、結果をスタックにPush：　オペランドなし
            opcMUL,             // スタックから２つの値をポップして積を計算、結果をスタックにPush：　オペランドなし
            opcDIV,             // スタックから２つの値をポップして除算、結果をスタックにPush：　オペランドなし
            opcPRINT            // スタックから値を１つPopしてコンソールに出力する：　オペランドなし
        }

        public EnumOpecode Opecode { get; }
        public List<int> Operands { get; }

        /**
         * コンストラクタ
         */
        public Bytecode(EnumOpecode opecode)
        {
            Opecode = opecode;
            Operands = new List<int>();
        }
        public Bytecode(EnumOpecode opecode, int operand_1)
        {
            Opecode = opecode;
            Operands = new List<int>()
            {
                operand_1
            };
        }

        /**
         * 文字列表現
         */
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{Opecode}");
            if (Operands.Count > 0)
            {
                sb.Append('(');
                sb.Append(string.Join(',', Operands));
                sb.Append(')');
            }
            return sb.ToString();
        }
    }
}
