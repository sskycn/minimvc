using System.Configuration;
using System.Reflection;
using ComMiniMvc.Mini.IDAL;

namespace ComMiniMvc.Mini.DALFactory
{
    public sealed class DataAccess
    {
        private static readonly string path = ConfigurationManager.AppSettings["WebDAL"];

        private DataAccess() { }
        
        public static ITest CreateTest()
        {
            string className = path + ".Test";
            return (ITest)Assembly.Load(path).CreateInstance(className);
        }
        
        //**{$ClassEnd}--//
    }
}
