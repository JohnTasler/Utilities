namespace Tasler.IO.Ebml
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	public class EbmlReader : IReader, IParent
	{
		#region Constants
		// TODO: Make these based on the EBML element fields
		private const int maxIdLength = 4;
		private const int maxSizeLength = 8;
		#endregion Constants

		#region Static Fields
		private static readonly ElementIdMap[] elementIdMaps =
		{
			new ElementIdMap(new ElementIdMapping<EbmlReader, EbmlElement>("EBML", 0x1A45DFA3))
		};
		#endregion Static Fields

		#region Instance Fields
		private long startPosition;
		private Stream stream;
		#endregion Instance Fields

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="EbmlReader"/> class.
		/// </summary>
		/// <param name="stream">
		/// The stream from which to read. Its <see cref="Stream.CanSeek"/> and <see cref="Stream.CanRead"/>
		/// properties must both be <c>true</c>.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException"><paramref name="stream"/> is either not seekable or not readable.</exception>
		/// <remarks>
		/// <para>The <see cref="Stream.Position"/> at the time of construction is considered the beginning of the
		/// EBML data. Typically, this will also be the beginning of the <see cref="Stream"/>.</para>
		/// <para>The <see cref="EbmlReader"/> does not own the <see cref="Stream"/> specified by
		/// <paramref name="stream"/>, so closing or disposing of the <see cref="Stream"/> will continue to be the
		/// caller's responsibility. Additionally, the caller should not close or dispose of the <see cref="Stream"/>
		/// until it is finished using the <see cref="EbmlReader"/> instance.</para>
		/// </remarks>
		public EbmlReader(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (!stream.CanSeek)
				throw new ArgumentException("Specified stream must be seekable.", "stream");
			if (!stream.CanRead)
				throw new ArgumentException("Specified stream must be readable.", "stream");

			this.startPosition = stream.Position;
			this.stream = stream;
		}
		#endregion Constructors

		#region IParent Interface Members
		public IEnumerable<IElement> Children
		{
			get
			{
				long position = this.startPosition;
				long endPosition = this.stream.Length;

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
			foreach (var elementIdMap in this.GetElementIdMaps())
			{
				if (elementIdMap != null)
				{
					var result = elementIdMap.FindElementMapping(id);
					if (result != null)
						return result;
				}
			}

			return null;
		}
		#endregion IParent Interface Members

		#region IHaveReader Interface Members
		public IReader Reader
		{
			get { return this; }
		}
		#endregion IHaveReader Interface Members

		#region IReader Interface Members
		public ulong ReadId(long position, out long bytesRead)
		{
			return this.ReadVUInt(position, maxIdLength, true, out bytesRead);
		}

		public ulong ReadSize(long position, out long bytesRead)
		{
			return this.ReadVUInt(position, maxSizeLength, false, out bytesRead);
		}

		public int ReadBytes(long position, byte[] buffer)
		{
			return this.ReadBytes(position, buffer, 0, buffer.Length);
		}

		public int ReadBytes(long position, byte[] buffer, int offset, int count)
		{
			if (buffer == null || buffer.Length == 0)
				throw new ArgumentException("buffer", "Argument must be non-null and have a length greater than zero.");

			if (position < 0 || offset < 0 || count < 0)
				throw new ArgumentOutOfRangeException("position, offset, or count is negative.");

			if (offset + count > buffer.Length)
				throw new ArgumentException(
					string.Format("The sum of offset ({0:N0}) and count ({1:N0}) is larger than the buffer length ({2:N0}).",
						offset, count, buffer.Length));

			this.stream.Position = position;
			var bytesRead = this.stream.Read(buffer, offset, count);
			if (bytesRead < count || (count != 0 && bytesRead == 0))
				throw new EndOfStreamException();

			return bytesRead;
		}
		#endregion IReader Interface Members

		#region Overridables
		protected virtual IEnumerable<ElementIdMap> GetElementIdMaps()
		{
			return elementIdMaps;
		}
		#endregion Overridables

		#region Private Implementation
		private ulong ReadVUInt(long position, int maxLength, bool preserveLengthMark, out long bytesRead)
		{
			if (1 > maxLength || maxLength > 8)
				throw new ArgumentOutOfRangeException("maxLength", "This EBML implementation only supports up to 8 bytes of element size bytes.");

			this.stream.Position = position;
			byte next = this.stream.ReadByteOrThrow();

			int length = 1;

			byte mask = 0x80;
			for (; mask != 0 && (next & mask) == 0; mask = (byte)(mask >> 1 | 0x80))
				if (++length > maxLength)
					throw new InvalidDataException(
						string.Format("Unable to read variable int: position {0} maxLength={1}", position, maxLength));

			ulong value = preserveLengthMark ? next : (byte)(next & ~mask);
			while (--length != 0)
			{
				value <<= 8;
				value |= this.stream.ReadByteOrThrow();
			}

			bytesRead = this.stream.Position - position;
			return value;
		}
		#endregion Private Implementation
	}
}
