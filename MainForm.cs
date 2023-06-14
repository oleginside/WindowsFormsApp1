using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.SQLite;

namespace SimpleWindowsForm
{
    public class MainForm : Form
    {
        //private DatabaseManager databaseManager;
        private SQLiteConnection connection;
        //private SQLiteConnection connection;
        private Button loginButton;
        private TextBox usernameTextBox;
        private TextBox passwordTextBox;
        //private ListBox itemListBox;
        private TextBox newTaskTextBox;
        private DataGridView taskDataGridView;
        //private DataGridView taskGridView;
        private Button addTaskButton;
        //private Button deleteEmployeeButton;
        private Button addEmployeeButton;
        private Button deleteEmployeeButton;
        private Button deleteTaskButton;
        private ComboBox urgencyComboBox;
        private ComboBox UserComboBox;
        private DateTimePicker dueDateTimePicker;
        private DataGridView employeeDataGridView;
        private TextBox newEmployeeLoginTextBox;
        private TextBox newEmployeePasswordTextBox;
        private TextBox newEmployeeNameTextBox;
        private TextBox newEmployeePositionTextBox;

        private Dictionary<string, string> whitelist = new Dictionary<string, string>() // Белый список пользователей и паролей
        {
            { "123", "123"}
        };

        private void UpdateEmployeesTable()
        {
            string connectionString = "Data Source = Tasks.db; Version = 3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Получение информации о столбцах таблицы
                string sqlColumnsQuery = "PRAGMA table_info(Employees)";
                using (SQLiteCommand columnsCommand = new SQLiteCommand(sqlColumnsQuery, connection))
                {
                    using (SQLiteDataReader columnsReader = columnsCommand.ExecuteReader())
                    {
                        // Цикл по каждому столбцу таблицы
                        while (columnsReader.Read())
                        {
                            string columnName = columnsReader.GetString(1);
                            string columnType = columnsReader.GetString(2);

                            // Проверка, является ли столбец NULL-совместимым
                            if (!columnType.Contains("NOT NULL"))
                            {
                                // Создание SQL-запроса для проверки и замены NULL значений в столбце
                                string sqlQuery = $"UPDATE Employees SET {columnName} = COALESCE({columnName}, 'Null') WHERE {columnName} IS NULL";
                                using (SQLiteCommand command = new SQLiteCommand(sqlQuery, connection))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
        }// Функция проверки и замены NULL на "Null"


        private void InitializeDatabase() // Вставка значений из Whitelist в базу данных Employee
        {
            string connectionString = "Data Source=Tasks.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Создание таблицы Employees (если она не существует)
                /*                string createTableQuery = "CREATE TABLE IF NOT EXISTS Employees (Id INTEGER PRIMARY KEY AUTOINCREMENT, Login TEXT, Password TEXT, Name TEXT, Position TEXT)";
                                using (SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection))
                                {
                                    createTableCommand.ExecuteNonQuery();
                                }*/

                // Вставка записей из словаря whitelist
                foreach (KeyValuePair<string, string> entry in whitelist)
                {
                    string login = entry.Key;
                    string password = entry.Value;
                    string name = "Boss";
                    string position = "Boss"; // Значение для поля Position

                    // Проверка наличия записи с такими значениями
                    string checkQuery = "SELECT COUNT(*) FROM Employees WHERE Login = @Login AND Password = @Password";
                    using (SQLiteCommand checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Login", login);
                        checkCommand.Parameters.AddWithValue("@Password", password);
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (count == 0)
                        {
                            // Записи не существует, выполняем вставку
                            string insertQuery = "INSERT INTO Employees (Login, Password, Name, Position) VALUES (@Login, @Password, @Name, @Position)";
                            using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@Login", login);
                                insertCommand.Parameters.AddWithValue("@Password", password);
                                insertCommand.Parameters.AddWithValue("@Position", position);
                                insertCommand.Parameters.AddWithValue("@Name", name);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        // Счетчик для нумерации задач
        private int taskCounter = 1;
        //private object dataGridView;

        private SQLiteConnection SQLiteConnection() // Подключаемся к нашей базе данных
        {
            connection = new SQLiteConnection("Data Source=Tasks.db;Version=3;");
            return connection;
        }

        public void CreateTasksTable() // создание таблицы Tasks
        {
            SQLiteConnection();
            connection.Open();

            string createTableQuery = "CREATE TABLE IF NOT EXISTS Tasks (Id INTEGER PRIMARY KEY AUTOINCREMENT, TaskText TEXT, Urgency TEXT, DueDate TEXT,User TEXT, Status TEXT)";
            SQLiteCommand command = new SQLiteCommand(createTableQuery, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }
        public void CreateEmployeesTable() // создание таблицы Employee
        {
            SQLiteConnection();
            connection.Open();

            string createTableQuery = "CREATE TABLE IF NOT EXISTS Employees (Id INTEGER PRIMARY KEY AUTOINCREMENT, Login TEXT, Password TEXT, Name TEXT, Position TEXT)";
            SQLiteCommand command = new SQLiteCommand(createTableQuery, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        private void UpdateUserComboBox() // Обновление списка сотрудников
        {
            UserComboBox.Items.Clear();
            UserComboBox.Items.AddRange(GetLoginEmployeeData());
        }

        private string[] GetLoginEmployeeData()
        {
            List<string> employeeDataList = new List<string>();

            string connectionString = "Data Source = Tasks.db; Version = 3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Создание команды SQL для выборки данных
                string sqlQuery = "SELECT Login, Position FROM Employees";
                using (SQLiteCommand command = new SQLiteCommand(sqlQuery, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string login = reader.GetString(0);
                            string position = reader.GetString(1);

                            // Конкатенация значений Login и Position и добавление их в список employeeDataList
                            string employeeData = $"{login} - {position}";
                            employeeDataList.Add(login);
                        }
                    }
                }
            }

            return employeeDataList.ToArray();
        }

        private string[] GetLoginPasswordEmployeeData()
        {
            List<string> employeeDataList = new List<string>();

            string connectionString = "Data Source = Tasks.db; Version = 3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Создание команды SQL для выборки данных
                string sqlQuery = "SELECT Login, Password FROM Employees";
                using (SQLiteCommand command = new SQLiteCommand(sqlQuery, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string login = reader.GetString(0);
                            string password = reader.GetString(1);

                            // Конкатенация значений Name и Position и добавление их в список employeeDataList
                            string employeeData = $"{login} - {password}";
                            employeeDataList.Add(employeeData);
                        }
                    }
                }
            }

            return employeeDataList.ToArray();
        }
        public MainForm()
        {
            CreateTasksTable();
            CreateEmployeesTable();
            UpdateEmployeesTable();

            // Настройки главной формы
            Text = "Приложение";
            Width = 1200;
            Height = 800;

            // Создание кнопки "Войти"
            loginButton = new Button();
            loginButton.Text = "Войти";
            loginButton.Width = 100;
            loginButton.Height = 30;
            loginButton.Left = (Width - loginButton.Width) / 2;
            loginButton.Top = (Height - loginButton.Height) / 2;

            // Добавление кнопки на форму
            Controls.Add(loginButton);

            InitializeDatabase();

            // Подключение обработчика события нажатия на кнопку "Войти"
            loginButton.Click += LoginButton_Click;

            // Подключение обработчика событий сочетаний клавиш
            KeyDown += MainForm_KeyDown;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            // Создание и настройка полей для ввода имени пользователя и пароля
            usernameTextBox = new TextBox();
            usernameTextBox.Width = 200;
            usernameTextBox.Left = (Width - usernameTextBox.Width) / 2;
            usernameTextBox.Top = loginButton.Top + loginButton.Height + 10;

            passwordTextBox = new TextBox();
            passwordTextBox.Width = 200;
            passwordTextBox.Left = (Width - passwordTextBox.Width) / 2;
            passwordTextBox.Top = usernameTextBox.Top + usernameTextBox.Height + 10;
            passwordTextBox.PasswordChar = '*'; // Замените символом, отображающим пароль

            // Добавление полей на форму
            Controls.Add(usernameTextBox);
            Controls.Add(passwordTextBox);

            // Подключение обработчика события нажатия на кнопку "Войти" после ввода данных
            loginButton.Click -= LoginButton_Click;
            loginButton.Click += LoginButtonWithCredentials_Click;
            loginButton.Text = "Войти с учетными данными";
        }

        private string GetEmployeePosition(string username)
        {
            string position = string.Empty;
            string connectionString = "Data Source = Tasks.db; Version = 3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sqlQuery = "SELECT Position FROM Employees WHERE Login = @Username";
                using (SQLiteCommand command = new SQLiteCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        position = result.ToString();
                    }
                }
            }
            return position;
        }
        public void CreateDeleteButton()
        {
            // Создание кнопки
            deleteEmployeeButton = new Button();
            deleteEmployeeButton.Text = "Удалить сотрудника";
            deleteEmployeeButton.Width = 150;
            deleteEmployeeButton.Height = 30;
            deleteEmployeeButton.Top = Height - deleteEmployeeButton.Height - 20; // Расположение по вертикали
            deleteEmployeeButton.Left = Width - deleteEmployeeButton.Width - 20; // Расположение по горизонтали

            // Добавление кнопки на форму
            Controls.Add(deleteEmployeeButton);
            deleteEmployeeButton.Click += DeleteEmployeeButton_Click;
            deleteEmployeeButton.Click += DeleteTaskButton_Click;
        }
        private void DeleteEmployeeButton_Click(object sender, EventArgs e)
        {
            if (employeeDataGridView.SelectedRows.Count > 0)
            {
                // Получение логина выбранного сотрудника из выбранной строки
                string login = employeeDataGridView.SelectedRows[0].Cells["Login"].Value.ToString();

                // Удаление сотрудника из таблицы DataGridView
                employeeDataGridView.Rows.Remove(employeeDataGridView.SelectedRows[0]);

                // Удаление сотрудника из базы данных
                DeleteEmployeeFromDatabase(login);
                UpdateUserComboBox();
            }
        }

        private void CreateDeleteTask()
        {
            deleteTaskButton = new Button();
            deleteTaskButton.Text = "Удалить задачу";
            deleteTaskButton.Width = 150;
            deleteTaskButton.Height = 30;
            deleteTaskButton.Top = Height - deleteTaskButton.Height + 30; // Расположение по вертикали
            deleteTaskButton.Left = Width - deleteTaskButton.Width - 20; // Расположение по горизонтали

            // Добавление кнопки на форму
            Controls.Add(deleteTaskButton);

            // Привязка обработчика события к кнопке "Удалить задачу"
            deleteTaskButton.Click += DeleteTaskButton_Click;
        }
        private void DeleteTaskFromDatabase(string taskText)
        {
            string connectionString = "Data Source=Tasks.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Создание команды SQL для удаления задачи по значению TaskText
                string deleteQuery = "DELETE FROM Tasks WHERE TaskText = @TaskText";
                using (SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@TaskText", taskText);
                    deleteCommand.ExecuteNonQuery();
                }
            }
        }

        private void DeleteTaskButton_Click(object sender, EventArgs e)
        {
            // Получение выбранной строки в таблице
            if (taskDataGridView.SelectedRows.Count > 0)
            {
                // Получение значения TaskText задачи из выбранной строки
                string taskText = Convert.ToString(taskDataGridView.SelectedRows[0].Cells["TaskText"].Value);

                // Выполнение удаления задачи из базы данных
                DeleteTaskFromDatabase(taskText);

                // Перезагрузка задач из базы данных
                LoadTasksFromDatabase();
            }
        }
        private void DeleteEmployeeFromDatabase(string login)
        {
            string connectionString = "Data Source=Tasks.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM Employees WHERE Login = @Login";
                using (SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@Login", login);
                    deleteCommand.ExecuteNonQuery();
                }
            }
        }


        public void CreateTasksFields()
        {
            // Создание и настройка текстового поля и кнопки для добавления задачи
            newTaskTextBox = new TextBox();
            newTaskTextBox.Width = 200;
            newTaskTextBox.Left = 200;
            newTaskTextBox.Top = 100;

            urgencyComboBox = new ComboBox();
            urgencyComboBox.Items.AddRange(new string[] { "Низкий", "Средний", "Высокий" });
            urgencyComboBox.Width = 100;
            urgencyComboBox.Left = newTaskTextBox.Left;
            urgencyComboBox.Top = newTaskTextBox.Top + newTaskTextBox.Height + 10;

            dueDateTimePicker = new DateTimePicker();
            dueDateTimePicker.Format = DateTimePickerFormat.Custom;
            dueDateTimePicker.CustomFormat = "dd.MM.yyyy HH:mm";
            dueDateTimePicker.Width = 130;
            dueDateTimePicker.Left = newTaskTextBox.Left;
            dueDateTimePicker.Top = urgencyComboBox.Top + urgencyComboBox.Height + 10;

            UserComboBox = new ComboBox();
            UserComboBox.Items.AddRange(GetLoginEmployeeData());
            UserComboBox.Width = 100;
            UserComboBox.Left = newTaskTextBox.Left;
            UserComboBox.Top = dueDateTimePicker.Top + dueDateTimePicker.Height + 10;

            addTaskButton = new Button();
            addTaskButton.Text = "Добавить задачу";
            addTaskButton.Width = 150;
            addTaskButton.Left = newTaskTextBox.Left;
            addTaskButton.Top = UserComboBox.Top + UserComboBox.Height + 10;


            // Создание таблицы для отображения списка задач
            taskDataGridView = new DataGridView();
            taskDataGridView.Width = 400;
            taskDataGridView.Height = 300;
            taskDataGridView.Left = newTaskTextBox.Left;
            taskDataGridView.Top = addTaskButton.Top + addTaskButton.Height + 10;
            taskDataGridView.AutoGenerateColumns = false;

            // Создание столбцов таблицы
            /*            DataGridViewTextBoxColumn taskNumberColumn = new DataGridViewTextBoxColumn();
                        taskNumberColumn.Name = "TaskNumber";
                        taskNumberColumn.HeaderText = "№";
                        taskNumberColumn.Width = 30;*/

            DataGridViewTextBoxColumn taskUrgencyColumn = new DataGridViewTextBoxColumn();
            taskUrgencyColumn.Name = "TaskUrgency";
            taskUrgencyColumn.HeaderText = "Приоритет";
            taskUrgencyColumn.Width = 80;
            DataGridViewTextBoxColumn taskTextColumn = new DataGridViewTextBoxColumn();
            taskTextColumn.Name = "TaskText";
            taskTextColumn.HeaderText = "Задача";
            taskTextColumn.Width = 200;
            DataGridViewTextBoxColumn taskDueDateColumn = new DataGridViewTextBoxColumn();
            taskDueDateColumn.Name = "TaskDueDate";
            taskDueDateColumn.HeaderText = "Срок выполнения";
            taskDueDateColumn.Width = 120;
            DataGridViewTextBoxColumn taskUserColumn = new DataGridViewTextBoxColumn();
            taskUserColumn.Name = "TaskUser";
            taskUserColumn.HeaderText = "Назначен";
            taskUserColumn.Width = 180;

            // Добавление столбцов в таблицу
            //taskDataGridView.Columns.Add(taskNumberColumn);
            taskDataGridView.Columns.Add(taskUrgencyColumn);
            taskDataGridView.Columns.Add(taskTextColumn);
            taskDataGridView.Columns.Add(taskDueDateColumn);
            taskDataGridView.Columns.Add(taskUserColumn);

            // Добавление DataGridView на форму
            Controls.Add(taskDataGridView);

            // Добавление элементов на форму
            Controls.Add(newTaskTextBox);
            Controls.Add(urgencyComboBox);
            Controls.Add(dueDateTimePicker);
            Controls.Add(UserComboBox);
            Controls.Add(addTaskButton);

            LoadTasksFromDatabase();

            // Подключение обработчика события нажатия на кнопку "Добавить задачу"
            addTaskButton.Click += AddTaskButton_Click;
        }

        public void CreateEmployeeFields()
        {

            // Создание таблицы для отображения списка сотрудников
            employeeDataGridView = new DataGridView();
            employeeDataGridView.Width = 400;
            employeeDataGridView.Height = 200;
            employeeDataGridView.Left = Width / 2;
            employeeDataGridView.Top = 100;
            employeeDataGridView.AutoGenerateColumns = false;

            // Создание полей для добавления нового сотрудника
            newEmployeeLoginTextBox = new TextBox();
            newEmployeeLoginTextBox.Width = 150;
            newEmployeeLoginTextBox.Left = employeeDataGridView.Left;
            newEmployeeLoginTextBox.Top = employeeDataGridView.Bottom + 10;

            newEmployeePasswordTextBox = new TextBox();
            newEmployeePasswordTextBox.Width = 150;
            newEmployeePasswordTextBox.Left = employeeDataGridView.Left;
            newEmployeePasswordTextBox.Top = newEmployeeLoginTextBox.Bottom + 10;
            newEmployeePasswordTextBox.PasswordChar = '*'; // прячем пароль

            newEmployeeNameTextBox = new TextBox();
            newEmployeeNameTextBox.Width = 150;
            newEmployeeNameTextBox.Left = employeeDataGridView.Left;
            newEmployeeNameTextBox.Top = newEmployeePasswordTextBox.Bottom + 10;

            newEmployeePositionTextBox = new TextBox();
            newEmployeePositionTextBox.Width = 150;
            newEmployeePositionTextBox.Left = employeeDataGridView.Left;
            newEmployeePositionTextBox.Top = newEmployeeNameTextBox.Bottom + 10;

            // Создание кнопки "Добавить сотрудника"
            addEmployeeButton = new Button();
            addEmployeeButton.Text = "Добавить сотрудника";
            addEmployeeButton.Width = 150;
            addEmployeeButton.Height = 30;
            addEmployeeButton.Left = newEmployeePositionTextBox.Left;
            addEmployeeButton.Top = newEmployeePositionTextBox.Bottom + 10;

            // Создание столбцов таблицы
            DataGridViewTextBoxColumn loginColumn = new DataGridViewTextBoxColumn();
            loginColumn.Name = "Login";
            loginColumn.HeaderText = "Логин";
            loginColumn.Width = 100;
            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.Name = "Name";
            nameColumn.HeaderText = "Имя";
            nameColumn.Width = 150;
            DataGridViewTextBoxColumn positionColumn = new DataGridViewTextBoxColumn();
            positionColumn.Name = "Position";
            positionColumn.HeaderText = "Должность";
            positionColumn.Width = 150;

            // Добавление столбцов в таблицу
            employeeDataGridView.Columns.Add(loginColumn);
            employeeDataGridView.Columns.Add(nameColumn);
            employeeDataGridView.Columns.Add(positionColumn);

            // Добавление элементов на форму
            Controls.Add(employeeDataGridView);
            Controls.Add(newEmployeeLoginTextBox);
            Controls.Add(newEmployeePasswordTextBox);
            Controls.Add(newEmployeeNameTextBox);
            Controls.Add(newEmployeePositionTextBox);
            Controls.Add(addEmployeeButton);

            LoadEmployeeFromDatabase();
            // Подключение обработчиков событий для кнопок
            addEmployeeButton.Click += AddEmployeeButton_Click;

            //загрузка сотрудников из файла
            //LoadEmployees();
        }
        private void addTaskButton_Click(object sender, EventArgs e)
        {
            // Получаем выбранную строку в DataGridView
            DataGridViewRow selectedRow = taskDataGridView.SelectedRows[0];

            // Получаем значения столбцов выбранной строки
            string taskText = selectedRow.Cells["TaskText"].Value.ToString();
            string urgency = selectedRow.Cells["TaskUrgency"].Value.ToString();
            //string dueDate = selectedRow.Cells["TaskDueDate"].Value.ToString();
            string status = "Выполнено";

            // Обновляем значение столбца "Статус" в DataGridView
            selectedRow.Cells["TaskStat"].Value = status;

            // Обновляем запись в базе данных
            string query = "UPDATE Tasks SET Status = @Status WHERE (TaskText = @TaskText AND DueDate = @DueDate) OR (TaskText = @TaskText AND DueDate = @DueDate AND Status IS NULL)";

            SQLiteConnection();
            string connectionString = "Data Source=Tasks.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TaskText", taskText);
                    command.Parameters.AddWithValue("@Urgency", urgency);
                    //command.Parameters.AddWithValue("@DueDate", dueDate);
                    command.Parameters.AddWithValue("@Status", status);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        private void CreateFieldByUser()
        {

            addTaskButton = new Button();
            addTaskButton.Text = "Выполнено";
            addTaskButton.Width = 100;
            addTaskButton.Left = 100;
            addTaskButton.Top = 100;

            taskDataGridView = new DataGridView();
            taskDataGridView.Width = 400;
            taskDataGridView.Height = 200;
            taskDataGridView.Left = Width / 2;
            taskDataGridView.Top = 100;
            taskDataGridView.AutoGenerateColumns = false;

            DataGridViewTextBoxColumn taskTextColumn = new DataGridViewTextBoxColumn();
            taskTextColumn.Name = "TaskText";
            taskTextColumn.HeaderText = "Задача";
            taskTextColumn.Width = 200;
            DataGridViewTextBoxColumn taskUrgencyColumn = new DataGridViewTextBoxColumn();
            taskUrgencyColumn.Name = "TaskUrgency";
            taskUrgencyColumn.HeaderText = "Приоритет";
            taskUrgencyColumn.Width = 80;
            DataGridViewTextBoxColumn taskDueDateColumn = new DataGridViewTextBoxColumn();
            taskDueDateColumn.Name = "TaskDueDate";
            taskDueDateColumn.HeaderText = "Срок выполнения";
            taskDueDateColumn.Width = 120;
            DataGridViewTextBoxColumn taskStatColumn = new DataGridViewTextBoxColumn();
            taskStatColumn.Name = "TaskStat";
            taskStatColumn.HeaderText = "Статус";
            taskStatColumn.Width = 120;

            taskDataGridView.Columns.Add(taskTextColumn);
            taskDataGridView.Columns.Add(taskUrgencyColumn);
            taskDataGridView.Columns.Add(taskDueDateColumn);
            taskDataGridView.Columns.Add(taskStatColumn);
            Controls.Add(taskDataGridView);
            Controls.Add(addTaskButton);
            addTaskButton.Click += addTaskButton_Click;
        }
        /*        private string GetEmployeePosition(string login)
                {
                    string position = string.Empty;
                    string query = "SELECT Position FROM Employees WHERE Login = @Login";

                    SQLiteConnection();
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Login", login);

                        connection.Open();

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                position = reader.GetString(0);
                            }
                        }

                        connection.Close();
                    }

                    return position;
                }*/
        private void LoadTasksByUser(string login)
        {
            string query = "SELECT TaskText, Urgency, DueDate FROM Tasks WHERE User = @User";

            SQLiteConnection();
            //string user GetEmployeePosition(login);
            //MessageBox.Show(login);
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@User", login);

                connection.Open();

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    //taskGridView.Rows.Clear(); // Очистка данных в DataGridView

                    while (reader.Read())
                    {
                        string taskText = reader.GetString(0);
                        string urgency = reader.GetString(1);
                        DateTime dueDateValue = reader.GetDateTime(2);

                        DataGridViewRow row = new DataGridViewRow();
                        row.CreateCells(taskDataGridView);
                        row.Cells[0].Value = taskText;
                        row.Cells[1].Value = urgency;
                        row.Cells[2].Value = dueDateValue.ToString("dd.MM.yyyy HH:mm");
                        taskDataGridView.Rows.Add(row);

                        //taskDataGridView.Rows.Add(urgency, taskText, dueDateValue);
                    }
                }

                connection.Close();
            }
        }
        private bool ValidateCredentials(string username, string password)
        {
            string query = "SELECT COUNT(*) FROM Employees WHERE Login = @Login AND Password = @Password";

            SQLiteConnection();
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Login", username);
                command.Parameters.AddWithValue("@Password", password);

                connection.Open();

                object result = command.ExecuteScalar();
                int count = result != null ? Convert.ToInt32(result) : 0;

                connection.Close();

                return count > 0;
            }
        }

        private void LoginButtonWithCredentials_Click(object sender, EventArgs e)
        {
            string login = usernameTextBox.Text;
            string password = passwordTextBox.Text;

            if (ValidateCredentials(login, password))
            {
                // Проверка должности пользователя
                string position = GetEmployeePosition(login);
                if (position == "Boss")
                {
                    // Удаление полей для ввода имени пользователя и пароля
                    Controls.Remove(usernameTextBox);
                    Controls.Remove(passwordTextBox);

                    // Удаление кнопки "Войти"
                    Controls.Remove(loginButton);
                    CreateDeleteTask();
                    CreateDeleteButton();
                    CreateEmployeeFields();
                    CreateTasksFields();
                }
                else if (position != "Boss")
                {
                    CreateFieldByUser();
                    LoadTasksByUser(login);
                    usernameTextBox.Clear();
                    passwordTextBox.Clear();
                }
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль.");
                usernameTextBox.Clear();
                passwordTextBox.Clear();
            }
        }
        private bool IsEmployeeAlreadyExists(string login/*,string name*/)
        {
            string query = "SELECT COUNT(*) FROM Employees WHERE Login = @Login";
            SQLiteConnection();
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Login", login);
                /* command.Parameters.AddWithValue("@Name", *//*name*//*);*/

                connection.Open(); // Открытие соединения
                object result = command.ExecuteScalar();
                int count = result != null ? Convert.ToInt32(result) : 0;

                connection.Close(); // Закрытие соединения

                return count > 0; // Возвращает true, если найдено хотя бы одно совпадение
            }
        }
        private bool IsTaskTextAlreadyExistsInTable(string taskText)
        {
            foreach (DataGridViewRow row in taskDataGridView.Rows)
            {
                if (row.Cells["TaskText"].Value != null && row.Cells["TaskText"].Value.ToString() == taskText)
                {
                    return true; // Данные уже существуют в таблице
                }
            }
            return false; // Данные не найдены в таблице
        }
        private void LoadTasksFromDatabase()
        {
            connection = SQLiteConnection();
            using (connection)
            {
                // Открытие подключения
                connection.Open();

                // Создание команды SQL
                string sqlQuery = "SELECT TaskText, Urgency, DueDate, User FROM Tasks";
                using (SQLiteCommand command = new SQLiteCommand(sqlQuery, connection))
                {
                    // Выполнение команды и получение данных
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        // Очистка данных в таблице DataGridView
                        taskDataGridView.Rows.Clear();

                        // Чтение данных и добавление их в таблицу DataGridView
                        while (reader.Read())
                        {
                            string taskTextValue = reader.GetString(0);
                            string urgencyValue = reader.GetString(1);
                            DateTime dueDateValue = reader.GetDateTime(2);
                            string userValue = reader.GetString(3);

                            // Проверка наличия данных в таблице DataGridView
                            if (IsTaskTextAlreadyExistsInTable(taskTextValue))
                            {
                                continue; // Пропустить добавление строки, если данные уже существуют
                            }

                            DataGridViewRow row = new DataGridViewRow();
                            row.CreateCells(taskDataGridView);
                            row.Cells[0].Value = urgencyValue;
                            row.Cells[1].Value = taskTextValue;
                            row.Cells[2].Value = dueDateValue.ToString("dd.MM.yyyy HH:mm");
                            row.Cells[3].Value = userValue;
                            taskDataGridView.Rows.Add(row);
                        }
                    }
                }
            }
        }
        private void LoadEmployeeFromDatabase()
        {
            string connectionString = "Data Source = Tasks.db; Version = 3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                // Открытие подключения
                connection.Open();

                // Создание команды SQL
                string sqlQuery = "SELECT Login, Name, Position FROM Employees";
                using (SQLiteCommand command = new SQLiteCommand(sqlQuery, connection))
                {
                    // Выполнение команды и получение данных
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        // Очистка данных в таблице DataGridView
                        employeeDataGridView.Rows.Clear();

                        // Чтение данных и добавление их в таблицу DataGridView
                        while (reader.Read())
                        {
                            /*                            if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2))
                                                        {*/
                            // Извлечение значений из результата запроса
                            string login = reader.GetString(0);
                            string name = reader.GetString(1);
                            string position = reader.GetString(2);

                            // Добавление данных в таблицу DataGridView
                            employeeDataGridView.Rows.Add(login, name, position);
                            //}
                        }
                    }
                }
            }
        }



        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                // Обработка сочетания клавиш Ctrl + S
                // Например, сохранение файла
            }
            else if (e.Control && e.KeyCode == Keys.O)
            {
                // Обработка сочетания клавиш Ctrl + O
                // Например, открытие файла
            }
            else if (e.Alt && e.KeyCode == Keys.F4)
            {
                // Обработка сочетания клавиш Alt + F4
                // Например, закрытие окна
            }
            // Добавьте другие обработки клавиш по аналогии
        }
        private bool IsTaskAlreadyExists(string taskText)
        {
            string query = "SELECT COUNT(*) FROM Tasks WHERE TaskText = @TaskText";
            SQLiteConnection();
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TaskText", taskText);

                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();

                return count > 0;
            }
        }
        private void InsertTaskIntoDatabase(string taskText, string urgency, DateTime dueDate, string user)
        {
            // Проверка на уникальность названия задачи
            if (IsTaskAlreadyExists(taskText))
            {
                MessageBox.Show("Задача с таким названием уже существует.", "Ошибка");
                return;
            }

            string query = "INSERT INTO Tasks (TaskText, Urgency, DueDate, User) VALUES (@TaskText, @Urgency, @DueDate, @User)";
            SQLiteConnection();
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TaskText", taskText);
                command.Parameters.AddWithValue("@Urgency", urgency);
                command.Parameters.AddWithValue("@DueDate", dueDate);
                command.Parameters.AddWithValue("@User", user);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            LoadTasksFromDatabase();
        }
        private void AddTaskButton_Click(object sender, EventArgs e)
        {
            string taskText = newTaskTextBox.Text;
            string urgency = urgencyComboBox.SelectedItem != null ? urgencyComboBox.SelectedItem.ToString() : string.Empty;
            DateTime dueDate = dueDateTimePicker.Value;
            string user = UserComboBox.Text;

            if (string.IsNullOrWhiteSpace(taskText) || string.IsNullOrWhiteSpace(urgency) || string.IsNullOrWhiteSpace(user))
            {
                MessageBox.Show("Пожалуйста, заполните все поля для добавления новой задачи.");
                return; // Остановить выполнение метода
            }

            // Установка секунд в ноль
            dueDate = dueDate.AddSeconds(-dueDate.Second); // Удалить секунды

            InsertTaskIntoDatabase(taskText, urgency, dueDate, user);

            newTaskTextBox.Clear();
            urgencyComboBox.SelectedIndex = -1;
            dueDateTimePicker.Value = DateTime.Now;
            UserComboBox.SelectedIndex = -1;

            taskCounter++;
        }



        private void InsertEmployeeIntoDatabase(string login, string password, string name, string position)
        {
            if (IsEmployeeAlreadyExists(login /*,name)*/))
            {
                MessageBox.Show("Такой сотрудник уже существует в базе данных.", "Ошибка");
                return;
            }

            string query = "INSERT INTO Employees (Login, Password, Name, Position) VALUES (@Login, @Password, @Name, @Position)";
            SQLiteConnection();
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Position", position);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            LoadEmployeeFromDatabase();
        }

        private void AddEmployeeButton_Click(object sender, EventArgs e)
        {
            string login = newEmployeeLoginTextBox.Text;
            string password = newEmployeePasswordTextBox.Text;
            string name = newEmployeeNameTextBox.Text;
            string position = newEmployeePositionTextBox.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(position))
            {
                MessageBox.Show("Пожалуйста, заполните все поля для добавления нового сотрудника.");
                return; // Остановить выполнение метода
            }
            InsertEmployeeIntoDatabase(login, password, name, position);

            newEmployeeLoginTextBox.Clear();
            newEmployeePasswordTextBox.Clear();
            newEmployeeNameTextBox.Clear();
            newEmployeePositionTextBox.Clear();

            UpdateUserComboBox();
        }
    }
}
