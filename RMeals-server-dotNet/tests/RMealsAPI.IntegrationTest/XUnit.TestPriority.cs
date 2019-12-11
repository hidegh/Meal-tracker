using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

/*
 * THIS:
 * http://hamidmosalla.com/2018/08/16/xunit-control-the-test-execution-order/   
 * 
 * OUT OF THE BOX:
 * https://stackoverflow.com/questions/52146564/how-to-order-xunit-tests-belonging-to-one-test-collection-but-spread-across-mult
 * 
 *  [Trait("Order", "")]    
 *  [TestCaseOrderer(DependencyOrderer.TypeName, DependencyOrderer.AssemblyName)]
 *  public partial class OrderTests 
 *  {
 *      [Fact]
 *      [TestDependency("Test1", "Test0")]
 *      public void Test3()
 *      {
 *          Thread.Sleep(TimeSpan.FromSeconds(1));
 *      }
 *  }
 */
namespace RMealsAPI.IntegrationTest
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestPriorityAttribute : Attribute
    {
        public TestPriorityAttribute(int priority)
        {
            Priority = priority;
        }

        public int Priority { get; private set; }
    }

    public class PriorityOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            var sortedMethods = new SortedDictionary<int, List<TTestCase>>();

            foreach (TTestCase testCase in testCases)
            {
                int priority = 0;

                foreach (IAttributeInfo attr in testCase.TestMethod.Method.GetCustomAttributes((typeof(TestPriorityAttribute).AssemblyQualifiedName)))
                    priority = attr.GetNamedArgument<int>("Priority");

                GetOrCreate(sortedMethods, priority).Add(testCase);
            }

            foreach (var list in sortedMethods.Keys.Select(priority => sortedMethods[priority]))
            {
                list.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
                foreach (TTestCase testCase in list) yield return testCase;

            }
        }

        static TValue GetOrCreate<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
            where TValue : new()
        {
            TValue result;

            if (dictionary.TryGetValue(key, out result)) return result;

            result = new TValue();
            dictionary[key] = result;

            return result;
        }
    }
}
