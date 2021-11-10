using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SQLite;

namespace Cyanair
{
   
    public partial class frmLogin : Form
    {
        //create a booking agent object
        List<BookingAgent> usersList = new List<BookingAgent>();

        //public
        public static string currentUser = "";
        private bool isNotCorrectLoginDetails = true;

        // variable to stoe the number of rows in a given table
        int rowCount;
        public frmLogin()
        {
            InitializeComponent();
        }



        private void frmLogin_Load(object sender, EventArgs e)
        {
            // create db object
            DBConnection dBConnection = new DBConnection();

            //check if the file exists
            dBConnection.validateDBfileExist();

            //open the database connection
            dBConnection.openDbConnection();

            //fetch the booking agents details from the database
            dBConnection.getAllUsers("users");

            //get number of entries in the BookingAgents table 
            rowCount = dBConnection.dataSet.Tables["users"].Rows.Count;

            //create booking agent objects for each user 
            for (int i = 0; i < rowCount; i++)
            {
                usersList.Add(new BookingAgent());
                usersList[i].FirstName = dBConnection.dataSet.Tables["users"].Rows[i]["agent_first_name"].ToString();
                usersList[i].LastName = dBConnection.dataSet.Tables["users"].Rows[i]["agent_last_name"].ToString();
                usersList[i].Password = dBConnection.dataSet.Tables["users"].Rows[i]["login_password"].ToString();
                usersList[i].adminRole = dBConnection.dataSet.Tables["users"].Rows[i]["admin_role"].ToString();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

            CheckDetails();

            if(isNotCorrectLoginDetails == true)
            {
                MessageBox.Show("Please enter the correct login details");
            }
                               
        }
        public void CheckDetails()
        {

            foreach (BookingAgent user in usersList)
            {
                if ((user.FirstName + " " + user.LastName) == System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtBoxName.Text.ToLower()) && user.Password == txtBoxPassword.Text)
                {
                    if (user.adminRole == "1")
                    {
                        currentUser = user.FirstName + " " + user.LastName + ": Admin";
                    }
                    else
                    {
                        currentUser = user.FirstName + " " + user.LastName + ": Agent";
                    }

                    isNotCorrectLoginDetails = false;

                    frmMain newForm = new frmMain();

                    newForm.Show();
                }
               
            }

        }
    }
}
