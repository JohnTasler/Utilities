namespace Tasler.IO.Ebml
{
	using System;

	public abstract class ElementIdMapping
	{
		protected ElementIdMapping(Type elementType, string elementName, ulong id)
		{
			this.ElementType = elementType;
			this.ElementName = elementName;
			this.Id = id;
		}

		public Type ElementType { get; private set; }
		public string ElementName { get; private set; }
		public ulong Id { get; private set; }

		public abstract bool HasAssignValueAction { get; }
		public abstract bool HasAssignDefaultAction { get; }

		public abstract IElement CreateElement();
		public abstract void AssignValue(IParent parent, IElement element);
		public abstract void AssignDefault(IParent parent);
	}

	public class ElementIdMapping<TElement> : ElementIdMapping
		where TElement : class, IElement, new()
	{
		public ElementIdMapping(string elementName, ulong id)
			: base(typeof(TElement), elementName, id)
		{
		}

		public override bool HasAssignValueAction
		{
			get { return false; }
		}

		public override bool HasAssignDefaultAction
		{
			get { return false; }
		}

		public override IElement CreateElement()
		{
			return new TElement();
		}

		public override void AssignValue(IParent parent, IElement element)
		{
			var typedElement = element as TElement;
			if (typedElement == null)
				throw new ArgumentException("element", "Argument must be an instance of type " + typeof(TElement).FullName);
		}

		public override void AssignDefault(IParent parent)
		{
		}
	}

	public sealed class ElementIdMapping<TParent, TElement> : ElementIdMapping<TElement>
		where TParent : class, IParent
		where TElement : class, IElement, new()
	{
		public ElementIdMapping(string elementName, ulong id)
			: base(elementName, id)
		{
		}

		public Action<TParent, TElement> AssignValueAction { get; set; }

		public Action<TParent> AssignDefaultAction { get; set; }

		public override bool HasAssignValueAction
		{
			get { return this.AssignValueAction != null; }
		}
	
		public override bool HasAssignDefaultAction
		{
			get { return this.AssignDefaultAction != null; }
		}

		public override void AssignValue(IParent parent, IElement element)
		{
			var typedParent = parent as TParent;
			if (typedParent == null)
				throw new ArgumentException("parent", "Argument must be an instance of type " + typeof(TParent).FullName);

			var typedElement = element as TElement;
			if (typedElement == null)
				throw new ArgumentException("element", "Argument must be an instance of type " + typeof(TElement).FullName);

			if (this.AssignValueAction != null)
				this.AssignValueAction(typedParent, typedElement);
		}

		public override void AssignDefault(IParent parent)
		{
			var typedParent = parent as TParent;
			if (typedParent == null)
				throw new ArgumentException("parent", "Argument must be an instance of type " + typeof(TParent).FullName);

			if (this.AssignDefaultAction != null)
				this.AssignDefaultAction(typedParent);
		}
	}
}
