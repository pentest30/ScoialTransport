using System.Collections;
using System.Linq;
using System.Resources;
using System.Threading;
using Newtonsoft.Json;

namespace TachographReader.Web.Helpers
{
    public class ResourcesHelper
    {
        public static string GenerateResxJSON<T>()
        {
            var currentCulture = Thread.CurrentThread.CurrentUICulture;
            ResourceManager rm = new ResourceManager(typeof(T));
            var entries = rm.GetResourceSet(currentCulture, true, true).OfType<DictionaryEntry>().ToDictionary(x=>x.Key, y=>y.Value);
            return JsonConvert.SerializeObject(entries);
        }
    }
}
