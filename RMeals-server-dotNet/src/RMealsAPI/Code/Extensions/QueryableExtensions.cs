using System.Collections;
using System.Linq;

namespace RMealsAPI.Code.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Executes the deferred IQueryable&lt;T&gt;.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IList ToListExecutor(this IQueryable query)
        {
            var elementType = query.ElementType;
            var enumerableToListMethod = typeof(Enumerable).GetMethod("ToList").MakeGenericMethod(elementType);
            var value = enumerableToListMethod.Invoke(null, new object[] { query });
            return value as IList;
        }
    }
}
