using System;

namespace CodeBro.Client.Code_Recognision
{
    public enum TokenType
    {
        Keyword,
        Identifier,
        Number,
        String,
        Operator,
        Separator,
        Comment,
        Whitespace,
        Error
    }

    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type}: {Value}";
        }
    }
}