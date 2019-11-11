using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq.Expressions;

namespace WebApplication8.Infrastructure
{
    public static class ControllerExtensions
    {
        public static IActionResult RedirectTo<TController>(this Controller controller, Expression<Action<TController>> redurectExpression)
        {
            if (redurectExpression.Body.NodeType != ExpressionType.Call) 
            {
                throw new InvalidOperationException($"the provided expression is not a valid method call: {redurectExpression.Body}");
            }

            var methodCallExpression = (MethodCallExpression)redurectExpression.Body;

            var actionName = methodCallExpression.Method.Name;
            var controllerName = typeof(TController).Name.Replace(nameof(Controller),string.Empty);

            return controller.RedirectToAction(actionName, controllerName);
        }
    }
}
