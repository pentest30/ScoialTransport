using System.Reflection;

namespace tacchograaph_reader.Core.Linq
{
    public static class LinqHelper
    {
        // be careful when using this method because it can raise an exception (index out of range ) or brings the false the property name
        // to use it safely the order of datatables columns must corresponds exactly the order of the model
        public static string GetPropertyNameByIndex<T>(int idenx)
        {
            PropertyInfo[] propInfos = typeof(T).GetProperties();
            return propInfos[idenx + 1].Name;
        }

    }

}
