using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Tools.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace R4Mvc.Tools.CodeGen
{
    public class ClassBuilder
    {
        private readonly string _className;
        private ClassDeclarationSyntax _class;

        public ClassBuilder(string className)
        {
            _className = className;
            _class = ClassDeclaration(className);
        }

        public ClassBuilder WithModifiers(params SyntaxKind[] modifiers)
        {
            _class = _class.WithModifiers(modifiers);
            return this;
        }

        public ClassBuilder WithBaseTypes(string[] classNames)
        {
            _class = _class.WithBaseTypes(classNames);
            return this;
        }

        public ClassBuilder WithMethod(MethodBuilder method)
        {
            _class = _class.AddMembers(method.Build());
            return this;
        }

        public ClassBuilder WithConstructor(MethodBuilder method) => WithMethod(method);

        internal ClassDeclarationSyntax Build()
        {
            return _class;
        }
    }
}
