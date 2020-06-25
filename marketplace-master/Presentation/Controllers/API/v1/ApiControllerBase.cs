using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Web.Http;

namespace Presentation.Controllers.API.v1
{
    public class ApiControllerBase : ApiController
    {
        public PagingHelper PagedUrl(int start, int limit, int totalCount = int.MaxValue)
        {
            return new PagingHelper(Url, start, limit, totalCount);
        }

        public class PagingHelper
        {
            private readonly System.Web.Http.Routing.UrlHelper urlHelper;
            private readonly int start;
            private readonly int limit;
            private readonly int totalCount;

            public PagingHelper(System.Web.Http.Routing.UrlHelper urlHelper, int start, int limit, int totalCount)
            {
                this.urlHelper = urlHelper;
                this.start = Math.Max(start, 0);
                this.limit = Math.Max(limit, 1);
                this.totalCount = totalCount;
            }

            public string LinkToPreviousPage(string routeName, object routeValues)
            {
                var start = this.start - limit;
                if (start < 0) return null;
                return Link(routeName, routeValues, new { start, limit });
            }

            public string LinkToNextPage(string routeName, object routeValues)
            {
                var start = this.start + limit;
                if (start > totalCount) return null;
                return Link(routeName, routeValues, new { start, limit });
            }

            private string Link(string routeName, object routeValues, object pagingValues)
            {
                routeValues = routeValues?.ToDictionary() == null || routeValues?.ToDictionary().Count == 0 ? pagingValues : CombineDynamics(pagingValues, routeValues);
                return urlHelper.Link(routeName, routeValues);
            }

            private dynamic CombineDynamics(object object1, object object2)
            {
                IDictionary<string, object> dictionary1 = GetKeyValueMap(object1);
                IDictionary<string, object> dictionary2 = GetKeyValueMap(object2);

                var result = new ExpandoObject();
                var d = result as IDictionary<string, object>;

                foreach (var pair in dictionary1.Concat(dictionary2))
                    d[pair.Key] = pair.Value;

                return result;
            }

            private IDictionary<string, object> GetKeyValueMap(object values)
            {
                if (values == null)
                    return new Dictionary<string, object>();

                var map = values as IDictionary<string, object>;
                if (map == null)
                    return map;

                map = new Dictionary<string, object>();

                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
                    map.Add(descriptor.Name, descriptor.GetValue(values));

                return map;
            }
        }
    }
}