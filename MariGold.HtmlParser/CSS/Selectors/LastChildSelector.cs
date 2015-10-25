﻿namespace MariGold.HtmlParser
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	
	internal sealed class LastChildSelector : CSSelector
	{
		private readonly Regex regex;
		
		private string selectorText;
		
		internal LastChildSelector(ISelectorContext context)
		{
			if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.context = context;
            regex = new Regex("^(:last-child)|(:last-of-type)");
		}
		
		internal override bool Prepare(string selector)
		{
			Match match = regex.Match(selector);
			
			this.selectorText = string.Empty;
			
			if (match.Success)
			{
				this.selectorText = selector.Substring(match.Value.Length);
			}
			
			return match.Success;
		}
		
		internal override bool IsValidNode(HtmlNode node)
		{
			bool isValid = false;
			
			if (node != null && node.Parent != null)
			{
				HtmlNode lastChild = null;
				
				foreach (HtmlNode child in node.Parent.Children)
				{
					if (string.Compare(node.Tag, child.Tag, true) == 0)
					{
						lastChild = child;
					}
				}
				
				isValid = lastChild != null && lastChild == node;
			}
			
			return isValid;
		}
		
		internal override void Parse(HtmlNode node, List<HtmlStyle> htmlStyles)
		{
			if (IsValidNode(node))
			{
				if (string.IsNullOrEmpty(this.selectorText))
				{
					ApplyStyle(node, htmlStyles);
				}
				else
				{
					context.ParseBehavior(this.selectorText, node, htmlStyles);
				}
			}
		}
		
		internal override void ApplyStyle(HtmlNode node, List<HtmlStyle> htmlStyles)
		{
			node.CopyHtmlStyles(htmlStyles, SelectorWeight.Child);
		}
	}
}
