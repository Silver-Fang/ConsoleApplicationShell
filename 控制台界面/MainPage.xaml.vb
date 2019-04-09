' https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板
Imports ProcessForUWP.UWP端
''' <summary>
''' 可用于自身或导航至 Frame 内部的空白页。
''' </summary>
Public NotInheritable Class MainPage
	Inherits Page
	WithEvents 进程 As Process
	Private Async Sub MainPage_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
		Dim a As New Windows.Storage.Pickers.FileOpenPicker, b As Windows.Storage.StorageFile
		a.FileTypeFilter.Add(".exe")
		Dim c As Task(Of Process) = Task.Run(Function() As Process
												 Return New Process()
											 End Function)
		Do
			b = Await a.PickSingleFileAsync
		Loop While b Is Nothing
		进程 = Await c
		With 进程.StartInfo
			.FileName = b.Path
			.CreateNoWindow = True
			.RedirectStandardError = True
			.RedirectStandardInput = True
			.RedirectStandardOutput = True
			.UseShellExecute = False
		End With
		With 进程
			.Start()
			.BeginErrorReadLine()
			.BeginOutputReadLine()
		End With
	End Sub
	Private Sub 输出(输出内容 As String)
		Call Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
																					Dim a As New Documents.Paragraph
																					a.Inlines.Add(New Documents.Run With {.Text = 输出内容})
																					With 输出框
																						.Blocks.Add(a)
																						.Select(.ContentEnd, .ContentEnd)
																						滚动条.ChangeView(0, Double.MaxValue, 1)
																					End With
																				End Sub)
	End Sub

	Private Sub 进程_DataReceived(sender As Object, e As DataReceivedEventArgs) Handles 进程.ErrorDataReceived, 进程.OutputDataReceived
		输出(e.Data)
	End Sub

	Private Sub 输入框_KeyUp(sender As Object, e As KeyRoutedEventArgs) Handles 输入框.KeyUp
		If e.Key = Windows.System.VirtualKey.Enter Then
			输出(输入框.Text)
			If 进程 Is Nothing Then
				输出("输入无效，因为尚未连接到远程Process代理，请稍后……")
			Else
				进程.StandardInput.Write(输入框.Text)
			End If
			输入框.Text = ""
		End If
	End Sub
End Class
