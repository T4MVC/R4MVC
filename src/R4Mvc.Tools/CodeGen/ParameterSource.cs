using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace R4Mvc.Tools.CodeGen
{
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

        public ExpressionSyntax String(string value) => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value));
    }
}
