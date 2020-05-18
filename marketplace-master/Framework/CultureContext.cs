using System;
using System.Globalization;
using System.Threading;

namespace Framework
{
    public class CultureContext : IDisposable
    {
        public static CultureContext Invariant => new CultureContext(CultureInfo.InvariantCulture);

        public CultureInfo ContextCulture { get; }

        public CultureInfo CultureBackup { get; }

        public CultureInfo UiCultureBackup { get; }

        public CultureContext(CultureInfo culture)
        {
            ContextCulture = culture;
            CultureBackup = Thread.CurrentThread.CurrentCulture;
            UiCultureBackup = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = ContextCulture;
        }

        public CultureContext(string cultureName) : this(CultureInfo.CreateSpecificCulture(cultureName)) { }

        public void Dispose()
        {
            Thread.CurrentThread.CurrentCulture = CultureBackup;
            Thread.CurrentThread.CurrentUICulture = UiCultureBackup;
        }
    }
}
