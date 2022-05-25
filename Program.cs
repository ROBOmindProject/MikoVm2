using System;
using System.Collections.Generic;

namespace MikoVm
{
    class Program
    {
        static void Main(string[] args)
        {
            // ソースコード
            string src = @"
a = 2 * (5 - 1) + 6 / 3;
print a;
";
            // パース処理
            var tokens = Parser.Parse(src);

            Console.WriteLine("\nトークン：");
            foreach (var tk in tokens)
            {
                Console.WriteLine($"{tk}");
            }

            // バイトコード生成
            Bytecode[] codes = CodeGenerator.GenerateCodes(tokens);

            Console.WriteLine("\nバイトコード：");
            foreach (var bc in codes)
            {
                Console.WriteLine($"{bc}");
            }

            // VM実行
            Console.WriteLine("\nVMの実行結果：");
            MikoVM.Run(codes);

            Console.WriteLine("\n(何かキーを押すと終了します)");
            Console.ReadLine();
        }
    }
}
