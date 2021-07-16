Public Class frmLoginScreen

    ' TODO: inserte el c�digo para realizar autenticaci�n personalizada usando el nombre de usuario y la contrase�a proporcionada 
    ' (Consulte https://go.microsoft.com/fwlink/?LinkId=35339).  
    ' El objeto principal personalizado se puede adjuntar al objeto principal del subproceso actual como se indica a continuaci�n: 
    '     My.User.CurrentPrincipal = CustomPrincipal
    ' donde CustomPrincipal es la implementaci�n de IPrincipal utilizada para realizar la autenticaci�n. 
    ' Posteriormente, My.User devolver� la informaci�n de identidad encapsulada en el objeto CustomPrincipal
    ' como el nombre de usuario, nombre para mostrar, etc.

    Private Sub OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK.Click

        If UsernameTextBox.Text <> Nothing And PasswordTextBox.Text <> Nothing Then

            Sesion.Usuario = UsernameTextBox.Text

            Sesion.Password = PasswordTextBox.Text

            frmPrincipal.Show()

            Me.Close()

        Else

            If UsernameTextBox.Text = String.Empty Then

                MessageBox.Show("INGRESE UN USUARIO", "ATENCION", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)


                UsernameTextBox.Focus()

            Else

                If PasswordTextBox.Text = String.Empty Then


                    MessageBox.Show("INGRESE SU PASSWORD", "ATENCION", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

                    PasswordTextBox.Focus()

                End If

            End If

        End If

    End Sub

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Me.Close()
    End Sub

    Private Sub frmLoginScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        lblVersionSistema.Text = My.Application.Info.Version.ToString

    End Sub
End Class
