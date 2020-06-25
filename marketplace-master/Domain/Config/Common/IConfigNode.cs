using Framework;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Domain.Config.Common
{
    public interface IConfigNode
    {
        void Sanitize();
        ErrorDictionary CheckErrors();
        ErrorDictionary CheckWarnings();
    }

    internal static class ConfigNodeHelper
    {
        public static void SanitizeSelfAndChildren(IConfigNode self)
        {
            StringHelper.Sanitize(self);
            SanitizeChildren(self);
        }

        public static ErrorDictionary CheckChildErrors(this IConfigNode self)
        {
            var errors = new ErrorDictionary();

            foreach (var child in GetChildConfigNodes(self))
            {
                var nodeErrors = child.CheckErrors();
                errors.Merge(nodeErrors);
            }

            return errors;
        }

        public static ErrorDictionary CheckChildWarnings(this IConfigNode self)
        {
            var warnings = new ErrorDictionary();

            foreach (var node in GetChildConfigNodes(self))
            {
                var nodeWarnings = node.CheckWarnings();
                warnings.Merge(nodeWarnings);
            }

            return warnings;
        }

        public static void SanitizeChildren(IConfigNode self)
        {
            foreach (var child in GetChildConfigNodes(self)) child.Sanitize();
        }

        public static IEnumerable<IConfigNode> GetChildConfigNodes(object node)
        {
            var properties = node.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var p in properties)
            {
                if (!typeof(IConfigNode).IsAssignableFrom(p.PropertyType)) continue;
                if (!p.CanWrite || !p.CanRead) continue;
                if (p.GetGetMethod(false) == null) continue;

                var childNode = p.GetValue(node, null) as IConfigNode;
                if (childNode == null) continue;

                yield return childNode;
            }
        }

        public static bool IsValidUrl(string url)
        {
            if (url == null) return false;

            try
            {
                var uri = new Uri(url, UriKind.Absolute);
                return true;
            }
            catch { return false; }
        }

        public static string SanitizeUrl(string url)
        {
            if (url == null) return url;
            if (!IsValidUrl(url)) return url;
            if (!url.EndsWith("/")) url = url + "/";
            return url;
        }
    }
}
