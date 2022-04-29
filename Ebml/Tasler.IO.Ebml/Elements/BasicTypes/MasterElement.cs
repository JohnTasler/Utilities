namespace Tasler.IO.Ebml
{
	using System.Collections.Generic;
	using System.Linq;
	
	public abstract class MasterElement : Element, IParent
	{
		#region Constants
		private const string classNameSuffix = "Element";
		#endregion Constants

		#region IParent Interface Members
		public IEnumerable<IElement> Children
		{
			get
			{
				long position = this.DataPosition;
				long endPosition = position + (long)this.Size;
				while (position < endPosition)
				{
					var element = ElementFactory.CreateElement(this, position);
					position += element.DataOffset + (long)element.Size;

					yield return element;
				}
			}
		}

		public ElementIdMapping FindElementMapping(ulong id)
		{
			var elementIdMap = this.GetElementIdMap();
			var result = (elementIdMap != null) ? elementIdMap.FindElementMapping(id) : null;

			return result;
		}
		#endregion IParent Interface Members

		#region Overridables
		protected abstract ElementIdMap GetElementIdMap();
		#endregion Overridables

		#region Overrides
		protected override void OnInitialized()
		{
			var elementIdMap = this.GetElementIdMap();
			if (elementIdMap != null)
			{
				// Set default values
				foreach (var mapping in elementIdMap.Where(m => m.HasAssignDefaultAction))
					mapping.AssignDefault(this);

				// Find values in children
				if (elementIdMap.Where(m => m.HasAssignValueAction).Any())
				{
					foreach (var child in this.Children)
					{
						var mapping = this.FindElementMapping(child.Id);
						if (mapping != null)
							mapping.AssignValue(this, child);
					}
				}
			}
		}
		#endregion Overrides
	}
}
