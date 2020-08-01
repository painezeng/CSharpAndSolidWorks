Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.ComTypes
Imports System.Text.RegularExpressions
Imports Microsoft.Win32
Imports SolidWorks.Interop.sldworks

Public Class PStandAlone

    Private Declare Function GetActiveObject Lib "oleaut32.dll" (ByRef rclsid As Guid, ByVal pvReserved As IntPtr, <MarshalAs(UnmanagedType.IUnknown)> ByRef ppunk As Object) As Integer
    Private Declare Function CreateBindCtx Lib "ole32.dll" (ByVal reserved As Integer, <Out> ByRef ppbc As ComTypes.IBindCtx) As Integer

    ''' <summary>
    ''' 创建SolidWorks或返回一个现有的实例 返回ISldWorks
    ''' </summary>
    ''' <param name="visible">实例可见性.</param>
    ''' <param name="version">版本(26 = sw 2018), 默认为-1 表示默认版本.</param>
    ''' <returns></returns>
    Public Shared Function CreateSolidWorks(visible As Boolean, Optional version As Integer = -1) As SldWorks
        Dim app As SldWorks

        If version = -1 Then
            app = Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"))
        Else
            app = Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application." + version.ToString()))
        End If

        If app IsNot Nothing Then app.Visible = visible
        Return app
    End Function

    ''' <summary>
    ''' 获得一个运行中的实例ISldWorks 并返回SolidWorks。
    ''' </summary>
    ''' <param name="version">指定版本号，-1表示默认</param>
    ''' <returns></returns>
    Public Shared Function GetSolidWorks(Optional version As Integer = -1) As SldWorks
        Dim app As SldWorks = Nothing
        Dim myGuid As Guid = Nothing

        If version = -1 Then
            myGuid = Type.GetTypeFromProgID("SldWorks.Application").GUID
        Else
            myGuid = Type.GetTypeFromProgID("SldWorks.Application." + version.ToString()).GUID
        End If

        GetActiveObject(myGuid, IntPtr.Zero, app)

        Return app
    End Function

    <DllImport("user32.dll")>
    Private Shared Function ShowWindow(ByVal hWnd As IntPtr, ByVal nCmdShow As Integer) As Boolean
    End Function

    ''' <summary>
    ''' 创建一个新的Solidworks并返回实例
    ''' </summary>
    ''' <param name="version">指定版本号，-1表示默认.</param>
    ''' <param name="suppressDialogs">True 则禁用solidworks弹出消息.</param>
    ''' <param name="requireMainWindow">True 表示运行完显示到主窗口</param>
    ''' <param name="startProcessTimeout">返回Null 如果SolidWorks在指定时间内没有打开。</param>
    ''' <param name="createWindowTimeout">返回Null 如果SolidWorks主窗口在指定时间内没有显示.</param>
    ''' <returns></returns>
    Public Shared Function RunSolidWorks(version As Integer,
                                         visible As Boolean,
                                         Optional suppressDialogs As Boolean = False,
                                         Optional requireMainWindow As Boolean = True,
                                         Optional startProcessTimeout As Integer = 30,
                                         Optional createWindowTimeout As Integer = 15) As SldWorks

        Dim executablePath As String = PFiles.GetSolidWorksExecutablePath(version)

        If File.Exists(executablePath) = False Then Return Nothing

        Dim info As ProcessStartInfo = New ProcessStartInfo(executablePath)

        If suppressDialogs Then info.Arguments = "/r"

        Dim process As Process = Process.Start(info)
        Dim app As SldWorks = Nothing
        Dim t As DateTime = DateTime.Now

        While app Is Nothing
            Threading.Thread.Sleep(1000)
            If Math.Abs(DateTime.Now.Subtract(t).Seconds) > startProcessTimeout Then Return Nothing

            'If it were possible to get a GUID from a process ID then we could use GetActiveObject instead of this
            app = GetComObjectFromProcessId(process.Id)
        End While

        t = DateTime.Now
        While IsRunning(isMainWindowCreated:=True) = False
            Threading.Thread.Sleep(1000)
            If Math.Abs(DateTime.Now.Subtract(t).Seconds) > createWindowTimeout Then Return Nothing
        End While

        If visible = False Then
            Dim frame As Frame = app.Frame()
            If frame Is Nothing Then Return app
            Dim handle As IntPtr = frame.GetHWndx64()
            If ShowWindow(handle, 0) Then Return app
        End If

        Return app
    End Function

    Private Shared Function GetComObjectFromProcessId(processId As Integer) As Object
        Dim runningObjectTable As IRunningObjectTable = Nothing
        Dim monikerEnumerator As IEnumMoniker = Nothing
        Dim ctx As IBindCtx = Nothing

        Try
            Dim numFetched As IntPtr = New IntPtr()
            CreateBindCtx(0, ctx)
            ctx.GetRunningObjectTable(runningObjectTable)
            runningObjectTable.EnumRunning(monikerEnumerator)
            monikerEnumerator.Reset()
            Dim monikers As IMoniker() = New IMoniker(0) {}

            While monikerEnumerator.[Next](1, monikers, numFetched) = 0
                Dim runningObjectName As String = Nothing
                monikers(0).GetDisplayName(ctx, Nothing, runningObjectName)

                If runningObjectName.IndexOf(processId.ToString()) <> -1 Then
                    Dim objReturnObject As Object = Nothing
                    runningObjectTable.GetObject(monikers(0), objReturnObject)
                    Return objReturnObject
                End If
            End While
        Finally
            If runningObjectTable IsNot Nothing Then Marshal.ReleaseComObject(runningObjectTable)
            If monikerEnumerator IsNot Nothing Then Marshal.ReleaseComObject(monikerEnumerator)
            If ctx IsNot Nothing Then Marshal.ReleaseComObject(ctx)
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' 获取安装的最新solidworks版本
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetLatestSolidWorksVersion() As Integer
        Dim years() As Integer = GetSolidWorksYears()

        If years.Length > 0 Then
            Return years(years.Length - 1) - 1992
        Else
            Return -1
        End If
    End Function

    Private Shared Function GetSolidWorksYears() As Integer()
        Dim versions As List(Of Integer) = New List(Of Integer)()
        Dim registry_key As String = "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"

        Using key As RegistryKey = Registry.LocalMachine.OpenSubKey(registry_key)

            For Each subkey_name As String In key.GetSubKeyNames()

                Using subkey As RegistryKey = key.OpenSubKey(subkey_name)
                    Dim displayName As String = TryCast(subkey.GetValue("DisplayName"), String)

                    If Not String.IsNullOrWhiteSpace(displayName) Then

                        If displayName.IndexOf("SOLIDWORKS", StringComparison.OrdinalIgnoreCase) > -1 Then
                            Dim pattern As String = "^SOLIDWORKS (\d{4})"
                            Dim regex = New Regex(pattern)

                            If regex.IsMatch(displayName) Then
                                Dim matches As MatchCollection = Regex.Matches(displayName, pattern)

                                For Each match As Match In matches
                                    Dim versionGroup As String = match.Groups(1).Value
                                    Dim version As Integer = Integer.Parse(versionGroup)
                                    If Not versions.Contains(version) Then versions.Add(version)
                                Next
                            End If
                        End If
                    End If
                End Using
            Next
        End Using

        versions.Sort()
        Return versions.ToArray()
    End Function

    ''' <summary>
    ''' 查看是否在Solidworks进程存在，可以选择关闭进程
    ''' </summary>
    ''' <param name="killAll"></param>
    ''' <param name="isMainWindowCreated">是否有主窗口进程.</param>
    ''' <returns></returns>
    Public Shared Function IsRunning(Optional killAll As Boolean = False,
                                     Optional isMainWindowCreated As Boolean = False) As Boolean
        Dim swProcess() As Process
        swProcess = Process.GetProcessesByName("SLDWORKS")

        If swProcess.Length = 0 Then Return False

        If killAll Then
            For Each myProcess As Process In swProcess
                myProcess.Kill()
            Next
        End If

        If isMainWindowCreated Then
            For Each myProcess In swProcess
                If myProcess.MainWindowHandle = IntPtr.Zero Then Return False
            Next
        End If

        Return True
    End Function

End Class