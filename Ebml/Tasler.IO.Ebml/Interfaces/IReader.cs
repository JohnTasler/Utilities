namespace Tasler.IO.Ebml
{

	public interface IReader : IHaveReader
	{
		ulong ReadId(long position, out long bytesRead);

		ulong ReadSize(long position, out long bytesRead);

		int ReadBytes(long position, byte[] buffer);

		int ReadBytes(long position, byte[] buffer, int offset, int count);
	}
}
