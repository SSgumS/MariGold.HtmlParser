﻿namespace MariGold.HtmlParser
{
	using System;
	using System.Collections.Generic;

	internal sealed class SelectorContext : ISelectorContext
	{
		private readonly List<IAttachedSelector> attachedSelectors;
		
		private List<CSSelector> selectors;
		private List<CSSBehavior> behaviors;
		
		internal SelectorContext()
		{
			attachedSelectors = new List<IAttachedSelector>();
			
			behaviors = new List<CSSBehavior>()
			{
				new ApplyAllChildren(this),
				new ApplyImmediateChildren(this),
				new ApplyNextElement(this),
				new ApplyAllNextElement(this)
			};
			
			selectors = new List<CSSelector>()
			{
				new IdentitySelector(this),
				new ClassSelector(this),
				new AttributeSelector(this),
				new ElementSelector(this),
				new FirstChildSelector(this),
				new LastChildSelector(this),
				new NotSelector(this),
				new NthChildSelector(this),
				new GlobalSelector()
			};			
		}
		
		public List<CSSBehavior> CSSBehaviors
		{
			get
			{
				return behaviors;
			}
		}
		
		public List<CSSelector> Selectors
		{
			get
			{
				return selectors;
			}
		}
		
		public List<IAttachedSelector> AttachedSelectors
		{
			get
			{
				return attachedSelectors;
			}
		}
		
		public void AddAttachedSelector(IAttachedSelector selector)
		{
			attachedSelectors.Add(selector);
		}
		
		public bool ParseSelector(string selectorText, out CSSelector selector)
		{
			selector = null;
			
			foreach (CSSelector item in selectors)
			{
				if (item.Prepare(selectorText))
				{
					selector = item;
					
					return true;
				}
			}
			
			return false;
		}
		
		public bool ParseBehavior(string selectorText, HtmlNode node, List<HtmlStyle> htmlStyles)
		{
			bool foundBehavior = false;
			
			foreach (CSSBehavior behavior in CSSBehaviors)
			{
				if (behavior.IsValidBehavior(selectorText))
				{
					foundBehavior = true;
					
					behavior.Parse(node, htmlStyles);
					
					break;
				}
			}

			return foundBehavior;
		}
		
		public bool ParseSelectorOrBehavior(string selectorText, HtmlNode node, List<HtmlStyle> htmlStyles)
		{
			bool found = false;
			
			foreach (IAttachedSelector selector in attachedSelectors)
			{
				if (selector.Prepare(selectorText))
				{
					found = true;
					
					selector.Parse(node, htmlStyles);
					
					break;
				}
			}
			
			if (!found)
			{
				found = ParseBehavior(selectorText, node, htmlStyles);
			}
			
			return found;
		}
	}
}
