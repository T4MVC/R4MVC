using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Tools.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace R4Mvc.Tools.CodeGen
{
    public class MethodBuilder
    {
        protected BaseMethodDeclarationSyntax _method;
        private SyntaxKind[] _modifiers = null;
        private IList<ParameterSyntax> _parameters = new List<ParameterSyntax>();
        private IList<ExpressionStatementSyntax> _statements = new List<ExpressionStatementSyntax>();
        private bool _useGeneratedAttributes = false;

        protected MethodBuilder() { }
        public MethodBuilder(string name, string returnType = null)
        {
            TypeSyntax returnTypeValue = returnType != null
                ? (TypeSyntax)IdentifierName(returnType)
                : PredefinedType(Token(SyntaxKind.VoidKeyword));
            _method = MethodDeclaration(returnTypeValue, Identifier(name));
        }

        public MethodBuilder WithModifiers(params SyntaxKind[] modifiers)
        {
            _modifiers = modifiers;
            return this;
        }

        public MethodBuilder WithGeneratedNonUserCodeAttributes()
        {
            _useGeneratedAttributes = true;
            return this;
        }

        public MethodBuilder WithStringParameter(string name, bool defaultsToNull = false)
        {
            var parameter = Parameter(Identifier(name)).WithType(SyntaxNodeHelpers.PredefinedStringType());
            if (defaultsToNull)
                parameter = parameter.WithDefault(EqualsValueClause(LiteralExpression(SyntaxKind.NullLiteralExpression)));
            _parameters.Add(parameter);
            return this;
        }

        public MethodBuilder WithParameter(string name, string type)
        {
            var parameter = Parameter(Identifier(name)).WithType(ParseTypeName(type));
            _parameters.Add(parameter);
            return this;
        }

        public MethodBuilder WithStatement(ExpressionStatementSyntax statement)
        {
            _statements.Add(statement);
            return this;
        }

        public virtual MemberDeclarationSyntax Build()
        {
            switch (_method)
            {
                case MethodDeclarationSyntax method:
                    if (_modifiers != null)
                        method = method.WithModifiers(_modifiers);
                    if (_parameters.Count > 0)
                        method = method.AddParameterListParameters(_parameters.ToArray());
                    if (_useGeneratedAttributes)
                        method = method.AddAttributeLists(SyntaxNodeHelpers.GeneratedNonUserCodeAttributeList());

                    method = method.WithBody(Block(_statements.ToArray()));
                    return method;

                case ConstructorDeclarationSyntax constructor:
                    if (_modifiers != null)
                        constructor = constructor.WithModifiers(_modifiers);
                    if (_parameters.Count > 0)
                        constructor = constructor.AddParameterListParameters(_parameters.ToArray());
                    if (_useGeneratedAttributes)
                        constructor = constructor.AddAttributeLists(SyntaxNodeHelpers.GeneratedNonUserCodeAttributeList());

                    constructor = constructor.WithBody(Block(_statements.ToArray()));
                    return constructor;

                default:
                    throw new NotSupportedException();
            }
        }

        public class ParameterSource
        {
            private ParameterSource() { }
            private static ParameterSource _instance;
            public static ParameterSource Instance => _instance ?? (_instance = new ParameterSource());

            public ExpressionSyntax Null => LiteralExpression(SyntaxKind.NullLiteralExpression);
            public ExpressionSyntax True => LiteralExpression(SyntaxKind.TrueLiteralExpression);
            public ExpressionSyntax False => LiteralExpression(SyntaxKind.FalseLiteralExpression);
            public ExpressionSyntax EmptyString => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(""));
            public ExpressionSyntax Space => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(" "));
        }
    }
}
