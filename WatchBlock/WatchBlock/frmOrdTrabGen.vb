Imports System.IO
Imports System.Data.SqlClient


Public Class frmOrdTrabGen

    Private Database1DataSet As DataSet

    Private Sub MetroButton1_Click(sender As Object, e As EventArgs) Handles btnConsultar.Click
        Using conexion = ConectorBase.CreateConnection

            conexion.Open()

            Dim ConsultaTableAdapter As New SqlDataAdapter



            ConsultaTableAdapter = New SqlDataAdapter With {
                .SelectCommand = New SqlCommand("SELECT estado, fecha, idUser, producto 
                                                 FROM WS_NUEVO_CLON_OT", conexion)
            }




            Database1DataSet = New DataSet


            Database1DataSet.Tables.Add("ConsultaCalipso")



            ConsultaTableAdapter.Fill(Database1DataSet.Tables("ConsultaCalipso"))

            dgv1.DataSource = Database1DataSet.Tables("ConsultaCalipso")


            conexion.Close()

        End Using

        Using conexion = ConectorBase.CreateConnection

            conexion.Open()

            Dim ConsultaTableAdapter As New SqlDataAdapter



            ConsultaTableAdapter = New SqlDataAdapter With {
                .SelectCommand = New SqlCommand("SELECT estado, fecha, idUser, nombreET 
                                                 FROM WS_NUEVA_ESPECIFICACION_TECNICA", conexion)
            }




            Database1DataSet = New DataSet


            Database1DataSet.Tables.Add("ConsultaCalipso")



            ConsultaTableAdapter.Fill(Database1DataSet.Tables("ConsultaCalipso"))

            dgv2.DataSource = Database1DataSet.Tables("ConsultaCalipso")


            conexion.Close()

        End Using




    End Sub

    Private Sub MetroButton2_Click(sender As Object, e As EventArgs) Handles btnVolverAlFormularioPpal.Click
        frmPrincipal.Show()
        Me.Close()
    End Sub

    Private Sub frmOrdTrabGen_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class