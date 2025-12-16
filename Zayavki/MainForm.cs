using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using RepairRequests;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Zayavki
{
    public partial class MainForm : Form
    {
        private readonly RepairRequestService _requestService;
        private readonly string _connectionString = "server=localhost;user=root;password=vertrigo;database=application;";
        private List<Model> allRequests = new List<Model>();
        public MainForm()
        {
            InitializeComponent();
            _requestService = new RepairRequestService(_connectionString);
            comboFilterStatus.SelectedIndex = 0;
            comboFilterStatus.SelectedIndexChanged += comboFilterStatus_SelectedIndexChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadRequests();
        }
        private void ApplyFilter()
        {
            string selectedStatus = comboFilterStatus.SelectedItem?.ToString() ?? "Все заявки";

            List<Model> filtered;

            if (selectedStatus == "Все заявки")
            {
                filtered = allRequests;
            }
            else
            {
                filtered = allRequests
                    .Where(r => r.Status == selectedStatus)
                    .ToList();
            }

            UpdateDataGrid(filtered);
        }

        private void LoadRequests()
        {
            allRequests = _requestService.GetAllRequests();
            UpdateDataGrid(allRequests);
        }

        // Метод обновления DataGridView с русскими заголовками
        private void UpdateDataGrid(List<Model> requests)
        {
            var dt = new DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("Наименование", typeof(string));
            dt.Columns.Add("Описание проблемы", typeof(string));
            dt.Columns.Add("Срок выполнения", typeof(DateTime));
            dt.Columns.Add("Ответственный", typeof(string));
            dt.Columns.Add("Статус", typeof(string));

            foreach (var r in requests)
                dt.Rows.Add(r.Id, r.Name, r.Problem, r.Deadline, r.Responsible, r.Status);

            dataGridView1.DataSource = dt;

        }

        //Метод для экспорта таблицы в Excel
        private void ExportToExcel()
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var excelApp = new Excel.Application();
                excelApp.Visible = true; // можно сделать false, если не нужно показывать сразу
                Excel.Workbook workbook = excelApp.Workbooks.Add();
                Excel.Worksheet worksheet = workbook.Sheets[1];

                // Заголовки
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    if (dataGridView1.Columns[j].Visible) // учитываем только видимые колонки
                        worksheet.Cells[1, j + 1] = dataGridView1.Columns[j].HeaderText;
                }

                // Данные
                int rowIndex = 2;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    int colIndex = 1;
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        if (dataGridView1.Columns[j].Visible)
                        {
                            worksheet.Cells[rowIndex, colIndex] = dataGridView1.Rows[i].Cells[j].Value?.ToString();
                            colIndex++;
                        }
                    }
                    rowIndex++;
                }

                MessageBox.Show("Экспорт завершён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangeStatusWithConfirm(string newStatus)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите заявку!");
                return;
            }

            int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);
            string currentStatus = dataGridView1.SelectedRows[0].Cells["Статус"].Value.ToString();

            // Проверяем через валидатор
            var validation = RequestValidator.ValidateStatusChange(currentStatus, newStatus);
            if (!validation.IsValid)
            {
                MessageBox.Show(validation.ErrorMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Подтверждение
            if (MessageBox.Show(
                $"Вы уверены, что хотите изменить статус c '{currentStatus}' на '{newStatus}'?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            // Меняем статус через сервис
            var result = _requestService.ChangeStatus(id, newStatus);

            if (result.IsSuccess)
            {
                MessageBox.Show("Статус успешно обновлен!", "Успех");
                LoadRequests();
            }
            else
            {
                MessageBox.Show("Ошибка обновления статуса!", "Ошибка");
            }
        }

       

        private void buttonV_Click(object sender, EventArgs e)
        {
            ChangeStatusWithConfirm("Выполнено");
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            LoadRequests();
        }

        private void buttonO_Click(object sender, EventArgs e)
        {
            ChangeStatusWithConfirm("Не выполнено");
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Проверка заполнения обязательных полей
            if (string.IsNullOrWhiteSpace(textBoxName.Text) ||
                string.IsNullOrWhiteSpace(textBoxProblem.Text) ||
                string.IsNullOrWhiteSpace(textBoxResponsible.Text))
            {
                MessageBox.Show("Заполните все обязательные поля!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Создаем модель заявки
            var newRequest = new Model
            {
                Name = textBoxName.Text,
                Problem = textBoxProblem.Text,
                Responsible = textBoxResponsible.Text,
                Deadline = dateTimePickerDeadline.Value,
                Status = checkedListBox1.CheckedItems[0].ToString()
            };

            // Вызов сервиса
            var result = _requestService.AddNewRequest(newRequest);

            if (result.IsSuccess)
            {
                MessageBox.Show("Заявка успешно добавлена!",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadRequests(); // обновляем таблицу
            }
            else
            {
                MessageBox.Show($"Ошибка при добавлении заявки: {result.Message}",
                    "Ошибка при добавлении заявки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
            ClearForm();
        }       

        private void comboFilterStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }
        private void ClearForm() 
        { textBoxName.Text = "";
          textBoxResponsible.Text = ""; 
          textBoxProblem.Text = ""; 
          checkedListBox1.ClearSelected(); 
          for (int i = 0; i < checkedListBox1.Items.Count; i++) 
          checkedListBox1.SetItemChecked(i, false); 
        }
    }
}

