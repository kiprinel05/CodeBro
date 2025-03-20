using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeBro.Client.Code_Recognision
{
    public class Lexer
    {
        // Lista de cuvinte cheie C#
        private static readonly HashSet<string> CSharpKeywords = new HashSet<string>
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
            "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
            "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed",
            "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw",
            "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using",
            "virtual", "void", "volatile", "while", "add", "alias", "ascending", "async", "await",
            "by", "descending", "dynamic", "equals", "from", "get", "global", "group", "into", "join",
            "let", "nameof", "on", "orderby", "partial", "remove", "select", "set", "value", "var",
            "when", "where", "yield"
        };

        public List<Token> Tokenize(string code)
        {
            List<Token> tokens = new List<Token>();
            if (string.IsNullOrEmpty(code)) return tokens;

            int position = 0;
            while (position < code.Length)
            {
                if (char.IsWhiteSpace(code[position]))
                {
                    tokens.Add(new Token(TokenType.Whitespace, code[position].ToString()));
                    position++;
                    continue;
                }

                if (position + 1 < code.Length && code[position] == '/' && code[position + 1] == '/')
                {
                    int start = position;
                    position = code.Length;
                    tokens.Add(new Token(TokenType.Comment, code.Substring(start)));
                    continue;
                }

                if (code[position] == '"')
                {
                    int start = position;
                    position++;
                    bool isEscaped = false;

                    while (position < code.Length)
                    {
                        if (code[position] == '\\' && !isEscaped)
                        {
                            isEscaped = true;
                        }
                        else if (code[position] == '"' && !isEscaped)
                        {
                            position++;
                            break;
                        }
                        else
                        {
                            isEscaped = false;
                        }
                        position++;
                    }

                    if (position <= code.Length)
                    {
                        tokens.Add(new Token(TokenType.String, code.Substring(start, position - start)));
                    }
                    continue;
                }

                if (char.IsLetter(code[position]) || code[position] == '_')
                {
                    int start = position;
                    while (position < code.Length && (char.IsLetterOrDigit(code[position]) || code[position] == '_'))
                    {
                        position++;
                    }

                    string word = code.Substring(start, position - start);
                    if (CSharpKeywords.Contains(word))
                    {
                        tokens.Add(new Token(TokenType.Keyword, word));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Identifier, word));
                    }
                    continue;
                }

                if (char.IsDigit(code[position]))
                {
                    int start = position;
                    bool hasDecimalPoint = false;

                    while (position < code.Length &&
                          (char.IsDigit(code[position]) ||
                           (code[position] == '.' && !hasDecimalPoint) ||
                           (position > start && "fFdDmM".IndexOf(code[position]) >= 0)))
                    {
                        if (code[position] == '.') hasDecimalPoint = true;
                        position++;
                    }

                    tokens.Add(new Token(TokenType.Number, code.Substring(start, position - start)));
                    continue;
                }

                if ("+-*/%=<>!&|^~?:".IndexOf(code[position]) >= 0)
                {
                    int start = position;

                    if (position + 1 < code.Length)
                    {
                        string op2 = code.Substring(position, 2);
                        if (new[] { "==", "!=", "<=", ">=", "+=", "-=", "*=", "/=", "%=", "&&", "||", "??", "?.", "::" }.Contains(op2))
                        {
                            tokens.Add(new Token(TokenType.Operator, op2));
                            position += 2;
                            continue;
                        }
                    }

                    tokens.Add(new Token(TokenType.Operator, code[position].ToString()));
                    position++;
                    continue;
                }

                if ("{}[]();,.".IndexOf(code[position]) >= 0)
                {
                    tokens.Add(new Token(TokenType.Separator, code[position].ToString()));
                    position++;
                    continue;
                }

                tokens.Add(new Token(TokenType.Error, code[position].ToString()));
                position++;
            }

            return tokens;
        }
    }
}