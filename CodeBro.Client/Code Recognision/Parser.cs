using System;
using System.Collections.Generic;

namespace CodeBro.Client.Code_Recognision
{
    public class Parser
    {
        private List<Token> tokens;
        private int position;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            this.position = 0;
        }

        public void Parse()
        {
            while (position < tokens.Count)
            {
                Token token = tokens[position];
                switch (token.Type)
                {
                    case TokenType.Keyword:
                    case TokenType.Identifier:
                    case TokenType.Number:
                        Console.WriteLine($"Token valid: {token.Value}");
                        break;
                    case TokenType.Error:
                        Console.WriteLine($"Eroare de sintaxa la token: {token.Value}");
                        break;
                }
                position++;
            }
        }
    }
}