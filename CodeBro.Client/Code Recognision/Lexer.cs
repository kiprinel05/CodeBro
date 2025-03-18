using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CodeBro.Client.Code_Recognision
{
    public class Lexer
    {
        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
    {
        { "int", TokenType.Keyword }, { "string", TokenType.Keyword }, { "if", TokenType.Keyword },
        { "else", TokenType.Keyword }, { "return", TokenType.Keyword }, { "void", TokenType.Keyword }
    };

        private static readonly Regex TokenRegex = new Regex(
            @"(?<Keyword>\b(int|string|if|else|return|void)\b)|" +
            @"(?<Identifier>\b[a-zA-Z_][a-zA-Z0-9_]*\b)|" +
            @"(?<Number>\b\d+\b)|" +
            @"(?<Operator>[=+\-*/<>!])|" +
            @"(?<Separator>[{}();,])|" +
            @"(?<Comment>//.*?$)", RegexOptions.Compiled | RegexOptions.Multiline
        );

        public List<Token> Tokenize(string code)
        {
            List<Token> tokens = new List<Token>();
            foreach (Match match in TokenRegex.Matches(code))
            {
                if (match.Groups["Keyword"].Success)
                    tokens.Add(new Token(TokenType.Keyword, match.Value));
                else if (match.Groups["Identifier"].Success)
                    tokens.Add(new Token(TokenType.Identifier, match.Value));
                else if (match.Groups["Number"].Success)
                    tokens.Add(new Token(TokenType.Number, match.Value));
                else if (match.Groups["String"].Success)
                    tokens.Add(new Token(TokenType.String, match.Value));
                else if (match.Groups["Operator"].Success)
                    tokens.Add(new Token(TokenType.Operator, match.Value));
                else if (match.Groups["Separator"].Success)
                    tokens.Add(new Token(TokenType.Separator, match.Value));
                else if (match.Groups["Comment"].Success)
                    tokens.Add(new Token(TokenType.Comment, match.Value));
                else
                    tokens.Add(new Token(TokenType.Error, match.Value));
            }
            return tokens;
        }
    }


}
