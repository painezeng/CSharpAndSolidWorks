using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace CSharpAndSolidWorks
{
    public class Win32Helper
    {
        internal const string IShellItem2Guid = "7E9FB0D3-919F-4307-AB2E-9B1860310C93";

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string path,
            // The following parameter is not used - binding context.
            IntPtr pbc,
            ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem shellItem);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr hObject);

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
        internal interface IShellItem
        {
            void BindToHandler(IntPtr pbc,
                [MarshalAs(UnmanagedType.LPStruct)] Guid bhid,
                [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                out IntPtr ppv);

            void GetParent(out IShellItem ppsi);

            void GetDisplayName(SIGDN sigdnName, out IntPtr ppszName);

            void GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);

            void Compare(IShellItem psi, uint hint, out int piOrder);
        };

        public enum ThumbnailOptions : uint
        {
            None = 0,
            ReturnOnlyIfCached = 1,
            ResizeThumbnail = 2,
            UseCurrentScale = 4
        }

        internal enum SIGDN : uint
        {
            NORMALDISPLAY = 0,
            PARENTRELATIVEPARSING = 0x80018001,
            PARENTRELATIVEFORADDRESSBAR = 0x8001c001,
            DESKTOPABSOLUTEPARSING = 0x80028000,
            PARENTRELATIVEEDITING = 0x80031001,
            DESKTOPABSOLUTEEDITING = 0x8004c000,
            FILESYSPATH = 0x80058000,
            URL = 0x80068000
        }

        internal enum HResult
        {
            Ok = 0x0000,
            False = 0x0001,
            InvalidArguments = unchecked((int)0x80070057),
            OutOfMemory = unchecked((int)0x8007000E),
            NoInterface = unchecked((int)0x80004002),
            Fail = unchecked((int)0x80004005),
            ElementNotFound = unchecked((int)0x80070490),
            TypeElementNotFound = unchecked((int)0x8002802B),
            NoObject = unchecked((int)0x800401E5),
            Win32ErrorCanceled = 1223,
            Canceled = unchecked((int)0x800704C7),
            ResourceInUse = unchecked((int)0x800700AA),
            AccessDenied = unchecked((int)0x80030005)
        }

        [ComImportAttribute()]
        [GuidAttribute("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IShellItemImageFactory
        {
            [PreserveSig]
            HResult GetImage(
            [In, MarshalAs(UnmanagedType.Struct)] NativeSize size,
            [In] ThumbnailOptions flags,
            [Out] out IntPtr phbm);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NativeSize
        {
            private int width;
            private int height;

            public int Width { set { width = value; } }
            public int Height { set { height = value; } }
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }
    }

    public class ThumbnailHelper
    {
        #region instance

        private static object ooo = new object();
        private static ThumbnailHelper _ThumbnailHelper;

        private ThumbnailHelper()
        {
        }

        public static ThumbnailHelper GetInstance()
        {
            if (_ThumbnailHelper == null)
            {
                lock (ooo)
                {
                    if (_ThumbnailHelper == null)
                        _ThumbnailHelper = new ThumbnailHelper();
                }
            }
            return _ThumbnailHelper;
        }

        #endregion instance

        #region public methods

        public string GetJPGThumbnail(string filename, int width = 80, int height = 80, Win32Helper.ThumbnailOptions options = Win32Helper.ThumbnailOptions.None)
        {
            if (!File.Exists(filename))
                return string.Empty;
            Bitmap bit = GetBitmapThumbnail(filename, width, height, options);
            if (bit == null)
                return string.Empty;
            return GetThumbnailFilePath(bit);
        }

        #endregion public methods

        #region private methods

        private Bitmap GetBitmapThumbnail(string fileName, int width = 80, int height = 80, Win32Helper.ThumbnailOptions options = Win32Helper.ThumbnailOptions.None)
        {
            IntPtr hBitmap = GetHBitmap(System.IO.Path.GetFullPath(fileName), width, height, options);

            try
            {
                Bitmap bmp = Bitmap.FromHbitmap(hBitmap);

                if (Bitmap.GetPixelFormatSize(bmp.PixelFormat) < 32)
                    return bmp;

                return CreateAlphaBitmap(bmp, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            finally
            {
                // delete HBitmap to avoid memory leaks
                Win32Helper.DeleteObject(hBitmap);
            }
        }

        private Bitmap CreateAlphaBitmap(Bitmap srcBitmap, System.Drawing.Imaging.PixelFormat targetPixelFormat)
        {
            Bitmap result = new Bitmap(srcBitmap.Width, srcBitmap.Height, targetPixelFormat);

            System.Drawing.Rectangle bmpBounds = new System.Drawing.Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);

            BitmapData srcData = srcBitmap.LockBits(bmpBounds, ImageLockMode.ReadOnly, srcBitmap.PixelFormat);

            bool isAlplaBitmap = false;

            try
            {
                for (int y = 0; y <= srcData.Height - 1; y++)
                {
                    for (int x = 0; x <= srcData.Width - 1; x++)
                    {
                        System.Drawing.Color pixelColor = System.Drawing.Color.FromArgb(
                            Marshal.ReadInt32(srcData.Scan0, (srcData.Stride * y) + (4 * x)));

                        if (pixelColor.A > 0 & pixelColor.A < 255)
                        {
                            isAlplaBitmap = true;
                        }

                        result.SetPixel(x, y, pixelColor);
                    }
                }
            }
            finally
            {
                srcBitmap.UnlockBits(srcData);
            }

            if (isAlplaBitmap)
            {
                return result;
            }
            else
            {
                return srcBitmap;
            }
        }

        private IntPtr GetHBitmap(string fileName, int width, int height, Win32Helper.ThumbnailOptions options)
        {
            Win32Helper.IShellItem nativeShellItem;
            Guid shellItem2Guid = new Guid(Win32Helper.IShellItem2Guid);
            int retCode = Win32Helper.SHCreateItemFromParsingName(fileName, IntPtr.Zero, ref shellItem2Guid, out nativeShellItem);

            if (retCode != 0)
                throw Marshal.GetExceptionForHR(retCode);

            Win32Helper.NativeSize nativeSize = new Win32Helper.NativeSize();
            nativeSize.Width = width;
            nativeSize.Height = height;

            IntPtr hBitmap;
            Win32Helper.HResult hr = ((Win32Helper.IShellItemImageFactory)nativeShellItem).GetImage(nativeSize, options, out hBitmap);

            Marshal.ReleaseComObject(nativeShellItem);

            if (hr == Win32Helper.HResult.Ok) return hBitmap;

            throw Marshal.GetExceptionForHR((int)hr);
        }

        /// <summary>
        /// 获取临时文件
        /// </summary>
        /// <returns></returns>
        private FileStream GetTemporaryFilePath(ref string filePath)
        {
            string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
            var index = path.IndexOf('.');
            string temp = path.Substring(0, index) + ".li";

            var fileStream = File.Create(temp);
            filePath = temp;
            return fileStream;
        }

        /// <summary>
        /// 参数
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        private ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private const int ExpectHeight = 200;
        private const int ExpectWidth = 200;

        /// <summary>
        /// 得到图片缩放后的大小  图片大小过小不考虑缩放了
        /// </summary>
        /// <param name="originSize">原始大小</param>
        /// <returns>改变后大小</returns>
        private System.Drawing.Size ScalePhoto(System.Drawing.Size originSize, bool can)
        {
            if (originSize.Height * originSize.Width < ExpectHeight * ExpectWidth)
            {
                can = false;
            }
            if (can)
            {
                bool isportrait = false;

                if (originSize.Width <= originSize.Height)
                {
                    isportrait = true;
                }

                if (isportrait)
                {
                    double ratio = (double)ExpectHeight / (double)originSize.Height;
                    return new System.Drawing.Size((int)(originSize.Width * ratio), (int)(originSize.Height * ratio));
                }
                else
                {
                    double ratio = (double)ExpectWidth / (double)originSize.Width;
                    return new System.Drawing.Size((int)(originSize.Width * ratio), (int)(originSize.Height * ratio));
                }
            }
            else
            {
                return new System.Drawing.Size(originSize.Width, originSize.Height);
            }
        }

        /// <summary>
        /// 获取缩略图文件
        /// </summary>
        /// <param name="BitmapIcon">缩略图</param>
        /// <returns>路径</returns>
        private string GetThumbnailFilePath(Bitmap bitmap)
        {
            var filePath = "";
            var fileStream = GetTemporaryFilePath(ref filePath);

            //bitmap.Save(filePath);

            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg"); //image code info 图形图像解码压缩
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            EncoderParameters encoderParameters = new EncoderParameters { Param = new EncoderParameter[] { myEncoderParameter } };
            var sizeScaled = ScalePhoto(bitmap.Size, true);
            //去黑色背景
            var finalBitmap = ClearBlackBackground(bitmap);
            finalBitmap.Save(fileStream, myImageCodecInfo, encoderParameters);
            fileStream.Dispose();
            return filePath;
        }

        private Bitmap ClearBlackBackground(Bitmap originBitmap)
        {
            using (var tempBitmap = new Bitmap(originBitmap.Width, originBitmap.Height))
            {
                tempBitmap.SetResolution(originBitmap.HorizontalResolution, originBitmap.VerticalResolution);

                using (var g = Graphics.FromImage(tempBitmap))
                {
                    g.Clear(System.Drawing.Color.White);
                    g.DrawImageUnscaled(originBitmap, 0, 0);
                }
                return new Bitmap(tempBitmap);
            }
        }

        #endregion private methods
    }
}