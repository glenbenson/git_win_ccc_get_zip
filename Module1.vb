Module Module1
    Public downloadpath As String           ' c:\adp\emsin usually
    Public backuppath As String             'not used now
    Public acctno As String
    Public password As String = "dakabi04"   'will be changed if new one passed
    Public havefiles As Boolean
    Public claim_no As String = "no_claimnoyet"
    Public pause As Boolean
    Public proc_files As Boolean
    Public ext_url As String
    Public ini_file As String
    Public app_path As String
    Public ini As New IniFile
    Public Property ZipFile As Object
    Public finalfilename As String
    Public row As String = "0"
    Public attempt As Integer = 0 ' the number of times we tried to get the zip file increments each attempt
    Public googleurl As String = "https://script.googleusercontent.com/a/macros/glenbenson.com/echo?user_content_key=aHmhrybKjeYhzcY7qoSTL-od3bvkwfvx2N5cbQz7fuJ7Y9w6mczGc1oD_rIpQVX2dS0gWZce7PHf5bCwDfView99k368CfAhOJmA1Yb3SEsKFZqtv3DaNYcMrmhZHmUMi80zadyHLKBWzKK6x87M--lQOxpx2pnRaAD8aA7uc1cBA8YfTzkNYDtYOUvC1hKV-TvxC0ShI4VXZ9sG0vQnp98eJdngHrWWeZG34mEGlb_y_luWKK3Al2o_IUxvACgdcLcNFEl34Qw8yZdRdAkpGA&lib=MdrL6d1vnC436fliCtbLmCna8n_ITth9O"
    Public testmode As Boolean = False

    Public Sub writetolog(strText As String)
        Dim line As String
        Dim strDate = DateTime.Now.ToString("MM-dd-yyyy hh:mm tt")
        line = strDate & " | " & acctno & " | " & claim_no & " | " & strText
        Dim objWriter As New System.IO.StreamWriter("ccc_ems_download.log", True)
        objWriter.WriteLine(line)
        objWriter.Close()
    End Sub
End Module
