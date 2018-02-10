using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace R4Mvc.Tools.CodeGen
{
    public class ConstructorMethodBuilder : MethodBuilder
    {
        public ConstructorMethodBuilder(string className)
        {
            _method = ConstructorDeclaration(className);
        }

        private IList<ArgumentSyntax> _baseConstructorArguments = null;

        public ConstructorMethodBuilder WithBaseConstructorCall(params Func<ParameterSource, ExpressionSyntax>[] arguments)
        {
            _baseConstructorArguments = arguments
                .Select(a => Argument(a(ParameterSource.Instance)))
                .ToList();
            return this;
        }

        public override MemberDeclarationSyntax Build()
        {
            var constructor = _method as ConstructorDeclarationSyntax;
            if (_baseConstructorArguments != null)
            {
                var arguments = _baseConstructorArguments.Count > 0
                    ? ArgumentList(SeparatedList(_baseConstructorArguments))
                    : null;
                constructor = constructor.WithInitializer(ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, arguments));
            }

            _method = constructor;
            return base.Build();
        }
    }
}
