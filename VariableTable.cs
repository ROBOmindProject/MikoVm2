using System;
using System.Collections.Generic;
using System.Text;

namespace MikoVm
{
    /**
     * 変数テーブル
     */
    class VariableTable
    {
        /**
         * 変数名に対するID（整数）のマップ
         */
        private Dictionary<string, int> _map = new Dictionary<string, int>();

        /**
         * 一意となるID（整数）を生成するためのフィールド
         */
        private int _seq = 0;

        /**
         * 変数を登録
         */
        public int RegisterVariable(string var_name)
        {
            // すでに登録済みの場合はそのIDを返すだけ
            if (_map.ContainsKey(var_name))
            {
                return _map[var_name];
            }

            // 新しいIDを生成し、マップに登録して返す
            int new_id = _seq++;
            _map[var_name] = new_id;

            return new_id;
        }

        /**
         * 変数を検索
         */
        public int FindVariable(string var_name)
        {
            return _map.ContainsKey(var_name) ? _map[var_name] : -1;
        }
    }
}
