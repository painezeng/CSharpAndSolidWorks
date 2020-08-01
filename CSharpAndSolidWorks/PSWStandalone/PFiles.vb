Imports System.IO
Imports Microsoft.Win32
Imports SolidWorks.Interop.swconst

Public Class PFiles
    Public Const Extension_Part As String = ".sldprt"
    Public Const Extension_Assembly As String = ".sldasm"
    Public Const Extension_Drawing As String = ".slddrw"

    ''' <summary>
    ''' Bitmask.
    ''' </summary>
    <Flags>
    Public Enum CTDocTypes
        Part = 1
        Assembly = 2
        Drawing = 4
        All = 7
    End Enum

    ''' <summary>
    ''' Attempts to create a folder. Returns false and an exception if creation fails.
    ''' </summary>
    Public Shared Function CreateFolder(path As String, ByRef ex As Exception) As Boolean
        Try
            Directory.CreateDirectory(path)
            Return True
        Catch except As Exception
            ex = except
            Return False
        End Try
    End Function

    Public Shared Function CopyFolder(sourceDir As String, destinationDir As String, ByRef ex As Exception) As Boolean
        Try
            For Each dirPath As String In Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories)
                Dim newDir As String = dirPath.Replace(sourceDir, destinationDir)
                If Directory.Exists(newDir) = False Then
                    Directory.CreateDirectory(newDir)
                End If
            Next

            For Each filePath As String In Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories)
                Dim newPath As String = filePath.Replace(sourceDir, destinationDir)
                File.Copy(filePath, newPath, True)
            Next

            ex = Nothing
            Return True
        Catch except As Exception
            ex = except
            Return False
        End Try
    End Function

    Public Shared Function DeleteFile(path As String, ByRef ex As Exception) As Boolean
        Try
            If File.Exists(path) = False Then Return False
            File.Delete(path)
            Return True
        Catch except As Exception
            ex = except
            Return False
        End Try
    End Function

    Public Shared Function DeleteFolder(path As String, ByRef ex As Exception) As Boolean
        Try
            Directory.Delete(path, True)
            Return True
        Catch except As Exception
            ex = except
            Return False
        End Try
    End Function

    Public Shared Function RenameFile(oldFilePath As String, newFileName As String, ByRef ex As Exception) As Boolean
        Const tempFileNamePrefix = "_TEMP_"

        Try
            Dim dirName As String = Path.GetDirectoryName(oldFilePath)
            Dim tempFilePath As String = Path.Combine(dirName, tempFileNamePrefix + Path.GetFileName(oldFilePath))
            Dim newFilePath As String = Path.Combine(dirName, newFileName)
            File.Move(oldFilePath, tempFilePath)
            File.Move(tempFilePath, newFilePath)
            Return True
        Catch except As Exception
            ex = except
            Return False
        End Try
    End Function

    Public Shared Function IsFileReadOnly(file As String) As Boolean
        Dim myFileInfo As FileInfo = New FileInfo(file)
        Return myFileInfo.IsReadOnly
    End Function

    ''' <summary>
    ''' Verifies that a file has a specified extension.
    ''' </summary>
    ''' <param name="filePath"></param>
    ''' <param name="extension"></param>
    ''' <returns></returns>
    Public Shared Function VerifyExtension(filePath As String, extension As String) As Boolean
        If Path.GetExtension(filePath).ToUpper() = extension.ToUpper() Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function IsValidFileName(text As String) As Boolean
        For Each c As Char In Path.GetInvalidFileNameChars()
            If text.Contains(c.ToString()) Then
                Return False
            End If
        Next

        Return True
    End Function

    Public Shared Function ReplaceIllegalChars(text As String, replaceChar As String) As String
        Dim illegalChars As String() = {Chr(34), "<", ">", "/", "\", ":", "*", "?", "|", ControlChars.Quote}

        For Each illegalChar As String In illegalChars
            If text.Contains(illegalChar) Then text = text.Replace(illegalChar, replaceChar)
        Next

        Return text
    End Function

    Public Shared Function ReadTextFile(textFilePath As String) As List(Of String)

        Try
            Dim myList As New List(Of String)

            Using reader As New StreamReader(textFilePath)
                Do While reader.Peek <> -1
                    myList.Add(reader.ReadLine)
                Loop
                reader.Close()
            End Using

            Return myList
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    ''' <summary>
    ''' Returns a list of file paths of SolidWorks files in a directory, not including temporary files.
    ''' </summary>
    ''' <param name="folderPath"></param>
    ''' <param name="docTypes">Bitmask.</param>
    ''' <param name="includeSubFolders"></param>
    ''' <returns></returns>
    Public Shared Function GetSolidWorksFilePaths(folderPath As String,
                                       Optional docTypes As CTDocTypes = CTDocTypes.All,
                                       Optional includeSubFolders As Boolean = False) As List(Of String)

        Dim searchOption As SearchOption
        If includeSubFolders Then
            searchOption = SearchOption.AllDirectories
        Else
            searchOption = SearchOption.TopDirectoryOnly
        End If

        Dim fileList As New List(Of String)

        If docTypes And CTDocTypes.Part Then fileList.AddRange(Directory.GetFiles(folderPath, "*" + Extension_Part, searchOption))
        If docTypes And CTDocTypes.Assembly Then fileList.AddRange(Directory.GetFiles(folderPath, "*" + Extension_Assembly, searchOption))
        If docTypes And CTDocTypes.Drawing Then fileList.AddRange(Directory.GetFiles(folderPath, "*" + Extension_Drawing, searchOption))

        For i As Integer = fileList.Count - 1 To 0 Step -1
            If fileList(i).IndexOf("~$") <> -1 Then fileList.RemoveAt(i)
        Next

        Return fileList
    End Function

    ''' <summary>
    ''' Examines the extension of a file path to determine if the file is a SolidWorks part, assembly, or drawing. Returns -1 if file is not a SolidWorks model.
    ''' </summary>
    ''' <param name="filePath"></param>
    ''' <returns></returns>
    Public Shared Function GetSolidWorksDocType(filePath As String) As swDocumentTypes_e

        If filePath Is Nothing Then Return -1

        Select Case Path.GetExtension(filePath).ToLower()
            Case Extension_Part
                Return swDocumentTypes_e.swDocPART
            Case Extension_Assembly
                Return swDocumentTypes_e.swDocASSEMBLY
            Case Extension_Drawing
                Return swDocumentTypes_e.swDocDRAWING
            Case Else
                Return -1
        End Select
    End Function

    ''' <param name="version">Major version of SolidWorks, or -1 for the newest version.</param>
    ''' <returns></returns>
    Public Shared Function GetSolidWorksExecutablePath(version As Integer) As String
        Using hklm As RegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)

            If version = -1 Then
                Using swKeys As RegistryKey = hklm.OpenSubKey("SOFTWARE\SolidWorks")
                    Dim keys As String() = swKeys.GetSubKeyNames()
                    For Each key As String In keys
                        If key.IndexOf("SOLIDWORKS ") <> -1 Then
                            Dim split As String() = key.Split(" ")
                            Dim retVal As Integer
                            If Integer.TryParse(split(1), retVal) Then version = PSolidWorks.ConvertYearToMajorVersion(retVal)
                        End If
                    Next
                End Using
            End If

            Using key As RegistryKey = hklm.OpenSubKey("SOFTWARE\SolidWorks\SOLIDWORKS " +
                                                       PSolidWorks.ConvertMajorVersionToYear(version).ToString() + "\Setup")
                If key Is Nothing Then
                    Return Nothing
                Else
                    Return key.GetValue("SolidWorks Folder") + "SLDWORKS.EXE"
                End If
            End Using
        End Using
    End Function

End Class