using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tacchograaph_reader.Core.Extensions
{
    public  static class ListExtension
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }
}
