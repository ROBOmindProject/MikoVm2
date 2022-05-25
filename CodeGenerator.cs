using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MikoVm
{
    /**
     * 中間コード生成クラス
     */
    class CodeGenerator
    {
        const int STATE_START       = 0x0001;     // 開始状態
        const int STATE_ASSIGN      = 0x0002;     // 代入文
        const int STATE_EXPRESSION  = 0x0004;     // 式
        const int STATE_PRINT       = 0x0008;     // print文
        const int STATE_BEFORE_END  = 0x0010;     // 文終端（；）の前

        /**
         * バイトコードを生成
         */
        public static Bytecode[] GenerateCodes(Token[] tokens)
        {
            // 状態変数
            int state = STATE_START;

            // バイトコードバッファ
            var bytecode_buffer = new List<Bytecode>();

            // 変数テーブル
            var var_tbl = new VariableTable();

            // 代入先変数のID
            int assign_var_id = 0;

            // １トークンずつ処理
            foreach (var tk in tokens)
            {
                // 状態毎の処理
                if (state == STATE_START)
                {
                    if (tk.TokenType == Token.EnumTokenType.T_VARIABLE)
                    {
                        // 【開始】状態のとき変数が来たら、変数を変数テーブルに登録して【代入文】状態に遷移
                        assign_var_id = var_tbl.RegisterVariable(tk.TokenString);
                        state = STATE_ASSIGN;
                    }
                    else if (tk.TokenType == Token.EnumTokenType.T_PRINT)
                    {
                        // 【開始】状態のときprint文が来たら、【print文】状態に遷移
                        state = STATE_PRINT;
                    }
                    else
                    {
                        throw new SyntaxError($"Syntax error found at token({tk})");
                    }
                }
                else if (state == STATE_ASSIGN)
                {
                    if (tk.TokenType == Token.EnumTokenType.T_EQUAL)
                    {
                        // 【代入文】状態のときイコール（＝）が来たら、【式】状態に遷移
                        state = STATE_EXPRESSION;
                    }
                    else
                    {
                        throw new SyntaxError($"Syntax error found at token({tk})");
                    }
                }
                else if (state == STATE_EXPRESSION)
                {
                    if (tk.TokenType == Token.EnumTokenType.T_EXPRESSION)
                    {
                        // 【式】状態のとき式トークン（四則演算子or数字）が来たら、逆ポーランド記法に変換してバイトコード化する
                        WriteExpressionCode(tk.TokenString, bytecode_buffer);
                        // 代入文を出力
                        var bc = new Bytecode(Bytecode.EnumOpecode.opcPOPV, assign_var_id);
                        bytecode_buffer.Add(bc);
                        // 【終端前】状態に遷移
                        state = STATE_BEFORE_END;
                    }
                    else
                    {
                        throw new SyntaxError($"Syntax error found at token({tk})");
                    }
                }
                else if (state == STATE_PRINT)
                {
                    if (tk.TokenType == Token.EnumTokenType.T_VARIABLE)
                    {
                        // 【print文】状態のとき変数がきたら変数テーブルを検索してprint文のバイトコードを出力
                        int var_id = var_tbl.FindVariable(tk.TokenString);
                        var bc = new Bytecode(Bytecode.EnumOpecode.opcPRINT, var_id);
                        bytecode_buffer.Add(bc);
                        // 【終端前】状態に遷移
                        state = STATE_BEFORE_END;
                    }
                    else
                    {
                        throw new SyntaxError($"Syntax error found at token({tk})");
                    }
                }
                else if (state == STATE_BEFORE_END)
                {
                    if (tk.TokenType == Token.EnumTokenType.T_END_OF_STATEMENT)
                    {
                        // 【開始】状態に遷移
                        state = STATE_START;
                    }
                    else
                    {
                        throw new SyntaxError($"Syntax error found at token({tk})");
                    }
                }
            }

            return bytecode_buffer.ToArray();
        }

        /**
         * 中置記法の式を逆ポーランド記法に変換し、バイトコードバッファに出力する
         */
        private static void WriteExpressionCode(string expr, List<Bytecode> buffer)
        {
            // RPNクラスを使って数式を逆ポーランド記法に変換
            List<string> rpn = RPN.Convert(expr);

            // 逆ポーランド記法の順で処理
            foreach(var e in rpn)
            {
                if (Regex.IsMatch(e, "^[0-9]+$"))
                {
                    // 数値の場合はPush
                    var bc = new Bytecode(Bytecode.EnumOpecode.opcPUSH, Int32.Parse(e));
                    buffer.Add(bc);
                }
                else if (e == "+")
                {
                    buffer.Add(new Bytecode(Bytecode.EnumOpecode.opcADD));
                }
                else if (e == "-")
                {
                    buffer.Add(new Bytecode(Bytecode.EnumOpecode.opcSUB));
                }
                else if (e == "*")
                {
                    buffer.Add(new Bytecode(Bytecode.EnumOpecode.opcMUL));
                }
                else if (e == "/")
                {
                    buffer.Add(new Bytecode(Bytecode.EnumOpecode.opcDIV));
                }
            }
        }
    }
}
