using System;
using System.Collections.Generic;
using System.Text;

namespace MikesTweaks.Scripts.Utilities
{
    public static class StringUtils
    {
        public static void RemoveChar(ref string str, char character)
        {
            List<int> Indexes = new List<int>();
            for (int i = str.Length-1; i >= 0; i--)
            {
                if (str[i] == character)
                    Indexes.Add(i);
            }

            for (int i = 0; i < Indexes.Count; i++)
            {
                str = str.Remove(Indexes[i], 1);
            }
        }
    }
}
