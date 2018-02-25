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

        public ExpressionSyntax MethodCallExpression(string entityName, string methodName, params string[] arguments)
        {
            var methodCallExpression = entityName != null
                ? InvocationExpression(SyntaxNodeHelpers.MemberAccess(entityName, methodName))
                : InvocationExpression(IdentifierName(methodName));
            if (arguments?.Length > 0)
                methodCallExpression = methodCallExpression.WithArgumentList(arguments.Select(a => IdentifierName(a)).ToArray());
            return methodCallExpression;
        }

        public ExpressionSyntax NewExpression(string entityType, params string[] arguments)
        {
            var newExpression = ObjectCreationExpression(IdentifierName(entityType));
            if (arguments?.Length > 0)
                newExpression = newExpression.WithArgumentList(arguments.Select(a => IdentifierName(a)).ToArray());
            return newExpression;
        }

        public BodyBuilder MethodCall(string entityName, string methodName, params string[] arguments)
        {
            var methodCallExpression = MethodCallExpression(entityName, methodName, arguments);
            _expressions.Add(ExpressionStatement(methodCallExpression));
            return this;
        }

        public BodyBuilder VariableFromMethodCall(string variableName, string entityName, string methodName, params string[] arguments)
        {
            var methodCallExpression = MethodCallExpression(entityName, methodName, arguments);
            var variableExpression = SyntaxNodeHelpers.VariableDeclaration(variableName, methodCallExpression);
            _expressions.Add(LocalDeclarationStatement(variableExpression));
            return this;
        }

        public BodyBuilder ReturnMethodCall(string entityName, string methodName, params string[] arguments)
        {
            var methodCallExpression = MethodCallExpression(entityName, methodName, arguments);
            _expressions.Add(ReturnStatement(methodCallExpression));
            return this;
        }

        public BodyBuilder ReturnNew(string entityType, params string[] arguments)
        {
            var newExpression = NewExpression(entityType, arguments);
            _expressions.Add(ReturnStatement(newExpression));
            return this;
        }

        public BlockSyntax Build()
        {
            return Block(_expressions.ToArray());
        }
    }
}
