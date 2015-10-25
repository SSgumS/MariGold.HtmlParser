﻿namespace MariGold.HtmlParser
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	
	internal sealed class FirstChildSelector : CSSelector
	{
		private string selectorText;
		
		private readonly Regex regex;
		
		internal FirstChildSelector(ISelectorContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}

			this.context = context;
			
			regex = new Regex("^:first-child");
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
				foreach (HtmlNode child in node.Parent.Children)
				{
					//Find first child tag which matches the node's tag. The break statement will discard the loop after finding the first matching node.
					if (string.Compare(node.Tag, child.Tag, true) == 0)
					{
						//If the node is the first child, it will apply the styles.
						if (node == child)
						{
							isValid = true;
							break;
						}
					}
				}
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
