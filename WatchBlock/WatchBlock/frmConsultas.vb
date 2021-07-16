Imports System.Data.SqlClient
Public Class frmConsultas

    Private Database1DataSet As DataSet


    Private Sub btnConsultar_Click(sender As Object, e As EventArgs) Handles btnConsultar.Click
        Using conexion = ConectorBase.CreateConnection

            conexion.Open()

            Dim ConsultaTableAdapter As New SqlDataAdapter

            ConsultaTableAdapter = New SqlDataAdapter With {
                .SelectCommand = New SqlCommand("select * from dbo.WS_SISTEMAS_CALIPSO_LOG", conexion)
            }

            Database1DataSet = New DataSet

            Database1DataSet.Tables.Add("ConsultaCalipso")

            ConsultaTableAdapter.Fill(Database1DataSet.Tables("ConsultaCalipso"))

            dgvConsultas.DataSource = Database1DataSet.Tables("ConsultaCalipso")

            conexion.Close()

        End Using
    End Sub
End Class