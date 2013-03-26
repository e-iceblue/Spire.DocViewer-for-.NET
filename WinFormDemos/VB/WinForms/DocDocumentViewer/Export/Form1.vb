﻿Imports System.ComponentModel
Imports System.Text

Namespace Export
	Partial Public Class Form1
		Inherits Form
		Public Shared bitMap As New Dictionary(Of String, Image)()
		Public Shared m_CurrentPageNum As Integer = 0
		Public Sub New()
			InitializeComponent()

			AddHandler docDocumentViewer1.PageNumberChanged, AddressOf docDocumentViewer1_PageNumberChanged
			AddHandler docDocumentViewer1.DocLoaded, AddressOf docDocumentViewer1_DocLoaded
			Me.textBox1.Enabled = ckbFrom.Checked
			Me.textBox2.Enabled = ckbTo.Checked
			Try
				' Load doc document from file.
				Me.docDocumentViewer1.LoadFromFile("..\..\..\..\..\..\Data\PartList.docx")
			Catch ex As Exception
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
			End Try
		End Sub

		Private Sub docDocumentViewer1_DocLoaded(ByVal sender As Object, ByVal args As EventArgs)
			Me.textBox2.Text = Me.docDocumentViewer1.PageCount.ToString()
		End Sub

		Private Sub docDocumentViewer1_PageNumberChanged(ByVal sender As Object, ByVal args As EventArgs)
			textBox1.Text = Me.docDocumentViewer1.CurrentPageNumber.ToString()
		End Sub


		Private Sub ckbHandtool_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ckbHandtool.CheckedChanged
			Me.docDocumentViewer1.EnableHandTools = ckbHandtool.Checked
		End Sub

		Private Sub btnOpen_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnOpen.Click
			'Open a Doc Document
			Dim dialog As New OpenFileDialog()
			dialog.Filter="Word97-2003 files(*.doc)|*.doc|Word2007-2010 files (*.docx)|*.docx|All files (*.*)|*.*"
			dialog.Title="Select a DOC file"
			dialog.Multiselect=False
			dialog.InitialDirectory=System.IO.Path.GetFullPath("..\..\..\..\..\..\Data")

			Dim result As DialogResult = dialog.ShowDialog()
			If result=DialogResult.OK Then
				Try
					' Load doc document from file.
					Me.docDocumentViewer1.LoadFromFile(dialog.FileName)
					Me.textBox2.Text = Me.docDocumentViewer1.PageCount.ToString()
				Catch ex As Exception
					MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
				End Try
			End If
		End Sub

		Private Sub btnSaveImage_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveImage.Click
			Me.Enabled = False
			bitMap.Clear()
			Try
				If ckbFrom.Checked AndAlso ckbTo.Checked Then
					Try
						Dim startIndex As Integer = 0
						Integer.TryParse(textBox1.Text, startIndex)
						m_CurrentPageNum = startIndex
						Dim endIndex As Integer = 0
						Integer.TryParse(textBox2.Text, endIndex)

						' Exports the specified pages as Images
						Dim bitmapsource() As Image = Me.docDocumentViewer1.SaveImage(CUShort(startIndex), CUShort(endIndex))
						SaveImageToFile(bitmapsource)
					Catch ex As Exception
						MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
					End Try
				ElseIf ckbFrom.Checked AndAlso (Not ckbTo.Checked) Then
					Try
						Dim currepageIndex As Integer = 0
						Integer.TryParse(textBox1.Text, currepageIndex)
						m_CurrentPageNum = currepageIndex
						'Saves the specified page as Image
						Dim bitmapsource As Image = Me.docDocumentViewer1.SaveImage(CUShort(currepageIndex))
						SaveImageToFile(New Image() { bitmapsource })
					Catch ex As Exception
						MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
					End Try
				End If
			Catch
			End Try
			ShowImage()
			Me.Enabled = True
		End Sub

		Private Sub SaveImageToFile(ByVal bitmpaSource() As Image)
			Dim startIndex As Integer = 1
			Integer.TryParse(textBox1.Text, startIndex)
			For Each bitmap As Bitmap In bitmpaSource
				WriteImageFile(bitmap, startIndex)
				startIndex += 1
			Next bitmap
		End Sub

		' <summary>
		' BitmapSource Write to File
		' </summary>
		' <param name="bitMapImg">bitmapSource </param>
		Private Sub WriteImageFile(ByVal bitMapImg As Image, ByVal currentPageIndex As Integer)
			Try
				If bitMapImg IsNot Nothing Then
					Dim [date] As String = Date.Now.ToShortDateString().Replace("-", "").Replace("/", "").Replace(".", "")
					Dim path As String = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)
					Dim dirPathFullname As String = path & "\" & [date] & "\"
					Dim filename As String = System.IO.Path.GetFileNameWithoutExtension(Me.docDocumentViewer1.FileName)
					Dim FullfileName As String = dirPathFullname & filename & "_" & currentPageIndex.ToString() & ".png"

					If Not System.IO.Directory.Exists(dirPathFullname) Then
						System.IO.Directory.CreateDirectory(dirPathFullname)
					End If

					bitMapImg.Save(FullfileName, System.Drawing.Imaging.ImageFormat.Png)
					bitMap.Add(FullfileName, bitMapImg)
				End If
			Catch ex As Exception
#If DEBUG Then
				Debug.WriteLine(ex.Message + ex.Source)
#End If
			End Try
		End Sub

		' <summary>
		' Show ImageViewer Window
		' </summary>
		Private Sub ShowImage()
			Dim count As Integer = bitMap.Count
			If count > 0 Then
				Dim iv As New ImageViewer()
				iv.ShowDialog()
			End If
		End Sub

		Private Sub ckbTo_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ckbTo.CheckedChanged
			Me.textBox2.Enabled = ckbTo.Checked
		End Sub
	End Class
End Namespace
