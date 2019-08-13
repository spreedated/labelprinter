Public Class PrintRaw
    Private Shared rText As String() = Nothing
    Private Shared WithEvents PrintDoc As Printing.PrintDocument
    Private Shared rTextSize As Short = 24

    Public Shared Function Print(ByVal receiptText() As String, ByVal printerName As String, ByVal pageWidth As Short, ByVal pageHeight As Short, ByVal textSize As Short)
        rText = receiptText
        rTextSize = textSize
        Try
            Dim psz As New Printing.PaperSize
            psz.RawKind = Printing.PaperKind.Custom
            psz.Width = MMInPixel(pageWidth)
            psz.Height = MMInPixel(pageHeight)

            PrintDoc = New Printing.PrintDocument
            Using PrintDoc
                With PrintDoc
                    .DefaultPageSettings.PaperSize = psz
                    .PrinterSettings.PrinterName = printerName
                    .DefaultPageSettings.Landscape = True
                    .Print()
                End With
            End Using
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Private Shared Sub PrintPageHandler(ByVal sender As Object, ByVal e As Printing.PrintPageEventArgs) Handles PrintDoc.PrintPage
        Dim myFontHead As Font = New Font("GeForce", 12)
        Dim myFont As Font = New Font("GeForce", rTextSize)
        With e.Graphics
            .SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
            .DrawString("Empfänger:", New Font(myFontHead, FontStyle.Bold), Brushes.Black, 0, 0)
            .DrawString(rText(0), New Font(myFont, FontStyle.Bold), Brushes.Black, 20, 30)
            .DrawString(rText(1), New Font(myFont, FontStyle.Bold), Brushes.Black, 20, 90)
            .DrawString(rText(2), New Font(myFont, FontStyle.Bold), Brushes.Black, 20, 130)
            .DrawString(rText(3), New Font(myFont, FontStyle.Bold), Brushes.Black, 20, 170)
        End With
    End Sub

    Private Shared inchInMM As Double = 25.4 'mm = 1 inch
    Private Shared dpi As Integer = 96

    Private Shared Function MMInPixel(value As Integer) As Integer
        Return Math.Round((value / inchInMM) * dpi, MidpointRounding.AwayFromZero)
    End Function
End Class