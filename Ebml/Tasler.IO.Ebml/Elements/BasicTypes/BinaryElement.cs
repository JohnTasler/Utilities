namespace Tasler.IO.Ebml
{
	using System;
	using System.IO;

	public class BinaryElement : Element
	{
		public Stream Value { get; private set; }

		public override string ToString()
		{
			return string.Format("{0} Value=(binary data)", base.ToString());
		}

		protected override void OnInitialized()
		{
			this.Value = new ReaderStream(this);
		}

		private class ReaderStream : Stream
		{
			public ReaderStream(IElement element)
			{
				this.Element = element;
			}

			private IElement Element { get; set; }

			public override bool CanRead
			{
				get { return true; }
			}

			public override bool CanSeek
			{
				get { return true; }
			}

			public override bool CanWrite
			{
				get { return false; }
			}

			public override void Flush()
			{
			}

			public override long Length
			{
				get { return (long)this.Element.Size; }
			}

			public override long Position { get; set; }

			public override int Read(byte[] buffer, int offset, int count)
			{
				var bytesRead = this.Element.Reader.ReadBytes(this.Element.DataOffset + this.Position, buffer);
				this.Position += bytesRead;
				return bytesRead;
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				switch (origin)
				{
					case SeekOrigin.Begin:
						return this.Position = offset;

					case SeekOrigin.Current:
						return this.Position += offset;

					case SeekOrigin.End:
						return this.Position = this.Length - offset;

					default:
						throw new ArgumentException("Unknown SeekOrigin enumeration value: " + origin);
				}
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException("The stream does not support writing.");
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException("The stream does not support writing.");
			}
		}
	}
}
