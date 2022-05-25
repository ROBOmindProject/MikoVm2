using System;
using System.Collections.Generic;
using System.Text;

namespace MikoVm
{
    /**
     * 仮想マシン
     */
    class MikoVM
    {
        // 変数テーブルのサイズ
        const int VARTBLE_SIZE = 256;

        /**
         * バイトコードを実行
         */
        public static void Run(Bytecode[] codes)
        {
            // 実行時変数テーブル
            var var_tbl = new int[VARTBLE_SIZE];

            // 実行時スタック
            var stack = new Stack<int>();

            foreach(var bc in codes)
            {
                switch (bc.Opecode)
                {
                    case Bytecode.EnumOpecode.opcPUSH:            // 指定した整数値をスタックにPush：　オペランド(1)=>整数値
                        {
                            int data = bc.Operands[0];
                            stack.Push(data);
                        }
                        break;

                    case Bytecode.EnumOpecode.opcPUSHV:           // 指定した変数の内容をスタックにPush：　オペランド(1)=>変数ID
                        {
                            int data = var_tbl[bc.Operands[0]];
                            stack.Push(data);
                        }
                        break;

                    case Bytecode.EnumOpecode.opcPOPV:            // スタックから値を１つPopして指定した変数にセット：　オペランド(1)=>変数ID
                        {
                            if (stack.Count == 0)
                            {
                                throw new VmRuntimeError("Stack empty!");
                            }
                            var data = stack.Pop();
                            int var_id = bc.Operands[0];
                            var_tbl[var_id] = data;
                        }
                        break;

                    case Bytecode.EnumOpecode.opcADD:             // スタックから２つの値をポップして和を計算、結果をスタックにPush：　オペランドなし
                        {
                            if (stack.Count < 2)
                            {
                                throw new VmRuntimeError("Stack empty!");
                            }
                            var data1 = stack.Pop();
                            var data2 = stack.Pop();
                            stack.Push(data1 + data2);
                        }
                        break;

                    case Bytecode.EnumOpecode.opcSUB:             // スタックから２つの値をポップして差を計算、結果をスタックにPush：　オペランドなし
                        {
                            if (stack.Count < 2)
                            {
                                throw new VmRuntimeError("Stack empty!");
                            }
                            var data1 = stack.Pop();
                            var data2 = stack.Pop();
                            stack.Push(data2 - data1);
                        }
                        break;

                    case Bytecode.EnumOpecode.opcMUL:             // スタックから２つの値をポップして積を計算、結果をスタックにPush：　オペランドなし
                        {
                            if (stack.Count < 2)
                            {
                                throw new VmRuntimeError("Stack empty!");
                            }
                            var data1 = stack.Pop();
                            var data2 = stack.Pop();
                            stack.Push(data1 * data2);
                        }
                        break;

                    case Bytecode.EnumOpecode.opcDIV:             // スタックから２つの値をポップして除算、結果をスタックにPush：　オペランドなし
                        {
                            if (stack.Count < 2)
                            {
                                throw new VmRuntimeError("Stack empty!");
                            }
                            var data1 = stack.Pop();
                            var data2 = stack.Pop();
                            if (data1 == 0)
                            {
                                throw new VmRuntimeError("Divided by zero");
                            }
                            stack.Push(data2 / data1);
                        }
                        break;

                    case Bytecode.EnumOpecode.opcPRINT:           // 指定した変数の内容をコンソールに出力する：　オペランド(1)=>変数ID
                        {
                            int data = var_tbl[bc.Operands[0]];
                            Console.WriteLine(data);
                        }
                        break;
                }
            }
        }
    }
}
