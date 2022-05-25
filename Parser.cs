using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MikoVm
{
    class Parser
    {
        const int STATE_START       = 0x0001;     // 開始状態
        const int STATE_VARIABLE    = 0x0002;     // 変数
        const int STATE_POST_VAR    = 0x0004;     // 変数後
        const int STATE_STATEMENT   = 0x0008;     // ステートメント
        const int STATE_POST_STMT   = 0x0010;     // ステートメント後
        const int STATE_EXPRESSION  = 0x0020;     // 式

        public static Token[] Parse(string src)
        {
            var tokens = new List<Token>();
            var buffer = new StringBuilder();
            var state = STATE_START;

            // 改行コードを統一(CRLF/CR=>LF)
            src = src.Replace("\r\n", "\n").Replace('\r', '\n');

            // １文字ずつ処理
            int line = 0;
            foreach (char c in src)
            {
                // 改行処理
                if (c == '\n')
                {
                    line++;
                    state = STATE_START;
                    // バッファが空でなければトークンを追加
                    if (buffer.Length > 0)
                    {
                        tokens.Add(DetectToken(buffer.ToString()));
                        buffer.Clear();
                    }
                    continue;
                }

                // 文の終了処理(;)
                if (c == ';')
                {
                    // ;の場合は文終了なので[開始]状態に遷移
                    state = STATE_START;
                    // トークンを追加
                    if (buffer.Length > 0)
                    {
                        tokens.Add(DetectToken(buffer.ToString()));
                        buffer.Clear();
                    }
                    tokens.Add(new Token(Token.EnumTokenType.T_END_OF_STATEMENT, ";"));
                    continue;
                }

                // 状態毎の処理
                if (state == STATE_START)
                {
                    // 開始状態
                    if (IsAlphabet(c))
                    {
                        // アルファベットなら変数または文の開始
                        state = STATE_VARIABLE | STATE_STATEMENT;
                        buffer.Append(c);
                    }
                    else if (c != ' ')
                    {
                        throw new CompilerError($"パース失敗: {c} at line: {line}");
                    }
                }
                else if (state == (STATE_VARIABLE | STATE_STATEMENT) || state == STATE_VARIABLE)
                {
                    // 変数またはステートメントの場合
                    if (IsAlphabet(c) || IsNumber(c))
                    {
                        // アルファベットまたは数字ならバッファに追加
                        buffer.Append(c);
                    }
                    else if (c == ' ')
                    {
                        // 変数かステートメントかを判定
                        var token = DetectToken(buffer.ToString());
                        // 変数なら[変数後]状態に遷移、ステートメントなら[ステートメント後]状態に遷移
                        state = token.TokenType == Token.EnumTokenType.T_VARIABLE ? STATE_POST_VAR : STATE_POST_STMT;
                        // トークンを追加
                        tokens.Add(token);
                        buffer.Clear();
                    }
                    else
                    {
                        throw new CompilerError($"パース失敗: {c} at line: {line}");
                    }
                }
                else if (state == STATE_POST_VAR)
                {
                    // 変数の後
                    if (c == '=')
                    {
                        // =の場合は[式]状態に遷移
                        state = STATE_EXPRESSION;
                        // トークンを追加
                        buffer.Append('=');
                        tokens.Add(DetectToken(buffer.ToString()));
                        buffer.Clear();
                    }
                    else if (c != ' ')
                    {
                        throw new CompilerError($"パース失敗: {c} at line: {line}");
                    }
                }
                else if (state == STATE_POST_STMT)
                {
                    // ステートメント後
                    if (IsAlphabet(c))
                    {
                        // アルファベットの場合は[変数]状態に遷移
                        state = STATE_VARIABLE;
                        buffer.Append(c);
                    }
                    else if (c != ' ')
                    {
                        throw new CompilerError($"パース失敗: {c} at line: {line}");
                    }
                }
                else if (state == STATE_EXPRESSION)
                {
                    // 式
                    if (IsExpression(c))
                    {
                        // 式を構成する文字ならバッファに追加（式先頭の空白は無視）
                        if (c != ' ' || buffer.Length > 0)
                        {
                            buffer.Append(c);
                        }
                    }
                    else
                    {
                        throw new CompilerError($"パース失敗: {c} at line: {line}");
                    }
                }
                else
                {
                    throw new CompilerError($"パース失敗: {c} at line: {line}");
                }
            }

            return tokens.ToArray();
        }

        /**
         * アルファベットか
         */
        private static bool IsAlphabet(char c)
        {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z';
        }

        /**
         * 数字か
         */
        private static bool IsNumber(char c)
        {
            return c >= '0' && c <= '9';
        }

        /**
         * 式か
         */
        private static bool IsExpression(char c)
        {
            return c >= '0' && c <= '9' || c == ' ' || c == '+' || c == '-' || c == '*' || c == '/' || c == ')' || c == '(';
        }

        /**
         * トークンを判定
         */
        private static Token DetectToken(string str)
        {
            var map = new Dictionary<string, Token>()
            {
                { "=", new Token(Token.EnumTokenType.T_EQUAL, str) },
                { ";", new Token(Token.EnumTokenType.T_END_OF_STATEMENT, str) },
                { "print", new Token(Token.EnumTokenType.T_PRINT, str) },
            };

            // 固定トークン
            if (map.ContainsKey(str))
            {
                return map[str];
            }

            // 正規表現で判定するトークン
            if (Regex.IsMatch(str, "^[0-9]+$"))
            {
                // 整数定数
                return new Token(Token.EnumTokenType.T_LIT_INTEGER, str);
            }
            if (Regex.IsMatch(str, @"^[0-9\+\-\*\/\s\(\)]+$"))
            {
                // 式
                return new Token(Token.EnumTokenType.T_EXPRESSION, str);
            }
            if (Regex.IsMatch(str, @"^[a-zA-Z]{1}[a-zA-Z0-9]*$"))
            {
                // 変数
                return new Token(Token.EnumTokenType.T_VARIABLE, str);
            }

            // 不明
            return new Token(Token.EnumTokenType.T_UNKNOWN, str);
        }
    }
}
