using PSWStandalon;
using SolidWorks.Interop.sldworks;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace CSharpAndSolidWorks
{
    internal class BodyHelper
    {
        [System.Runtime.InteropServices.DllImport("ole32")]
        private static extern long CreateStreamOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease, ref IStream ppstm);

        //private const string FILE_PATH = @"D:\body.dat";

        public static void ExportBodyToFile(string FILE_PATH)
        {
            ISldWorks app = PStandAlone.GetSolidWorks();
            app.Visible = true;

            IModelDoc2 swModel;

            swModel = app.IActiveDoc2;

            if (swModel != null)
            {
                IBody2 body = (Body2)swModel.ISelectionManager.GetSelectedObject6(1, -1);

                if (body != null)
                    SaveBodyToFile(body, FILE_PATH);
                else
                    throw new Exception("Please select body to export");
            }
            else
                throw new Exception("Please open the model");
        }

        /// <summary>
        /// 把body导出为文件流
        /// </summary>
        /// <param name="body"></param>
        /// <param name="filePath"></param>
        public static void SaveBodyToFile(IBody2 body, string filePath)
        {
            IStream stream = null;

            CreateStreamOnHGlobal(IntPtr.Zero, true, ref stream);
            body.Save(stream);

            var comStream = new ComStreamBody(ref stream, false, false);

            using (var fileStream = File.Create(filePath))
            {
                comStream.Seek(0, SeekOrigin.Begin);
                comStream.CopyTo(fileStream);
            }
        }

        /// <summary>
        /// 从文件加载实体
        /// </summary>
        /// <param name="app"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static IBody2 LoadBodyFromFile(ISldWorks app, string filePath)
        {
            IStream stream = null;

            CreateStreamOnHGlobal(IntPtr.Zero, true, ref stream);

            var comStream = new ComStreamBody(ref stream, true, true);

            using (var fileStream = File.OpenRead(filePath))
            {
                fileStream.CopyTo(comStream);
                comStream.Seek(0, SeekOrigin.Begin);
            }

            IModeler modeler = app.IGetModeler();

            return (IBody2)modeler.Restore(stream);
        }
    }

    public class ComStreamBody : Stream
    {
        private readonly IStream m_ComStream;
        private readonly bool m_Commit;
        private bool m_IsWritable;

        public ComStreamBody(ref IStream comStream, bool writable, bool commit = true)
        {
            if (comStream == null)
                throw new ArgumentNullException(nameof(comStream));

            m_ComStream = comStream;
            m_IsWritable = writable;
            m_Commit = commit;
        }

        public override bool CanRead
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

        public override bool CanWrite
        {
            get
            {
                return m_IsWritable;
            }
        }

        public override long Length
        {
            get
            {
                const int STATSFLAG_NONAME = 1;

                System.Runtime.InteropServices.ComTypes.STATSTG stats = default(System.Runtime.InteropServices.ComTypes.STATSTG);
                m_ComStream.Stat(out stats, STATSFLAG_NONAME);

                return stats.cbSize;
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

        public override void Flush()
        {
            if (m_Commit)
            {
                const int STGC_DEFAULT = 0;
                m_ComStream.Commit(STGC_DEFAULT);
            }
        }

        public override void SetLength(long Value)
        {
            m_ComStream.SetSize(Value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (offset != 0)
            {
                int bufferSize;
                bufferSize = buffer.Length - offset;
                byte[] tmpBuffer = new byte[bufferSize + 1];
                Array.Copy(buffer, offset, tmpBuffer, 0, bufferSize);
                m_ComStream.Write(tmpBuffer, bufferSize, default(IntPtr));
            }
            else
                m_ComStream.Write(buffer, count, default(IntPtr));
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;
            object boxBytesRead = bytesRead;
            GCHandle hObject = GCHandle.Alloc(boxBytesRead, GCHandleType.Pinned);

            try
            {
                IntPtr pBytesRead = hObject.AddrOfPinnedObject();

                if (offset != 0)
                {
                    byte[] tmpBuffer = new byte[count - 1 + 1];
                    m_ComStream.Read(tmpBuffer, count, pBytesRead);
                    bytesRead = System.Convert.ToInt32(boxBytesRead);
                    Array.Copy(tmpBuffer, 0, buffer, offset, bytesRead);
                }
                else
                {
                    m_ComStream.Read(buffer, count, pBytesRead);
                    bytesRead = System.Convert.ToInt32(boxBytesRead);
                }
            }
            finally
            {
                if (hObject.IsAllocated)
                    hObject.Free();
            }

            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long curPosition = 0;
            object boxCurPosition = curPosition;
            GCHandle hObject = GCHandle.Alloc(boxCurPosition, GCHandleType.Pinned);

            try
            {
                IntPtr pCurPosition = hObject.AddrOfPinnedObject();

                m_ComStream.Seek(offset, (int)origin, pCurPosition);
                curPosition = System.Convert.ToInt64(boxCurPosition);
            }
            finally
            {
                if (hObject.IsAllocated)
                    hObject.Free();
            }

            return curPosition;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                    m_IsWritable = false;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        ~ComStreamBody()
        {
            Dispose(false);
        }
    }
}