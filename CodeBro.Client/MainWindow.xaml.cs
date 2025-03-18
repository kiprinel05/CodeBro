using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.AspNetCore.SignalR.Client;
using CodeBro.Client.Code_Recognision;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Text;

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
                        SetRichTextBoxText(code);
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
            string code = GetRichTextBoxText();

            if (_connection != null && _connection.State == HubConnectionState.Connected)
            {
                _connection.InvokeAsync("SendCodeChange", "default_session", code);
            }

            HighlightSyntax(code);
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
        }

        private string GetRichTextBoxText()
        {
            TextRange textRange = new TextRange(CodeEditor.Document.ContentStart, CodeEditor.Document.ContentEnd);
            return textRange.Text;
        }

        private void SetRichTextBoxText(string text)
        {
            CodeEditor.Document.Blocks.Clear();
            CodeEditor.Document.Blocks.Add(new Paragraph(new Run(text)));
        }

        private void HighlightSyntax(string code)
        {
            CodeEditor.TextChanged -= CodeEditor_TextChanged;

            // Salvăm poziția cursorului
            TextPointer caretPosition = CodeEditor.CaretPosition;

            // Obținem textul curent
            TextRange textRange = new TextRange(CodeEditor.Document.ContentStart, CodeEditor.Document.ContentEnd);
            code = textRange.Text.Replace("\r\n", " ").Replace("\n", " ");

            List<Token> tokens = lexer.Tokenize(code);

            Paragraph paragraph = new Paragraph();
            StringBuilder formattedText = new StringBuilder();

            foreach (var token in tokens)
            {
                formattedText.Append(token.Value);
            }

            Run run = new Run(formattedText.ToString())
            {
                Foreground = Brushes.White
            };

            paragraph.Inlines.Add(run);

            // Înlocuim conținutul fără să stricăm poziția cursorului
            CodeEditor.Document.Blocks.Clear();
            CodeEditor.Document.Blocks.Add(paragraph);

            // Reaplicăm poziția cursorului
            CodeEditor.CaretPosition = caretPosition;
            CodeEditor.Focus();

            CodeEditor.TextChanged += CodeEditor_TextChanged;
        }


    }
}