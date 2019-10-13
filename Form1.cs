using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Testing_Project_CRUD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // панель передвижения формы
            Top_panel();
            // кнопка выхода из програмы
            btn_Close.Click += (s, e) => { Close(); };
            // кнопки переключения между панелями!
            switch_create.Click += (s, e) => { Panel(pnl_CreateDate); };
            switch_database.Click += (s, e) => { Panel(pnl_EditDate); };
            switch_trigger.Click += (s, e) => { Panel(pnl_Trigger); };
            // свернуть форму
            btn_minimize.Click += (s, e) => { this.WindowState = FormWindowState.Minimized; };
        }
        void Panel(Panel panel)
        {
            panel.BringToFront();
        }
        // метод для передвжения формы
        void Top_panel()
        {
            int move = 0, moveX = 0, moveY = 0;
            pnl_Top.MouseDown += (s, e) => { move = 1; moveX = e.X; moveY = e.Y; };
            pnl_Top.MouseMove += (s, e) => { if (move == 1) SetDesktopLocation(MousePosition.X - moveX, MousePosition.Y - moveY); };
            pnl_Top.MouseUp += (s, e) => { move = 0; };
        }
        // столка подключения к Базе данных ms sql
        readonly static string myConnection = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
        // метод добавления данных в базу данных
        public bool Insert()
        {
            bool IsSuccess = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(myConnection))
                {
                    connection.Open();
                    string query = $"INSERT INTO User_date ([Имя], [Фамилия], [Группа], [instagram], [Пол]) VALUES ('{txt_name.Text}', '{txt_surname.Text}', '{txt_group.Text}', '{txt_instagram.Text}', '{cmb_Gender.Text}')";
                    SqlCommand command = new SqlCommand(query, connection);
                    int row = command.ExecuteNonQuery();
                    IsSuccess = row > 0 ? true : false;
                    Refresh();
                    ClearRows();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Уведомление системы!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return IsSuccess;
        }
        // уведомление системы
        void Message(string line)
        {
            MessageBox.Show(line, "Уведомление системы!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        // событие вызывающее метод добавления
        void Btn_add_Click(object sender, EventArgs e)
        {
            bool success = Insert();
            Message(success ? "Добавление прошло успешно!" : "Косяк, не удалось отправить данные в Базу Данных!");
        }
        // вывод данных на DataGridView
        public new DataTable Select()
        {
            DataTable data = new DataTable();
            try
            {
                string query = "SELECT * FROM User_date";
                using(SqlConnection connection = new SqlConnection(myConnection))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(data);
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message, ex.Source); }
            return data;
        }
        // метод обновления таблицы
        new void Refresh()
        {
            DataTable data = Select();
            dataGridView1.DataSource = data;
        }
        // отчистка полей
        void ClearRows()
        {
            txt_ID.Text = "";
            txt_name.Text = "";
            txt_group.Text = "";
            txt_instagram.Text = "";
            cmb_Gender.Text = "";
        }
        // событие загрузки формы, для вывода данных в dataGridView1
        private void Form1_Load(object sender, EventArgs e) => Refresh();
        // события двойного клика для выборки нужной строки
        void DataGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int rowIndex = e.RowIndex;
            txt_ID.Text = dataGridView1.Rows[rowIndex].Cells[0].Value.ToString();
            txt_name.Text = dataGridView1.Rows[rowIndex].Cells[1].Value.ToString();
            txt_surname.Text = dataGridView1.Rows[rowIndex].Cells[2].Value.ToString();
            txt_group.Text = dataGridView1.Rows[rowIndex].Cells[3].Value.ToString();
            txt_instagram.Text = dataGridView1.Rows[rowIndex].Cells[4].Value.ToString();
            cmb_Gender.Text = dataGridView1.Rows[rowIndex].Cells[5].Value.ToString();
        }

        // метод редактирования данных
        public new bool Update()
        {
            bool IsSuccess = false;
            try
            {
                string query = $"UPDATE User_date SET [Имя] = '{txt_name.Text}', [Фамилия] = '{txt_surname.Text}', [Группа] = '{txt_group.Text}', [instagram] = '{txt_instagram.Text}', [Пол] = '{cmb_Gender.Text}' where Id='{txt_ID.Text}'";
                using(SqlConnection connection = new SqlConnection(myConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    int row = command.ExecuteNonQuery();
                    IsSuccess = row > 0 ? true : false;
                    Refresh();
                    ClearRows();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return IsSuccess;
        }
        // событи вызова метода редактирования
        void Btn_edit_Click(object sender, EventArgs e)
        {
            bool success = Update();
            Message(success ? "Редактирование данных прошло успешно и сохранены в Базе Данных" : "Косяк, произошла неизвестная ошибка!");
        }
        // метод удаления
        public bool Delete()
        {
            bool IsSuccess = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(myConnection))
                {
                    connection.Open();
                    string query = $"DELETE FROM User_date where Id = '{txt_ID.Text}'";
                    SqlCommand command = new SqlCommand(query, connection);
                    int row = command.ExecuteNonQuery();
                    IsSuccess = row > 0 ? true : false;
                    Refresh();
                    ClearRows();
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return IsSuccess;
        }
        // событие вызова метода удаления
        void Btn_delete_Click(object sender, EventArgs e)
        {
            bool success = Delete();
            Message(success ? "Удаление прошло успешно!" : "Косяк, удаление магическим образом не сработало!");
        }
        // поиск данных в Базе данных по ключевому слову
        void Txt_search_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = txt_search.Text;
                using (SqlConnection connection = new SqlConnection(myConnection))
                {
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM User_date WHERE [Имя] LIKE '%" + keyword + "%' OR [Фамилия] LIKE '%" + keyword + "%' OR [instagram] LIKE '%" + keyword + "%'", connection);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}
