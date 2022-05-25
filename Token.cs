using System;
using System.Collections.Generic;
using System.Text;

namespace MikoVm
{
    class Token
    {
        public enum EnumTokenType
        {
            T_UNKNOWN,              // 不明なトークン

            T_LIT_INTEGER,          // 整数定数
            T_EQUAL,                // =
            T_EXPRESSION,           // 式
            T_END_OF_STATEMENT,     // ;
            T_PRINT,                // print
            T_VARIABLE,             // 変数
        }

        public EnumTokenType TokenType { get; set; }
        public string TokenString { get; set; }

        /**
         * コンストラクタ
         */
        public Token(EnumTokenType type, string str)
        {
            TokenType = type;
            TokenString = str;
        }

        /**
         * 文字列表現
         */
        public override string ToString()
        {
            return $"{TokenType}({TokenString})";
        }
    }
}
