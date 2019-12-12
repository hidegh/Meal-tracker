using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;

namespace RMealsAPI.Code.Filters
{
    public class ODataQueryableAttribute : EnableQueryAttribute
    {
        /*
        public bool TrackingEnabled { get; set; }
        */

        public override IQueryable ApplyQuery(IQueryable queryable, ODataQueryOptions queryOptions)
        {
            /*
            if (!TrackingEnabled)
                queryable = queryable.AsNoTracking();
            */

            return base.ApplyQuery(queryable, queryOptions);
        }

        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);

            //
            // Post-process - errors

            // skip exceptions...
            if (actionExecutedContext.Exception != null)
                return;

            // assume object result
            var responseContent = actionExecutedContext.Result as ObjectResult;

            // check for some OData error (and make sure readable error message is generated)
            dynamic value = responseContent?.Value;

            if (value is SerializableError)
            {
                var se = value as SerializableError;
                var seKeys = se.Keys.ToArray();

                var sb = new StringBuilder();
                sb.AppendLine($"Count = {se.Count}");
                for (int i = 0; i < se.Keys.Count; i++)
                    sb.AppendLine($"\t[{i}]: {{{seKeys[i]}, {se[seKeys[i]]}}}");

                value = sb.ToString();

                throw new NotSupportedException($"Controller action result with OData queries encountered an error. We got StatusCode: {responseContent?.StatusCode}, Type: {responseContent?.Value?.GetType()}, Value: {value}");
            }

            // check for generic errors...
            var response = actionExecutedContext.HttpContext.Response;
            if (!response.IsSuccessStatusCode())
                return;

            //
            // Post-process - odata result

            // get extra props
            var nextLink = actionExecutedContext.HttpContext.Request.ODataFeature().NextLink; // NOTE: ev: Request.GetNextPageLink(odataOptions.Top?.Value ?? 10)
            var count = actionExecutedContext.HttpContext.Request.ODataFeature().TotalCount;

            // return wrapped content
            var result = new ODataPagedResult<dynamic>(value, nextLink, count);
            responseContent.Value = result;

        }
    }

    /// <summary>
    /// NOTE: theres a PageResult&lt;T&gt; class already in .NET but it does not serialize proper V4 OData JSON result...
    /// </summary>
    public class ODataPagedResult<T>
    {
        public ODataPagedResult(T items, Uri nextLink, long? count)
        {
            Value = items;
            NextLink = nextLink?.ToString() ?? "";
            Count = count;
        }

        [JsonProperty("value")]
        public T Value { get; set; }
        [JsonProperty("@odata.count")]
        public long? Count { get; set; }
        [JsonProperty("@odata.nextLink")]
        public string NextLink { get; set; }
    }

}
