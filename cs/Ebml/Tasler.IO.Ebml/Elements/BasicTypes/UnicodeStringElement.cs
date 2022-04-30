namespace Tasler.IO.Ebml
{
	using System.Text;

	public class UnicodeStringElement : StringElement
	{
		protected override Encoding Encoding
		{
			get { return Encoding.UTF8; }
		}
	}
}
