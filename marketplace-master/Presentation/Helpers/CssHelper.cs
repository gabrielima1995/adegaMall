using Domain.DTO;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.Helpers
{
    public static class Css
    {
        public static string If(bool condition, string className, string elseClassName)
        {
            return condition ? className : elseClassName;
        }

        public static CssClassByConditionBuilder If(bool condition, string className)
        {
            return new CssClassByConditionBuilder(condition, className);
        }

        /// <summary>
        /// Retorna uma classe CSS para desabilitar links e botões se o usuário for um SellerAssistant sem as permissões indicadas.
        /// </summary>
        public static string DisabledForSellerAssistant(LoginProfile profile, params string[] unlessHasPermissions)
        {
            if (profile == null) return null;
            var isAllowed = profile.IsNotAssistantOrHasPermissions(unlessHasPermissions);
            return If(!isAllowed, SellerAssistantPresentation.DisabledClass).Get();
        }
    }

    public class CssClassByConditionBuilder
    {
        private readonly List<CssClassByCondition> items = new List<CssClassByCondition>();

        public CssClassByConditionBuilder(bool condition, string className)
        {
            ElseIf(condition, className);
        }

        public CssClassByConditionBuilder ElseIf(bool condition, string className)
        {
            items.Add(new CssClassByCondition(condition, className));
            return this;
        }

        public CssClassByConditionBuilder Else(string className)
        {
            items.Add(new CssClassByCondition(true, className));
            return this;
        }

        public string Get()
        {
            return items.FirstOrDefault(item => item.ConditionMet)?.ClassName ?? "";
        }

        public override string ToString()
        {
            return Get();
        }

        private class CssClassByCondition
        {
            internal CssClassByCondition(bool condition, string className)
            {
                ConditionMet = condition;
                ClassName = className;
            }

            internal bool ConditionMet { get; }
            internal string ClassName { get; }
        }
    }
}