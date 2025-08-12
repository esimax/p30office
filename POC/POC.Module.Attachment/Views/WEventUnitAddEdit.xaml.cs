using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using DevExpress.CodeParser;
using DevExpress.Utils;
using DevExpress.Office.Internal;
using DevExpress.Office.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.RichEdit;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Import;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Commands;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.WPF.DXControls;


namespace POC.Module.Attachment.Views
{
    public partial class WEventUnitAddEdit : DXWindow
    {
        private IDatabase ADatabase { get; set; }
        
        private DBCTEvent Data { get; set; }
        public WEventUnitAddEdit(DBCTEvent data)
        {
            InitializeComponent();

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            APOCEventUnit = ServiceLocator.Current.GetInstance<IPOCEventUnit>();

            Data = data;
            Loaded += (s, e) =>
            {
                var events=APOCEventUnit.GetList();
                
                tePriority.EditValue = 1;
                cbeEvents.ItemsSource = events;
                cbeEvents.DisplayMember = "Title";

                if (Data != null)
                {
                    tePriority.EditValue = Data.Priority;
                    cbeEvents.SelectedItem = (from n in events where n.Key == data.Key select n).FirstOrDefault();
                    richEdit.Text = Data.Script;
                }
            };
        }


        private void richEdit_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            richEdit.Options.AutoCorrect.DetectUrls = false;
            richEdit.Options.AutoCorrect.ReplaceTextAsYouType = false;
            richEdit.Options.Behavior.PasteLineBreakSubstitution = LineBreakSubstitute.Paragraph;
            DocumentCapabilitiesOptions caps = richEdit.Options.DocumentCapabilities;
            caps.Bookmarks = DocumentCapability.Disabled;
            caps.CharacterStyle = DocumentCapability.Disabled;
            caps.HeadersFooters = DocumentCapability.Disabled;
            caps.Hyperlinks = DocumentCapability.Disabled;
            caps.InlinePictures = DocumentCapability.Disabled;
            caps.Numbering.Bulleted = DocumentCapability.Disabled;
            caps.Numbering.MultiLevel = DocumentCapability.Disabled;
            caps.Numbering.Simple = DocumentCapability.Disabled;
            caps.ParagraphFormatting = DocumentCapability.Disabled;
            caps.ParagraphStyle = DocumentCapability.Disabled;
            caps.Sections = DocumentCapability.Disabled;
            caps.Tables = DocumentCapability.Disabled;
            caps.TableStyle = DocumentCapability.Disabled;
            caps.FloatingObjects = DocumentCapability.Disabled;
            richEdit.Options.HorizontalRuler.Visibility = RichEditRulerVisibility.Hidden;
            richEdit.Views.DraftView.AllowDisplayLineNumbers = true;
            richEdit.Views.DraftView.Padding = new Padding(60, 4, 0, 0);

            richEdit.AddService(typeof (ISyntaxHighlightService), new SyntaxHighlightService(richEdit));
            IRichEditCommandFactoryService commandFactory = richEdit.GetService<IRichEditCommandFactoryService>();
            CustomRichEditCommandFactoryService newCommandFactory =
                new CustomRichEditCommandFactoryService(commandFactory);
            richEdit.RemoveService(typeof (IRichEditCommandFactoryService));
            richEdit.AddService(typeof (IRichEditCommandFactoryService), newCommandFactory);

            IDocumentImportManagerService importManager = richEdit.GetService<IDocumentImportManagerService>();
            importManager.UnregisterAllImporters();
            importManager.RegisterImporter(new PlainTextDocumentImporter());
            importManager.RegisterImporter(new SourcesCodeDocumentImporter());

            IDocumentExportManagerService exportManager = richEdit.GetService<IDocumentExportManagerService>();
            exportManager.UnregisterAllExporters();
            exportManager.RegisterExporter(new PlainTextDocumentExporter());
            exportManager.RegisterExporter(new SourcesCodeDocumentExporter());

        }

        private void richEdit_InitializeDocument(object sender, EventArgs e)
        {
            var document = richEdit.Document;
            document.BeginUpdate();
            try
            {
                document.DefaultCharacterProperties.FontName = "Courier New";
                document.DefaultCharacterProperties.FontSize = 10;
                document.Sections[0].Page.Width = Units.InchesToDocumentsF(100);
                document.Sections[0].LineNumbering.CountBy = 1;
                document.Sections[0].LineNumbering.RestartType = LineNumberingRestart.Continuous;

                SizeF tabSize = richEdit.MeasureSingleLineString("    ", document.DefaultCharacterProperties);
                TabInfoCollection tabs = document.Paragraphs[0].BeginUpdateTabs(true);
                try
                {
                    for (int i = 1; i <= 30; i++)
                    {
                        DevExpress.XtraRichEdit.API.Native.TabInfo tab =
                            new DevExpress.XtraRichEdit.API.Native.TabInfo();
                        tab.Position = i*tabSize.Width;
                        tabs.Add(tab);
                    }
                }
                finally
                {
                    document.Paragraphs[0].EndUpdateTabs(tabs);
                }
            }
            finally
            {
                document.EndUpdate();
            }
        }

        private void richEdit_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            e.Menu = null;
        }


        #region SyntaxHighlightService

        public class SyntaxHighlightService : ISyntaxHighlightService
        {
            #region Fields

            private readonly RichEditControl editor;
            private readonly SyntaxHighlightInfo syntaxHighlightInfo;

            #endregion


            public SyntaxHighlightService(RichEditControl editor)
            {
                this.editor = editor;
                this.syntaxHighlightInfo = new SyntaxHighlightInfo();
            }


            #region ISyntaxHighlightService Members

            void ISyntaxHighlightService.ForceExecute()
            {
                this.Execute();
            }

            void ISyntaxHighlightService.Execute()
            {
                this.Execute();
            }

            #endregion

            private void Execute()
            {
                var tokens = Parse(editor.Text);
                HighlightSyntax(tokens);
            }

            private TokenCollection Parse(string code)
            {
                if (string.IsNullOrEmpty(code))
                    return null;
                ITokenCategoryHelper tokenizer = CreateTokenizer();
                if (tokenizer == null)
                    return new TokenCollection();
                return tokenizer.GetTokens(code);
            }

            private ITokenCategoryHelper CreateTokenizer()
            {
                string fileName = editor.Options.DocumentSaveOptions.CurrentFileName;
                if (String.IsNullOrEmpty(fileName))
                    return null;
                ITokenCategoryHelper result =
                    TokenCategoryHelperFactory.CreateHelperForFileExtensions(Path.GetExtension(fileName));
                if (result != null)
                    return result;
                else
                    return null;
            }

            private void HighlightSyntax(TokenCollection tokens)
            {
                if (tokens == null || tokens.Count == 0)
                    return;
                Document document = editor.Document;
                CharacterProperties cp = document.BeginUpdateCharacters(0, 1);

                List<SyntaxHighlightToken> syntaxTokens = new List<SyntaxHighlightToken>(tokens.Count);
                foreach (Token token in tokens)
                {
                    HighlightCategorizedToken((CategorizedToken) token, syntaxTokens);
                }
                document.ApplySyntaxHighlight(syntaxTokens);
                document.EndUpdateCharacters(cp);
            }

            private void HighlightCategorizedToken(CategorizedToken token, List<SyntaxHighlightToken> syntaxTokens)
            {
                SyntaxHighlightProperties highlightProperties =
                    syntaxHighlightInfo.CalculateTokenCategoryHighlight(token.Category);
                SyntaxHighlightToken syntaxToken = SetTokenColor(token, highlightProperties, editor.ActiveView.BackColor);
                if (syntaxToken != null)
                    syntaxTokens.Add(syntaxToken);
            }

            private SyntaxHighlightToken SetTokenColor(Token token, SyntaxHighlightProperties foreColor, Color backColor)
            {
                if (editor.Document.Paragraphs.Count < token.Range.Start.Line)
                    return null;
                int paragraphStart =
                    DocumentHelper.GetParagraphStart(editor.Document.Paragraphs[token.Range.Start.Line - 1]);
                int tokenStart = paragraphStart + token.Range.Start.Offset - 1;
                if (token.Range.End.Line != token.Range.Start.Line)
                    paragraphStart =
                        DocumentHelper.GetParagraphStart(editor.Document.Paragraphs[token.Range.End.Line - 1]);

                int tokenEnd = paragraphStart + token.Range.End.Offset - 1;
                System.Diagnostics.Debug.Assert(tokenEnd > tokenStart);
                return new SyntaxHighlightToken(tokenStart, tokenEnd - tokenStart, foreColor);
            }
        }

        #endregion

        #region SyntaxHighlightInfo

        public class SyntaxHighlightInfo
        {
            private readonly Dictionary<TokenCategory, SyntaxHighlightProperties> properties;

            public SyntaxHighlightInfo()
            {
                this.properties = new Dictionary<TokenCategory, SyntaxHighlightProperties>();
                Reset();
            }

            public void Reset()
            {
                properties.Clear();
                Add(TokenCategory.Text, DXColor.Black);
                Add(TokenCategory.Keyword, DXColor.Blue);
                Add(TokenCategory.String, DXColor.Brown);
                Add(TokenCategory.Comment, DXColor.Green);
                Add(TokenCategory.Identifier, DXColor.Black);
                Add(TokenCategory.PreprocessorKeyword, DXColor.Blue);
                Add(TokenCategory.Number, DXColor.Red);
                Add(TokenCategory.Operator, DXColor.Black);
                Add(TokenCategory.Unknown, DXColor.Black);
                Add(TokenCategory.XmlComment, DXColor.Gray);

                Add(TokenCategory.CssComment, DXColor.Green);
                Add(TokenCategory.CssKeyword, DXColor.Brown);
                Add(TokenCategory.CssPropertyName, DXColor.Red);
                Add(TokenCategory.CssPropertyValue, DXColor.Blue);
                Add(TokenCategory.CssSelector, DXColor.Blue);
                Add(TokenCategory.CssStringValue, DXColor.Blue);

                Add(TokenCategory.HtmlAttributeName, DXColor.Red);
                Add(TokenCategory.HtmlAttributeValue, DXColor.Blue);
                Add(TokenCategory.HtmlComment, DXColor.Green);
                Add(TokenCategory.HtmlElementName, DXColor.Brown);
                Add(TokenCategory.HtmlEntity, DXColor.Gray);
                Add(TokenCategory.HtmlOperator, DXColor.Black);
                Add(TokenCategory.HtmlServerSideScript, DXColor.Black);
                Add(TokenCategory.HtmlString, DXColor.Blue);
                Add(TokenCategory.HtmlTagDelimiter, DXColor.Blue);
            }

            private void Add(TokenCategory category, Color foreColor)
            {
                SyntaxHighlightProperties item = new SyntaxHighlightProperties();
                item.ForeColor = foreColor;
                properties.Add(category, item);
            }

            public SyntaxHighlightProperties CalculateTokenCategoryHighlight(TokenCategory category)
            {
                SyntaxHighlightProperties result = null;
                if (properties.TryGetValue(category, out result))
                    return result;
                else
                    return properties[TokenCategory.Text];
            }
        }

        #endregion

        #region CustomRichEditCommandFactoryService

        public class CustomRichEditCommandFactoryService : IRichEditCommandFactoryService
        {
            private readonly IRichEditCommandFactoryService service;

            public CustomRichEditCommandFactoryService(IRichEditCommandFactoryService service)
            {
                Guard.ArgumentNotNull(service, "service");
                this.service = service;
            }

            #region IRichEditCommandFactoryService Members

            RichEditCommand IRichEditCommandFactoryService.CreateCommand(RichEditCommandId id)
            {
                if (id.Equals(RichEditCommandId.InsertColumnBreak) || id.Equals(RichEditCommandId.InsertLineBreak) ||
                    id.Equals(RichEditCommandId.InsertPageBreak))
                    return service.CreateCommand(RichEditCommandId.InsertParagraph);
                return service.CreateCommand(id);
            }

            #endregion
        }

        #endregion




        public static class SourceCodeDocumentFormat
        {
            public static readonly DocumentFormat Id = new DocumentFormat(1325);
        }

        public class SourcesCodeDocumentImporter : PlainTextDocumentImporter
        {
            internal static readonly FileDialogFilter filter = new FileDialogFilter("Source Files",
                new string[] {"cs", "vb", "html", "htm", "js", "xml", "css"});

            public override FileDialogFilter Filter
            {
                get { return filter; }
            }

            public override DocumentFormat Format
            {
                get { return SourceCodeDocumentFormat.Id; }
            }
        }

        public class SourcesCodeDocumentExporter : PlainTextDocumentExporter
        {
            public override FileDialogFilter Filter
            {
                get { return SourcesCodeDocumentImporter.filter; }
            }

            public override DocumentFormat Format
            {
                get { return SourceCodeDocumentFormat.Id; }
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (Data == null)
            {
                Data= new DBCTEvent(ADatabase.Dxs);
            }

            try
            {
                var v = cbeEvents.SelectedItem as POCEventItem;
                if(v==null) throw new Exception("The key is not valid.");
                Data.Title = v.Title;
                Data.Key = v.Key;
                Data.Priority = Convert.ToInt32(tePriority.EditValue);
                Data.Script = richEdit.Text;
                Data.Save();
                DialogResult = true;
                this.Close();
            }
            catch(Exception ex)
            {
                POLMessageBox.ShowWarning(ex.Message);
            }
        }

        private void BCompile_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
