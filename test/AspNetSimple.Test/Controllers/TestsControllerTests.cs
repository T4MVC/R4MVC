using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspNetSimple.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Xunit;

namespace AspNetSimple.Test.Controllers
{
    public class TestsControllerTests
    {
        private static IEnumerable<MethodInfo> GetControllerMethods()
        {
            var controllers = typeof(TestsController).Assembly.GetTypes()
                .Where(t => t.IsPublic)
                .Where(t => !t.IsAbstract)
                .Where(t => typeof(Controller).IsAssignableFrom(t))
                .Where(t => t.GetCustomAttribute<GeneratedCodeAttribute>() == null)
                .Where(t => t.GetCustomAttribute<R4MvcExcludeAttribute>() == null);

            var controllerMethods = typeof(Controller).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var methods = controllers.SelectMany(c => c.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                .Where(cm => cm.GetCustomAttribute<NonActionAttribute>() == null)
                .Where(cm => cm.GetCustomAttribute<R4MvcExcludeAttribute>() == null);

            return methods
                .Where(m => !controllerMethods.Any(cm => cm.Name == m.Name));
        }

        public static IEnumerable<object[]> HasDefaultMethodsCreatedData
            => GetControllerMethods()
                .Where(m =>
                {
                    var returnType = m.ReturnType;
                    if (typeof(Task).IsAssignableFrom(returnType) && returnType.IsGenericType)
                        returnType = returnType.GetGenericArguments()[0];

                    return
                        typeof(IActionResult).IsAssignableFrom(returnType) ||
                        typeof(IConvertToActionResult).IsAssignableFrom(returnType);
                })
                .GroupBy(m => new { m.DeclaringType, m.Name })
                .Select(m => new object[] { m.Key.DeclaringType.Name, m.Key.Name, m.Key.DeclaringType });

        [Theory]
        [MemberData(nameof(HasDefaultMethodsCreatedData))]
        public void HasDefaultMethodsCreated(string className, string methodName, Type controllerClass)
        {
            var methods = controllerClass
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name == methodName)
                .Where(m => m.GetParameters().Length == 0);

            Assert.Single(methods);
        }
    }
}
