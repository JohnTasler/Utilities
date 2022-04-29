namespace Tasler.IO.Ebml
{

	public interface IElementFactory
	{
		IElement CreateElement(IParent parent, long position);
	}
}
