﻿namespace MariGold.HtmlParser
{
    using System;

    internal sealed class MetaTagAnalyzer : HtmlAnalyzer, IOpenTag
    {
        private int startPosition;
        private HtmlNode parent;
        private int tagStart;
        private string tag;

        public MetaTagAnalyzer(IAnalyzerContext context) : base(context) { }

        private void ExtractTag(int position)
        {
            if (tagStart > -1 && tagStart <= position)
            {
                tag = context.Html.Substring(tagStart, position - tagStart + 1);

                TagCreated(tag);
            }
        }

        protected override bool ProcessHtml(int position, ref HtmlNode node)
        {
            bool tagCreated = false;
            char letter = context.Html[position];

            if (tagStart == -1 && IsValidHtmlLetter(letter))
            {
                tagStart = position;
            }

            if (string.IsNullOrEmpty(tag) && tagStart > -1 && !IsValidHtmlLetter(letter))
            {
                ExtractTag(position - 1);

                this.AddAnalyzer("attributeAnalyzer", new AttributeAnalyzer(context));
            }

            if (letter == HtmlTag.closeAngle)
            {
                if (string.IsNullOrEmpty(tag))
                {
                    ExtractTag(position - 1);
                }

                tagCreated = CreateTag(tag, startPosition, startPosition, position + 1, position + 1, parent, out node);

                if (node != null)
                {
                    node.SelfClosing = true;
                }

                this.FinalizeSubAnalyzers(position, ref node);

                if (!AssignNextAnalyzer(position + 1, parent))
                {
                    context.SetAnalyzer(context.GetTextAnalyzer(position + 1, parent));
                }
            }

            return tagCreated;
        }

        protected override void Finalize(int position, ref HtmlNode node)
        {
        }

        public bool IsOpenTag(int position, string html)
        {
            if (position + 2 >= context.EOF)
            {
                return false;
            }

            char plus2 = html[position + 2];

            return html[position] == HtmlTag.openAngle && html[position + 1] == HtmlTag.exclamation &&
                (plus2 == HtmlTag.space || char.IsLetter(plus2));
        }

        public HtmlAnalyzer GetAnalyzer(int position, HtmlNode parent)
        {
            if (position < 0 || position > context.EOF)
            {
                throw new ArgumentOutOfRangeException("position");
            }

            MetaTagAnalyzer analyzer = new MetaTagAnalyzer(context);

            analyzer.startPosition = position;
            analyzer.parent = parent;
            analyzer.tagStart = -1;
            analyzer.tag = string.Empty;

            return analyzer;
        }
    }
}
