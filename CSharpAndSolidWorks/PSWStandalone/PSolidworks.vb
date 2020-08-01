Imports System.Runtime.InteropServices
Imports SolidWorks.Interop.sldworks
Imports SolidWorks.Interop.swconst

Public Class PSolidWorks

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

    Public Shared Sub ExitSolidWorks(app As SldWorks)
        app.ExitApp()
        While True
            If Marshal.ReleaseComObject(app) = 0 Then Exit While
        End While
        app = Nothing
        GC.Collect()
        GC.WaitForPendingFinalizers()
    End Sub

    ''' <summary>
    ''' Creates a new document.
    ''' </summary>
    ''' <param name="app"></param>
    ''' <param name="docType">Document type as defined in swDocumentTypes_e.</param>
    ''' <param name="templatePath">If nothing then default template is used.</param>
    ''' <returns>Pointer to IModelDoc2 or null.</returns>
    Public Shared Function CreateModel(app As SldWorks, docType As Integer, Optional templatePath As String = Nothing) As ModelDoc2
        If templatePath = Nothing Then
            Select Case docType
                Case swDocumentTypes_e.swDocPART
                    templatePath = app.GetUserPreferenceStringValue(swUserPreferenceStringValue_e.swDefaultTemplatePart)
                Case swDocumentTypes_e.swDocASSEMBLY
                    templatePath = app.GetUserPreferenceStringValue(swUserPreferenceStringValue_e.swDefaultTemplateAssembly)
                Case swDocumentTypes_e.swDocDRAWING
                    templatePath = app.GetUserPreferenceStringValue(swUserPreferenceStringValue_e.swDefaultTemplateDrawing)
            End Select
        End If

        Return app.NewDocument(templatePath, Nothing, Nothing, Nothing)
    End Function

    Public Shared Function OpenModel(app As SldWorks,
                                                file As String,
                                                visible As Boolean,
                                                activate As Boolean,
                                                ByRef openErr As Integer,
                                                ByRef openWarn As Integer,
                                                ByRef activateErr As Integer) As ModelDoc2

        Dim docType As swDocumentTypes_e = PFiles.GetSolidWorksDocType(file)

        If visible = False Then app.DocumentVisible(False, docType)

        Dim model As ModelDoc2 = app.OpenDoc6(file,
                                              docType,
                                              swOpenDocOptions_e.swOpenDocOptions_Silent,
                                              Nothing,
                                              openErr,
                                              openWarn)

        If model Is Nothing Then
            If visible = False Then app.DocumentVisible(True, docType)
            Return Nothing
        End If

        'This will be Nothing if the document was opened invisibly.
        Dim activeDoc As ModelDoc2 = app.ActiveDoc

        If activate AndAlso app.IsSame(activeDoc, model) = swObjectEquality.swObjectNotSame Then
            model = app.ActivateDoc3(file,
                         False,
                         swRebuildOnActivation_e.swRebuildActiveDoc,
                         activateErr)
        End If

        If visible = False Then app.DocumentVisible(True, docType)

        Return model
    End Function

    ''' <summary>
    ''' Gets the active document as long as it is the specified type.
    ''' </summary>
    ''' <param name="app"></param>
    ''' <param name="allowedDocTypes">Allowed document types. Bitmasked.</param>
    ''' <returns></returns>
    Public Shared Function GetActiveDocument(app As SldWorks, allowedDocTypes As CTDocTypes) As ModelDoc2

        Dim model As ModelDoc2 = app.ActiveDoc

        If model Is Nothing Then Return Nothing

        Select Case model.GetType()
            Case swDocumentTypes_e.swDocPART
                If allowedDocTypes And CTDocTypes.Part Then Return model
            Case swDocumentTypes_e.swDocASSEMBLY
                If allowedDocTypes And CTDocTypes.Assembly Then Return model
            Case swDocumentTypes_e.swDocDRAWING
                If allowedDocTypes And CTDocTypes.Drawing Then Return model
        End Select

        Return Nothing

    End Function

    ''' <summary>
    ''' Closes all documents of a particular document type.
    ''' </summary>
    ''' <param name="app"></param>
    ''' <param name="docTypes">Bitmasked.</param>
    Public Shared Sub CloseDocuments(app As SldWorks, Optional docTypes As PFiles.CTDocTypes = PFiles.CTDocTypes.All)
        If docTypes = PFiles.CTDocTypes.All Then
            app.CloseAllDocuments(True)
            Return
        End If

        Dim openModels() As Object = app.GetDocuments()
        If openModels Is Nothing Then Return
        For Each model As ModelDoc2 In openModels
            If docTypes And PFiles.CTDocTypes.Part Then app.CloseDoc(model.GetPathName())
            If docTypes And PFiles.CTDocTypes.Assembly Then app.CloseDoc(model.GetPathName())
            If docTypes And PFiles.CTDocTypes.Drawing Then app.CloseDoc(model.GetPathName())
        Next
    End Sub

    ''' <summary>
    ''' Gets the major version of this instance of SolidWorks.
    ''' </summary>
    ''' <param name="app"></param>
    ''' <param name="displayAsYear">Whether to return the major version as a year.</param>
    ''' <returns></returns>
    Public Shared Function GetMajorVersion(app As SldWorks, displayAsYear As Boolean) As Integer
        Dim majorVersion As Integer = Convert.ToInt16(app.RevisionNumber().Split(".")(0))
        If displayAsYear Then
            Return ConvertMajorVersionToYear(majorVersion)
        Else
            Return majorVersion
        End If
    End Function

    Public Shared Function ConvertYearToMajorVersion(year As Integer) As Integer
        Return year - 1992
    End Function

    Public Shared Function ConvertMajorVersionToYear(version As Integer) As Integer
        Return version + 1992
    End Function

End Class