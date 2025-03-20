using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.AspNetCore.SignalR.Client;
using CodeBro.Client.Code_Recognision;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Text;
using System.Linq;
using System.Windows.Input;

namespace CodeBro.Client
{
    public partial class MainWindow : Window
    {
        private HubConnection _connection;
        private bool isDarkMode;
        private Lexer lexer;
        private bool isUpdatingText = false;

        public MainWindow()
        {
            InitializeComponent();
            ConnectToServer();
            LoadTheme();
            lexer = new Lexer();

            CodeEditor.Document.Blocks.Clear();
            CodeEditor.Document.Blocks.Add(new Paragraph());
        }

        private async void ConnectToServer()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/codehub")
                .Build();

            _connection.On<string>("ReceiveCodeChange", (code) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (!isUpdatingText)
                    {
                        isUpdatingText = true;
                        SetTextSafely(code);
                        isUpdatingText = false;
                    }
                });
            });

            try
            {
                await _connection.StartAsync();
                await _connection.InvokeAsync("JoinSession", "default_session");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nu s-a putut conecta la server: {ex.Message}");
            }
        }

        private void CodeEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdatingText) return;

            isUpdatingText = true;

            string code = GetPlainText();

            if (_connection != null && _connection.State == HubConnectionState.Connected)
            {
                _connection.InvokeAsync("SendCodeChange", "default_session", code);
            }

            ApplySyntaxHighlighting();

            isUpdatingText = false;
        }

        private void LoadTheme()
        {
            isDarkMode = Properties.Settings.Default.isDarkMode;
            ApplyTheme();
        }

        private void SwitchTheme_Click(object sender, RoutedEventArgs e)
        {
            isDarkMode = !isDarkMode;
            Properties.Settings.Default.isDarkMode = isDarkMode;
            Properties.Settings.Default.Save();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            if (isDarkMode)
            {
                Background = (Brush)new BrushConverter().ConvertFrom("#1E1E1E");
                CodeEditor.Background = (Brush)new BrushConverter().ConvertFrom("#1E1E1E");
                CodeEditor.Foreground = (Brush)new BrushConverter().ConvertFrom("#D4D4D4");
            }
            else
            {
                Background = Brushes.White;
                CodeEditor.Background = Brushes.White;
                CodeEditor.Foreground = Brushes.Black;
            }

            ApplySyntaxHighlighting();
        }

        private string GetPlainText()
        {
            StringBuilder text = new StringBuilder();

            foreach (Block block in CodeEditor.Document.Blocks)
            {
                if (block is Paragraph paragraph)
                {
                    TextRange range = new TextRange(paragraph.ContentStart, paragraph.ContentEnd);
                    text.Append(range.Text);
                }
            }

            return text.ToString();
        }

        private void SetTextSafely(string text)
        {
            try
            {
                TextPointer caretPosition = CodeEditor.CaretPosition;
                bool isAtEnd = caretPosition.CompareTo(CodeEditor.Document.ContentEnd) == 0;

                FlowDocument doc = new FlowDocument();
                doc.Blocks.Add(new Paragraph(new Run(text)));
                CodeEditor.Document = doc;

                if (isAtEnd)
                {
                    CodeEditor.CaretPosition = CodeEditor.Document.ContentEnd;
                }

                ApplySyntaxHighlighting();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la setarea textului: {ex.Message}");
            }
        }

        private void ApplySyntaxHighlighting()
        {
            try
            {
                CodeEditor.TextChanged -= CodeEditor_TextChanged;

                TextPointer caretPosition = CodeEditor.CaretPosition;

                string originalText = GetPlainText();

                FlowDocument newDocument = new FlowDocument();

                string[] lines = originalText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                foreach (string line in lines)
                {
                    Paragraph para = new Paragraph();
                    para.Margin = new Thickness(0);

                    if (string.IsNullOrEmpty(line))
                    {
                        para.Inlines.Add(new Run(string.Empty));
                    }
                    else
                    {
                        List<Token> tokens = lexer.Tokenize(line);

                        foreach (Token token in tokens)
                        {
                            Run run = new Run(token.Value);

                            Brush tokenColor = GetTokenColor(token.Type);
                            run.Foreground = tokenColor;

                            para.Inlines.Add(run);
                        }
                    }

                    newDocument.Blocks.Add(para);
                }

                CodeEditor.Document = newDocument;

                try
                {
                    if (caretPosition != null)
                    {
                        // Încercăm să estimăm noua poziție
                        TextPointer newPosition = CodeEditor.Document.ContentStart;
                        int offset = Math.Min(caretPosition.GetOffsetToPosition(caretPosition.DocumentEnd),
                                            newDocument.ContentStart.GetOffsetToPosition(newDocument.ContentEnd));

                        if (offset > 0)
                        {
                            TextPointer documentStart = CodeEditor.Document.ContentStart;
                            TextPointer newCaretPosition = documentStart.GetPositionAtOffset(offset);

                            if (newCaretPosition != null)
                                CodeEditor.CaretPosition = newCaretPosition;
                            else
                                CodeEditor.CaretPosition = CodeEditor.Document.ContentEnd;
                        }
                        else
                        {
                            CodeEditor.CaretPosition = CodeEditor.Document.ContentStart;
                        }
                    }
                }
                catch
                {
                    CodeEditor.CaretPosition = CodeEditor.Document.ContentEnd;
                }

                CodeEditor.TextChanged += CodeEditor_TextChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la evidențierea sintaxei: {ex.Message}");
            }
        }

        private Brush GetTokenColor(TokenType tokenType)
        {
            if (isDarkMode)
            {
                switch (tokenType)
                {
                    case TokenType.Keyword:
                        return (Brush)new BrushConverter().ConvertFrom("#569CD6"); // Albastru
                    case TokenType.Identifier:
                        return (Brush)new BrushConverter().ConvertFrom("#9CDCFE"); // Albastru deschis
                    case TokenType.Number:
                        return (Brush)new BrushConverter().ConvertFrom("#B5CEA8"); // Verde deschis
                    case TokenType.String:
                        return (Brush)new BrushConverter().ConvertFrom("#CE9178"); // Portocaliu
                    case TokenType.Operator:
                        return (Brush)new BrushConverter().ConvertFrom("#D4D4D4"); // Alb
                    case TokenType.Separator:
                        return (Brush)new BrushConverter().ConvertFrom("#D4D4D4"); // Alb
                    case TokenType.Comment:
                        return (Brush)new BrushConverter().ConvertFrom("#6A9955"); // Verde
                    default:
                        return (Brush)new BrushConverter().ConvertFrom("#D4D4D4"); // Alb
                }
            }
            else
            {
                switch (tokenType)
                {
                    case TokenType.Keyword:
                        return (Brush)new BrushConverter().ConvertFrom("#0000FF"); // Albastru
                    case TokenType.Identifier:
                        return (Brush)new BrushConverter().ConvertFrom("#1F377F"); // Albastru inchis
                    case TokenType.Number:
                        return (Brush)new BrushConverter().ConvertFrom("#098658"); // Verde
                    case TokenType.String:
                        return (Brush)new BrushConverter().ConvertFrom("#A31515"); // Rosu
                    case TokenType.Operator:
                        return Brushes.Black;
                    case TokenType.Separator:
                        return Brushes.Black;
                    case TokenType.Comment:
                        return (Brush)new BrushConverter().ConvertFrom("#008000"); // Verde
                    default:
                        return Brushes.Black;
                }
            }
        }
    }
}