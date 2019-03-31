Module Mod_Printing
    Public Function SendToPrinter(ByVal pdfFilepath As String)
        Dim i As Process = New Process
        Dim iInfo As ProcessStartInfo = New ProcessStartInfo

        With iInfo
            .FileName = "C:\Program Files (x86)\Adobe\Acrobat DC\Acrobat\Acrobat.exe"
            .Arguments = "/t """ & pdfFilepath & """ ""Brother QL-800"""
            .CreateNoWindow = True
        End With

        i.StartInfo = iInfo
        i.Start()

        Threading.Thread.Sleep(5000)
        Return Nothing
    End Function
End Module
