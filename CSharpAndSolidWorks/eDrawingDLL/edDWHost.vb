Imports EModelView

Public Class edDWHost
    Private ocx As EModelViewControl

    Protected Overrides Sub AttachInterfaces()
        MyBase.AttachInterfaces()
        ocx = MyBase.GetOcx()
        ocx.EnableFeatures = 16

    End Sub

End Class