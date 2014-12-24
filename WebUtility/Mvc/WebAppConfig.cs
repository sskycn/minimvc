using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;
using Cvv.WebUtility.Core;
using Cvv.WebUtility.Core.Json;
using Cvv.WebUtility.Mini;
using Cvv.WebUtility.Mvc.Provider;

namespace Cvv.WebUtility.Mvc
{
    public static class WebAppConfig
    {
        private static readonly object _locked = new object();

        private static string _defaultLayout = "master";
        private static string _defaultLanguageCode = "en-US";
        private static string _defaultTheme = "Default";
        private static string _extension;

        private static string _appKey;
        private static string _appSecret;
        private static string _oAuthUri;
        private static string _publicKey;
        private static string _privateKey;

        private static Type _sessionType;
        private static Type _securityType;

        private static ITimeProvider _timeProvider;
        private static ILoggingProvider _loggingProvider;
        private static ISecurityProvider _securityProvider;
        private static IDeserializerProvider _deserializeProvider;
        private static ISerializerProvider _serializeProvider;
        private static ISessionSerializer _sessionSerializer;
        private static IStringFilter _stringFilterProvider;

        private static ISessionDataProvider _sessionDataProvider = new MiniSessionDataProvider();
        private static ISessionLoggingProvider _sessionLoggingProvider = new MiniSessionLoggingProvider();
        private static IVisitorProvider _visitorProvider = new MinimalVisitorProvider();

        private static readonly string _version;
        private static readonly int _templateCacheDuration;
        private static readonly Encoding _templateFileEncoding;

        private static Dictionary<string, ControllerClass> _controllerClasses = null;

        private static readonly List<Assembly> _registeredAssemblies = new List<Assembly>();

        private static readonly bool _enabledPermission = false;
        private static readonly bool _enabledHttpCompress;

        public static event Action<SessionBase> SessionCreated;

        static WebAppConfig()
        {
            string appClassName = ConfigurationManager.AppSettings["WebAppClassName"];
            _version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _templateCacheDuration = Convert.ToInt32(ConfigurationManager.AppSettings["TemplateCacheDuration"]);
            _templateFileEncoding = string.IsNullOrEmpty(ConfigurationManager.AppSettings["TemplateFileEncoding"]) ? Encoding.Default : Encoding.GetEncoding(ConfigurationManager.AppSettings["TemplateFileEncoding"]);

            bool.TryParse(ConfigurationManager.AppSettings["EnabledHttpCompress"], out _enabledHttpCompress);

            string enabledPermission = ConfigurationManager.AppSettings["EnabledPermission"];

            string defaultLanguageCode = ConfigurationManager.AppSettings["DefaultLanguageCode"];

            if (defaultLanguageCode != null && defaultLanguageCode.Length >= 2)
            {
                _defaultLanguageCode = defaultLanguageCode;
            }

            _extension = ConfigurationManager.AppSettings["Extension"] ?? string.Empty;

            _appKey = ConfigurationManager.AppSettings["AppKey"];
            _appSecret = ConfigurationManager.AppSettings["AppSecret"];
            _oAuthUri = ConfigurationManager.AppSettings["OAuthUri"];
            _publicKey = ConfigurationManager.AppSettings["PublicKey"];
            _privateKey = ConfigurationManager.AppSettings["PrivateKey"];

            //if (string.IsNullOrEmpty(_appKey))
            //    throw new CException("AppKey not found, please check the Web.config's configuration/appSettings.");

            //if (string.IsNullOrEmpty(_appSecret))
            //    throw new CException("AppSecret not found, please check the Web.config's configuration/appSettings.");

            //if (string.IsNullOrEmpty(_oAuthUri))
            //    throw new CException("OAuthUri not found, please check the Web.config's configuration/appSettings.");

            //if (string.IsNullOrEmpty(_publicKey))
            //    throw new CException("PublicKey not found, please check the Web.config's configuration/appSettings.");

            //if (string.IsNullOrEmpty(_privateKey))
            //    throw new CException("PrivateKey not found, please check the Web.config's configuration/appSettings.");

            if (!string.IsNullOrEmpty(enabledPermission))
                bool.TryParse(enabledPermission, out _enabledPermission);

            if (string.IsNullOrEmpty(appClassName))
                throw new CException("WebAppClassName not found, please check the Web.config's configuration/appSettings.");

            Type appType = Type.GetType(appClassName, false);

            if (appType == null)
                throw new CException("Can not load the type of WebAppClassName '" + appClassName + "'");

            MethodInfo initMethod = appType.GetMethod("Init", new Type[0]);

            if (initMethod == null || !initMethod.IsStatic)
                throw new CException("Can not found the static Init method of " + appType.FullName);

            RegisterAssembly(appType.Assembly);

            initMethod.Invoke(null, null);

            if (_sessionType == null)
                throw new CException("WebAppConfig.SessionType is null.");

            if (_securityType == null)
                throw new CException("WebAppConfig.SecurityType is null.");

            LoadControllerClasses();

            if (_timeProvider == null)
            {
                _timeProvider = new RealTimeProvider();
            }

            if (_deserializeProvider == null)
            {
                _deserializeProvider = new JSONDeserializer();
            }

            if (_serializeProvider == null)
            {
                _serializeProvider = new JSONSerializer();
            }

            if (_securityProvider == null)
            {
                _securityProvider = MiniSecurity.CreateInstance();
            }

            if (_stringFilterProvider == null)
            {
                _stringFilterProvider = new MFilter();
            }
        }

        internal static bool EnabledHttpCompress
        {
            get { return _enabledHttpCompress; }
        }

        public static string DefaultLanguageCode
        {
            get { return _defaultLanguageCode; }
        }

        public static string Extension
        {
            get { return _extension; }
        }

        public static string AppKey
        {
            get { return _appKey; }
        }

        public static string AppSecret
        {
            get { return _appSecret; }
        }

        public static string OAuthUri
        {
            get { return _oAuthUri; }
        }

        public static string PublicKey
        {
            get { return _publicKey; }
        }

        public static string PrivateKey
        {
            get { return _privateKey; }
        }

        public static string DefaultLayout
        {
            get { return _defaultLayout; }
        }

        public static string Version
        {
            get { return _version; }
        }

        public static bool EnabledPermission
        {
            get { return _enabledPermission; }
        }

        public static ITimeProvider TimeProvider
        {
            get { return _timeProvider; }
        }

        public static ILoggingProvider LoggingProvider
        {
            get { return _loggingProvider; }
            set
            {
                lock (_locked)
                {
                    if (_loggingProvider == null)
                        _loggingProvider = value;
                }
            }
        }

        public static ISecurityProvider SecurityProvider
        {
            get { return _securityProvider; }
            set
            {
                lock (_locked)
                {
                    if (_securityProvider == null)
                        _securityProvider = value;
                }
            }
        }

        public static ISessionDataProvider SessionDataProvider
        {
            get { return _sessionDataProvider; }
            set { _sessionDataProvider = value; }
        }

        public static IDeserializerProvider DeserializeProvider
        {
            get { return _deserializeProvider; }
        }

        public static ISerializerProvider SerializeProvider
        {
            get { return _serializeProvider; }
        }

        public static IVisitorProvider VisitorProvider
        {
            get { return _visitorProvider; }
            set { _visitorProvider = value; }
        }

        public static void Init()
        {
            
        }

        public static Type SessionType
        {
            get { return _sessionType; }
            set
            {
                if (!typeof(SessionBase).IsAssignableFrom(value))
                    throw new CException("Session type should be derived from SessionBase.");

                _sessionType = value;
            }
        }

        public static Type SecurityType
        {
            get { return _securityType; }
            set
            {
                if (!typeof(Controller).IsAssignableFrom(value))
                    throw new CException("Security type should be derived from Controller.");

                _securityType = value;
            }
        }

        public static ISessionLoggingProvider SessionLoggingProvider
        {
            get { return _sessionLoggingProvider; }
            set { _sessionLoggingProvider = value; }
        }

        public static ISessionSerializer SessionSerializer
        {
            get { return _sessionSerializer; }
            set { _sessionSerializer = value; }
        }

        public static IStringFilter StringFilterProvider
        {
            get { return _stringFilterProvider; }
            set
            {
                lock (_locked)
                {
                    if (_stringFilterProvider == null)
                        _stringFilterProvider = value;
                }
            }
        }

        internal static int TemplateCacheDuration
        {
            get { return _templateCacheDuration; }
        }

        internal static Encoding TemplateFileEncoding
        {
            get { return _templateFileEncoding; }
        }

        public static string ThemePath
        {
            get
            {
                string theme = WebAppContext.Session.Theme ?? _defaultTheme;

                if (WebAppContext.Request.ApplicationPath != "/")
                {
                    return string.Concat(WebAppContext.Request.ApplicationPath, "/Themes/", theme);
                }
                else
                {
                    return string.Concat("/Themes/", theme);
                }
            }
        }

        internal static bool FireSessionCreated(SessionBase session)
        {
            if (SessionCreated != null)
            {
                SessionCreated(session);
                return true;
            }
            return false;
        }

        public static void RegisterAssembly(Assembly assembly)
        {
            if (_registeredAssemblies.Find(delegate(Assembly a) { return a.FullName == assembly.FullName; }) == null)
            {
                _registeredAssemblies.Add(assembly);
            }
        }

        public static void RegisterAssembly(string assemblyPath)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            RegisterAssembly(assembly);
        }

        public static string[] GetSecurityMethods()
        {
            List<string> list = new List<string>();

            foreach (ControllerClass c in _controllerClasses.Values)
            {
                if (SecurityType.IsAssignableFrom(c.ClassType))
                {
                    string[] methods = c.GetMethods();

                    foreach (string m in methods)
                    {
                        list.Add(string.Concat(c.Name, "$", m));
                    }
                }
            }

            list.Sort();

            return list.ToArray();
        }

        internal static ControllerClass GetControllerClass(string url)
        {
            ControllerClass controllerClass = null;

            _controllerClasses.TryGetValue(url, out controllerClass);

            return controllerClass;
        }

        private static void LoadControllerClasses()
        {
            _controllerClasses = new Dictionary<string, ControllerClass>(StringComparer.Ordinal);

            List<Type> controllerTypes = new List<Type>();

            foreach (Assembly assembly in _registeredAssemblies)
            {
                controllerTypes.AddRange(Util.FindCompatibleTypes(assembly, typeof(Controller)));
            }

            foreach (Type type in controllerTypes)
            {
                string baseAssemblyNamespace = type.Assembly.FullName.Split(',')[0] + ".PageControllers";

                if (type.FullName.IndexOf(baseAssemblyNamespace) != -1)
                {
                    string url = type.FullName.Substring(baseAssemblyNamespace.Length + 1).Replace('.', '/');

                    ControllerClass controllerClass = new ControllerClass(type, url.ToLower());

                    _controllerClasses.Add(controllerClass.Name, controllerClass);
                }
            }
        }
    }
}
