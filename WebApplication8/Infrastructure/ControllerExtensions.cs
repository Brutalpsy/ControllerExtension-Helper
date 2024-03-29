﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

namespace WebApplication8.Infrastructure
{
    public static class ControllerExtensions
    {
        private static readonly ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();
        public static IActionResult RedirectTo<TController>(this Controller controller, Expression<Action<TController>> redurectExpression)
        {
            if (redurectExpression.Body.NodeType != ExpressionType.Call)
            {
                throw new InvalidOperationException($"the provided expression is not a valid method call: {redurectExpression.Body}");
            }

            var methodCallExpression = (MethodCallExpression)redurectExpression.Body;
            var actionName = GetActionName(methodCallExpression);
            var controllerName = typeof(TController).Name.Replace(nameof(Controller), string.Empty);
            var routeValues = ExtractRouteValues(methodCallExpression);

            return controller.RedirectToAction(actionName, controllerName);
        }

        private static string GetActionName(MethodCallExpression methodCallExpression)
        {
            var cacheKey = $"{methodCallExpression.Method.Name}_{methodCallExpression.Object.Type.Name}";

            return _cache.GetOrAdd(cacheKey, _ =>
            {
                var methodName = methodCallExpression.Method.Name;

                var actionName = methodCallExpression
                    .Method
                    .GetCustomAttributes(true)
                    .OfType<ActionNameAttribute>()
                    .FirstOrDefault()
                    ?.Name;

                return actionName ?? methodName;
            });


        }

        private static RouteValueDictionary ExtractRouteValues(MethodCallExpression expression)
        {
            var names = expression.Method.GetParameters().Select(x => x.Name).ToArray();
            var values = expression.Arguments.Select(arg =>
            {
                if (arg.NodeType == ExpressionType.Constant)
                {
                    var constantExpression = (ConstantExpression)arg;
                    return constantExpression.Value;
                }

                var expressionConvert = Expression.Convert(arg, typeof(object));
                var funcExpression = Expression.Lambda<Func<object>>(expressionConvert);

                return funcExpression.Compile()();
            }).ToArray();
            var routeValueDictionary = new RouteValueDictionary();

            for (int i = 0; i < names.Length; i++)
            {
                routeValueDictionary.Add(names[i], values[i]);
            }
            return routeValueDictionary;
        }
    }
}
