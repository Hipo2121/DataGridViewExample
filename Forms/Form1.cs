using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows.Forms;

namespace Forms
{
    public partial class Form1 : Form
    {
        private AppDbContext context;
        private BindingList<User> users;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tblUsers.AutoGenerateColumns = false;
            tblUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            tblUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "firstName",
                HeaderText = "Jm�no"
            });

            tblUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "lastName",
                HeaderText = "P��jmen�"
            });

            tblUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "birthDate",
                HeaderText = "Datum narozen�"
            });

            tblUsers.Columns.Add(new DataGridViewComboBoxColumn
            {
                DataPropertyName = "gender",
                HeaderText = "Pohlav�",
                DataSource = new char[] { 'M', 'F' }
            });

            context = new AppDbContext();

            context.ChangeTracker.AutoDetectChangesEnabled = true;

            context.Users.Load();
            users = context.Users.Local.ToBindingList();

            tblUsers.DataSource = users;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (areUsersValid() && MessageBox.Show("Opravdu chcete ulo�it zm�ny?", "Ulo�it", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                context.SaveChanges();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            User user = new User
            {
                firstName = txtFirstName.Text,
                lastName = txtLastName.Text,
                birthDate = DateOnly.FromDateTime(dtpBirthDate.Value),
                gender = rbnMale.Checked ? 'M' : 'F'
            };

            if (isUserValid(user))
            {
                users.Add(user);
                resetForm();
            }
        }

        private void resetForm()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            dtpBirthDate.Value = DateTime.Now;
            rbnMale.Checked = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in tblUsers.SelectedRows)
            {
                users.Remove((User)row.DataBoundItem);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            context?.Dispose();
        }

        private void tblUsers_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Chyba dat", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // Prevent default error window
            e.ThrowException = false;

            tblUsers.CancelEdit();
        }

        private bool areUsersValid()
        {
            foreach (User user in users)
            {
                if (!isUserValid(user))
                {
                    MessageBox.Show("Jm�no a p��jmen� je povinn�.");
                    return false;
                }
            }
            return true;
        }

        private bool isUserValid(User user)
        {
            return (!string.IsNullOrWhiteSpace(user.firstName) && !string.IsNullOrWhiteSpace(user.lastName));
        }
    }
}
