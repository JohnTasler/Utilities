namespace Tasler.IO.Ebml
{
	using System.Collections.Generic;

	public interface IParent : IHaveReader
	{
		#region Properties
		IEnumerable<IElement> Children { get; }
		#endregion Properties

		#region Methods
		ElementIdMapping FindElementMapping(ulong id);
		#endregion Methods
	}
}
