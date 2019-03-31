Imports PdfSharp
Imports PdfSharp.Drawing
Imports PdfSharp.Pdf
Imports System.IO

Public Class Frm_Main
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetSettings(Me)

        ComboBox1.SelectedIndex = ComboBox1.Items.Count - 1
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim size As Double = CDbl(ComboBox1.SelectedItem.ToString)

        Dim i As Threading.Thread = New Threading.Thread(Sub() PrintPage(TextBox1.Text, TextBox2.Text, TextBox3.Text, TextBox4.Text, size))
        i.Start()
    End Sub


    Private Function PrintPage(ByVal Name As String, ByVal AdressZeile1 As String, ByVal AdressZeile2 As String, ByVal AdressZeile3 As String, ByVal Size As Double)
        Dim pdf As PdfDocument = New PdfDocument
        Dim pdfPage As PdfPage = pdf.AddPage
        Dim graph As XGraphics = XGraphics.FromPdfPage(pdfPage)
        Dim fontEmpfaenger As XFont = New XFont("GeForce", 12, XFontStyle.Bold)
        Dim fontAdresse As XFont = New XFont("GeForce", Size, XFontStyle.Bold)

        pdfPage.Width = 283.28
        pdfPage.Height = 175.52

        graph.DrawString("Empfänger:", fontEmpfaenger, XBrushes.Black, New XRect(10, 5, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft)

        graph.DrawString(Name, fontAdresse, XBrushes.Black, New XRect(20, 40, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft)
        graph.DrawString(AdressZeile1, fontAdresse, XBrushes.Black, New XRect(15, 70, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft)
        graph.DrawString(AdressZeile2, fontAdresse, XBrushes.Black, New XRect(15, 100, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft)
        graph.DrawString(AdressZeile3, fontAdresse, XBrushes.Black, New XRect(15, 130, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft)

        Randomize()
        Dim rndNumber As Short = (1024 * Rnd() + 0)

        Dim pdfFilename As String = "tmpdoc_" & Str(rndNumber) & ".pdf"
        Dim pdfFilepath As String = Path.Combine(Directory.GetCurrentDirectory, pdfFilename)
        pdf.Save(pdfFilename)
        pdf.Close()

        SendToPrinter(pdfFilepath)

        Try
            File.Delete(pdfFilepath)
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try

        graph.Dispose()
        pdfPage.Close()
        pdf.Dispose()
        Return Nothing
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim clipboard As String = My.Computer.Clipboard.GetText

        If Not clipboard.Length >= 3 Then
            Exit Sub
        End If

        ' Clean of all unwanted chars
        Dim clipSplit As String() = clipboard.Split(vbCr)
        Dim clipSplitCleaned As ArrayList = New ArrayList

        For i = 0 To clipSplit.Length - 1
            Dim acc As String = Nothing
            For Each o As Char In clipSplit(i)
                If Char.IsLetterOrDigit(o) Or o = " " Or o = "." Or o = "-" Or o = "_" Or o = "/" Or o = "\" Then
                    acc += o
                End If
            Next
            clipSplitCleaned.Add(acc)
        Next

        'PLZ Whitespace
        For i = 0 To clipSplitCleaned.Count - 1
            If IsNumeric(clipSplitCleaned(i).Substring(0, 1)) Then
                clipSplitCleaned(i) = clipSplitCleaned(i).ToString.Insert(5, " ")
            End If
        Next

        Select Case clipSplitCleaned.Count
            Case 3
                Dim count As Short = 0
                For Each item As String In clipSplitCleaned
                    If count = 0 Then
                        TextBox1.Text = item
                    ElseIf count = 1 Then
                        TextBox3.Text = item
                    ElseIf count = 2 Then
                        TextBox4.Text = item
                    End If
                    count += 1
                Next
            Case 4
                Dim count As Short = 0
                For Each item As String In clipSplitCleaned
                    If count = 0 Then
                        TextBox1.Text = item
                    ElseIf count = 1 Then
                        TextBox2.Text = item
                    ElseIf count = 2 Then
                        TextBox3.Text = item
                    ElseIf count = 3 Then
                        TextBox4.Text = item
                    End If
                    count += 1
                Next
        End Select
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim pdf As PdfDocument = New PdfDocument
        Dim pdfPage As PdfPage = pdf.AddPage
        Dim graph As XGraphics = XGraphics.FromPdfPage(pdfPage)
        Dim fontEmpfaenger As XFont = New XFont("GeForce", 12, XFontStyle.Bold)
        Dim fontAdresse As XFont = New XFont("GeForce", 28, XFontStyle.Bold)

        pdfPage.Width = 283.28
        pdfPage.Height = 175.52


        If Not My.Computer.Clipboard.ContainsImage Then
            Debug.Print("No Image in Clipboard")
            Exit Sub
        End If

        Dim bm_source As Bitmap = My.Computer.Clipboard.GetImage
        Dim bm_dest As New Bitmap(350, 210)
        Dim gr_dest As Graphics = Graphics.FromImage(bm_dest)
        gr_dest.DrawImage(bm_source, 0, 0, bm_dest.Width, bm_dest.Height)

        Dim pStream As MemoryStream = New MemoryStream
        bm_dest.Save(pStream, Imaging.ImageFormat.Png)

        Dim p = XImage.FromStream(pStream)

        graph.DrawImage(p, New XPoint(10, 10))


        Randomize()
        Dim rndNumber As Short = (1024 * Rnd() + 0)

        Dim pdfFilename As String = "tmpdoc_" & Str(rndNumber) & ".pdf"
        Dim pdfFilepath As String = Path.Combine(Directory.GetCurrentDirectory, pdfFilename)
        pdf.Save(pdfFilename)
        pdf.Close()


        SendToPrinter(pdfFilepath)

        Try
            File.Delete(pdfFilepath)
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try



        graph.Dispose()
        pdfPage.Close()
        pdf.Dispose()


    End Sub
End Class
