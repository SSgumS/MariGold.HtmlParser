﻿namespace MariGold.HtmlParser
{
    using System;
    using System.Collections.Generic;

    internal sealed class GlobalSelector : CSSelector
    {
        private const string globalSelector = "*";

        internal override CSSelector Parse(string selector)
        {
            if (selector == globalSelector)
            {
                return this;
            }
            else
            {
                return PassToSuccessor(selector);
            }
        }

        internal override void Parse(HtmlNode node, List<HtmlStyle> htmlStyles)
        {
            if (IsValidNode(node))
            {
				ApplyStyle(node, htmlStyles);
            }
        }

        internal override bool IsValidNode(HtmlNode node)
        {
            if (node == null)
            {
                return false;
            }

            return HtmlStyle.IsNonStyleElement(node.Tag);
        }
        
		internal override void ApplyStyle(HtmlNode node, List<HtmlStyle> htmlStyles)
		{
			node.CopyHtmlStyles(htmlStyles, SelectorWeight.Global);
		}
    }
}
