namespace Tasler.IO.Ebml
{

	public interface IElement : IHaveReader
	{
		#region Properties
		IParent Parent { get; }

		long Position { get; }

		ulong Id { get; }

		ulong Size { get; }

		long DataOffset { get; }

		string ElementName { get; }
		#endregion Properties

		#region Methods
		void Initialize(IParent parent, long position, ulong id, long idLength, string elementName);
		#endregion Methods
	}
}
