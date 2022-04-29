namespace Tasler.IO.Ebml
{
	using System.IO;

	/// <summary>
	/// Extension methods for the <see cref="Stream"/> class.
	/// </summary>
	public static class StreamExtensions
	{
		/// <summary>
		/// Reads a byte from the stream and advances the position within the stream by one byte,
		/// or throws an <see cref=EndOfStreamException""/> if at the end of the stream.
		/// </summary>
		/// <param name="stream">The stream from which to read the byte.</param>
		/// <returns>The unsigned byte that was read from the stream.</returns>
		/// <exception cref="EndOfStreamException">Reading was attempted past the end of the stream.</exception>
		public static byte ReadByteOrThrow(this Stream stream)
		{
			int value = stream.ReadByte();
			if (value != -1)
				return (byte)value;

			throw new EndOfStreamException();
		}
	}
}
