using System;
using System.IO;
using System.Reflection;

namespace WindowsDesktop.Properties
{
    public class ProductInfo
    {
        private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        private static readonly Lazy<string> _titleLazy = new(() => ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(_assembly, typeof(AssemblyTitleAttribute))).Title);
        private static readonly Lazy<string> _descriptionLazy = new(() => ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(_assembly, typeof(AssemblyDescriptionAttribute))).Description);
        private static readonly Lazy<string> _companyLazy = new(() => ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(_assembly, typeof(AssemblyCompanyAttribute))).Company);
        private static readonly Lazy<string> _productLazy = new(() => ((AssemblyProductAttribute)Attribute.GetCustomAttribute(_assembly, typeof(AssemblyProductAttribute))).Product);
        private static readonly Lazy<string> _copyrightLazy = new(() => ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(_assembly, typeof(AssemblyCopyrightAttribute))).Copyright);
        private static readonly Lazy<string> _trademarkLazy = new(() => ((AssemblyTrademarkAttribute)Attribute.GetCustomAttribute(_assembly, typeof(AssemblyTrademarkAttribute))).Trademark);
        private static readonly Lazy<string> _versionLazy = new(() => $"{Version.ToString(3)}");
        private static readonly Lazy<DirectoryInfo> _localAppDataLazy = new(() => new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Company, Product)));

        public static string Title => _titleLazy.Value;

        public static string Description => _descriptionLazy.Value;

        public static string Company => _companyLazy.Value;

        public static string Product => _productLazy.Value;

        public static string Copyright => _copyrightLazy.Value;

        public static string Trademark => _trademarkLazy.Value;

        public static Version Version => _assembly.GetName().Version;

        public static string VersionString => _versionLazy.Value;

        internal static DirectoryInfo LocalAppData => _localAppDataLazy.Value;

        // Once windows 11 comes out this will probably report the latest build that is Windows 10 (or Windows 11 Preview)
        // until the app.manifest has the Windows 11 sSupportedOS ID
        internal static int OSBuild => Environment.OSVersion.Version.Build;
    }
}
