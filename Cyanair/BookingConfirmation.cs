using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Cyanair
{
    public partial class BookingConfirmation : Form
    {
        public string email;
        public string phone;
        public string leg1GuidFrom;
        public string leg1GuidTo;
        public string leg2GuidFrom;
        public string leg2GuidTo;

        public BookingConfirmation()
        {
            InitializeComponent();
        }

        private void btnConfirmBooking_Click(object sender, EventArgs e)
        {
            DBConnection dBConnection = new DBConnection();

            //create a guid for the passengers table
            string passengerGuid = Guid.NewGuid().ToString();

            //format the last name and first name
            txtBoxPassengerName.Text.Trim();
            int firstSpaceIndex = txtBoxPassengerName.Text.IndexOf(" ");
            string fName = txtBoxPassengerName.Text.Substring(0, firstSpaceIndex);
            string lName = txtBoxPassengerName.Text.Substring(firstSpaceIndex);

            //db method to insert booing information into the database
            dBConnection.insertIntoPassenger(passengerGuid, fName, lName, email, phone, txtBoxPassportNum.Text);

            //format flight details for the booking object
            string flight = "FROM " + txtBoxDepartFrom.Text + " " + txtBoxDepartFromTime.Text + " To " + txtBoxDepartTo.Text + " " + txtBoxDepartToTime.Text + " Class: " + txtBoxDepartSeatClass.Text + " " + txtBoxDepartDate.Text + "\n";
           
            if (gbBookingConfirmLeg.Visible)
            {
                 flight += "FROM " + txtBoxLegFrom.Text + " " + txtBoxLegFromTime.Text + " To " + txtBoxLegTo.Text + " " + txtBoxLegToTime.Text + " " + txtBoxLegSeatClass.Text + " Class: " + txtBoxLegDate.Text;
 
            }
           
            flight.Trim();

            //create a booking guid 
            string bookingsGuid = Guid.NewGuid().ToString();

            //insert bookings data into the bookings table
            dBConnection.insertIntoBookings(bookingsGuid, txtBoxBookingRefNum.Text, passengerGuid, flight, txtBoxDateBooked.Text);


            MessageBox.Show("Booking Confirmed");
            this.Close();
        }
       
    }
}
