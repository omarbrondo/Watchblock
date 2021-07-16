Imports System.Data.SqlClient
Module ConectorBase
    Public Function CreateConnection() As SqlConnection

        Dim Connection As SqlConnection = Nothing

        Try
            Dim connString As New SqlConnectionStringBuilder(My.Settings.connString)

            Connection = New SqlConnection(connString.ConnectionString)

        Catch ex As Exception

            Throw ex

        End Try

        Return Connection

    End Function
End Module
