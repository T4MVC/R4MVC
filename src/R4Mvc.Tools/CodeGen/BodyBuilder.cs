using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Tools.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace R4Mvc.Tools.CodeGen
{
    public class BodyBuilder
    {
        private IList<StatementSyntax> _expressions = new List<StatementSyntax>();

        private ExpressionSyntax[] GetArguments(ICollection<object> arguments)
        {
            return arguments.Select(a =>
            {
                switch (a)
                {
                    case string argumentName:
                        return IdentifierName(argumentName);
                    case ExpressionSyntax argumentExpression:
                        return argumentExpression;
                    default:
                        throw new InvalidOperationException("Argument of wrong type passed. Has to be String or ExpressionSyntax");
                }
            })
            .ToArray();
        }

        private ExpressionSyntax MethodCallExpression(string entityName, string methodName, ICollection<object> arguments)
        {
            var methodCallExpression = entityName != null
                ? InvocationExpression(SyntaxNodeHelpers.MemberAccess(entityName, methodName))
                : InvocationExpression(IdentifierName(methodName));
            if (arguments?.Count > 0)
                methodCallExpression = methodCallExpression.WithArgumentList(GetArguments(arguments));
            return methodCallExpression;
        }

        private ExpressionSyntax NewObjectExpression(string entityType, ICollection<object> arguments)
        {
            var newExpression = ObjectCreationExpression(IdentifierName(entityType));
            if (arguments?.Count > 0)
                newExpression = newExpression.WithArgumentList(GetArguments(arguments));
            return newExpression;
        }

        public BodyBuilder MethodCall(string entityName, string methodName, params object[] arguments)
        {
            var methodCallExpression = MethodCallExpression(entityName, methodName, arguments);
            _expressions.Add(ExpressionStatement(methodCallExpression));
            return this;
        }

        public BodyBuilder ReturnMethodCall(string entityName, string methodName, params object[] arguments)
        {
            var methodCallExpression = MethodCallExpression(entityName, methodName, arguments);
            _expressions.Add(ReturnStatement(methodCallExpression));
            return this;
        }

        public BodyBuilder ReturnNewObject(string entityType, params object[] arguments)
        {
            var newExpression = NewObjectExpression(entityType, arguments);
            _expressions.Add(ReturnStatement(newExpression));
            return this;
        }

        public BodyBuilder ReturnVariable(string variableName)
        {
            _expressions.Add(ReturnStatement(IdentifierName(variableName)));
            return this;
        }

        public BodyBuilder VariableFromMethodCall(string variableName, string entityName, string methodName, params string[] arguments)
        {
            var methodCallExpression = MethodCallExpression(entityName, methodName, arguments);
            var variableExpression = SyntaxNodeHelpers.VariableDeclaration(variableName, methodCallExpression);
            _expressions.Add(LocalDeclarationStatement(variableExpression));
            return this;
        }

        public BodyBuilder VariableFromNewObject(string variableName, string entityType, params string[] arguments)
        {
            var newExpression = NewObjectExpression(entityType, arguments);
            var variableExpression = SyntaxNodeHelpers.VariableDeclaration(variableName, newExpression);
            _expressions.Add(LocalDeclarationStatement(variableExpression));
            return this;
        }

        public BodyBuilder ForMany<TEntity>(IEnumerable<TEntity> items, Action<BodyBuilder, TEntity> action)
        {
            if (items != null)
                foreach (var item in items)
                    action(this, item);
            return this;
        }

        public BlockSyntax Build()
        {
            return Block(_expressions.ToArray());
        }
    }
}
