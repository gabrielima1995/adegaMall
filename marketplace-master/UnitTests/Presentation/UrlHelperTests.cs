using System.Web.Mvc;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Presentation
{
    [TestClass]
    public class UrlHelperTests
    {
        [TestMethod]
        public void AddLangWithSlashWorks()
        {
            var result = UrlHelperGlobalizationExtensions.AddLang(null, "/");
            result.Should().Be("/?lang=en-US");
        }

        [TestMethod]
        public void AddLangWithFullPathWorks()
        {
        }

        [TestMethod]
        public void AddLangWithAbsoluteUrlWorks()
        {
        }

        [TestMethod]
        public void AddLangWithNullOrEmptyConvertsToRoot()
        {
        }
    }
}
