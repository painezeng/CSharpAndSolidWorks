using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace CSharpAndSolidWorks
{
    public class ComStream : Stream
    {
        //The managed stream being wrapped
        private IStream originalStream_;

        public ComStream(IStream stream)
        {
            if (stream != null)
            {
                originalStream_ = stream;
            }
            else
            {
                throw new ArgumentNullException("stream");
            }
        }

        ~ComStream()
        {
            Close();
        }

        // property to get original stream object
        public IStream UnderlyingStream
        {
            get
            {
                return originalStream_;
            }
        }

        // reads a specified number of bytes from the stream object
        // into memory starting at the current seek pointer
        public override unsafe int Read(byte[] buffer, int offset, int count)
        {
            if (originalStream_ == null)
            {
                throw new ObjectDisposedException("originalStream_");
            }

            if (offset != 0)
            {
                throw new NotSupportedException("only 0 offset is supported");
            }

            int bytesRead;

            IntPtr address = new IntPtr(&bytesRead);

            originalStream_.Read(buffer, count, address);

            return bytesRead;
        }

        // writes a specified number of bytes into the stream object
        // starting at the current seek pointer
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (originalStream_ == null)
            {
                throw new ObjectDisposedException("originalStream_");
            }

            if (offset != 0)
            {
                throw new NotSupportedException("only 0 offset is supported");
            }

            originalStream_.Write(buffer, count, IntPtr.Zero);
        }

        // changes the seek pointer to a new location relative to the beginning
        // of the stream, the end of the stream, or the current seek position

        public override unsafe long Seek(long offset, SeekOrigin origin)
        {
            if (originalStream_ == null)
            {
                throw new ObjectDisposedException("originalStream_");
            }

            long position = 0;

            IntPtr address = new IntPtr(&position);

            originalStream_.Seek(offset, (int)origin, address);

            return position;
        }

        public override long Length
        {
            get
            {
                if (originalStream_ == null)
                {
                    throw new ObjectDisposedException("originalStream_");
                }

                STATSTG statstg;

                originalStream_.Stat(out statstg, 1 /* STATSFLAG_NONAME*/ );

                return statstg.cbSize;
            }
        }

        public override long Position
        {
            get
            {
                return Seek(0, SeekOrigin.Current);
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        // changes the size of the stream object
        public override void SetLength(long value)
        {
            if (originalStream_ == null)
            {
                throw new ObjectDisposedException("originalStream_");
            }

            originalStream_.SetSize(value);
        }

        // closes (disposes) the stream
        public override void Close()
        {
            if (originalStream_ != null)
            {
                originalStream_.Commit(0);

                // Marshal.ReleaseComObject( originalStream_ );
                originalStream_ = null;

                GC.SuppressFinalize(this);
            }
        }

        public override void Flush()
        {
            originalStream_.Commit(0);
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }
    }
}