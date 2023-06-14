using System;
using System.Windows.Forms;

namespace LoginFormExample
{
    public partial class LoginForm : Form
    {
        private LoginService loginService;

        public LoginForm()
        {
            InitializeComponent();
            loginService = new LoginService();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (loginService.Login(username, password))
            {
                // Очистить текстовые поля
                txtUsername.Text = string.Empty;
                txtPassword.Text = string.Empty;

                // Создать и отобразить новую форму
                EmptyForm emptyForm = new EmptyForm();
                emptyForm.Show();

                // Закрыть текущую форму входа
                this.Hide();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка входа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
