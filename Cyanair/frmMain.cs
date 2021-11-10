using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Drawing;
using Microsoft.SqlServer.Server;
using System.IO;

namespace Cyanair
{
    
    public partial class frmMain : Form
    {
        //create new route objects for each possilble legs
        public Routes departRoute = new Routes();
        public Routes returnRoute = new Routes();
        public Routes secondLegRoute = new Routes();

        public Passenger passenger = new Passenger();

        public Booking booking = new Booking();

      
       

        String todayDate = DateTime.Now.ToString("dd/MM/yyyy");
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //Initial UI setup, disable these option until checkboxes are enabled
            gbBookingsReturn.Visible = false;
            gbBookingsLeg.Visible = false;
            chkBoxReturn.Enabled = false;
            chkBoxLeg.Enabled = false;

            //initial seat class settings
            radioBookEccoDepart.Checked = true;
            radioBookEccoLeg.Checked = true;
            radioBookEccoReturn.Checked = true;

            
            //Display the current user
            lbCurrentUser.Text = frmLogin.currentUser;

            if (frmLogin.currentUser == "Lars Ulrich: Admin")
            {
                ((Control)this.ViewBookings).Enabled = true;
                ((Control)this.editAirports).Enabled = true;
            }
            else
            {
                ((Control)this.ViewBookings).Enabled = false;
                ((Control)this.editAirports).Enabled = false;
            }

            //new db connection
            DBConnection dBConnection = new DBConnection();

            //methods to get all airports and airprt codes
            dBConnection.getAirportsCodes("DeparturesFrom");
            dBConnection.getAirportsCodes("DeparturesTo");
            dBConnection.getAirportsCodes("SecondLegTo");

            //methods to get all bookings for the admin tab datagrid
            dBConnection.getAllBookingsDataGrid("Bookings");


            dBConnection.getAllBooking("AllBookings");

            //methos to get all airports for the admin tabs
            dBConnection.getAllAirports("AdminAirportsList");
            dBConnection.getAirportsCodes("AdinAirportCodes");

            //mehod to get all country names for the admin tab
            dBConnection.getAllCountryNames("CountryNames");

            cmbSelectCountry.DataSource = dBConnection.dataSet.Tables["countryNames"];
            cmbSelectCountry.DisplayMember = "country_name";
            cmbSelectCountry.ValueMember = "guid";

            //get all bookings for bookings view tab datagrid
            dgListOfBookings.DataSource = dBConnection.dataSet.Tables["Bookings"];

            //get values combo box booking ref num 
            cmbBookingRefNum.DataSource = dBConnection.dataSet.Tables["AllBookings"];
            cmbBookingRefNum.DisplayMember = "Ref Num";
            cmbBookingRefNum.ValueMember = "Ref Num";

            //get airport for admin tab
            dataGridListOfAirports.DataSource = dBConnection.dataSet.Tables["AdminAirportsList"];
            cmbDeleteAirport.DataSource = dBConnection.dataSet.Tables["AdinAirportCodes"];
            cmbDeleteAirport.DisplayMember = "airport_code";
            
            
            //Fill departures Comboxes
            cmbBookDepartFrom.DataSource = dBConnection.dataSet.Tables["DeparturesFrom"];
            cmbBookDepartFrom.DisplayMember = "airport_code";
            cmbBookDepartFrom.ValueMember = "guid";

            cmbBookDepartTo.DataSource = dBConnection.dataSet.Tables["DeparturesTo"];
            cmbBookDepartTo.DisplayMember = "airport_code";
            cmbBookDepartTo.ValueMember = "guid";

            //comboboxes for return flights will always be the reverse of deparute flights
            cmbBookReturnFrom.DataSource = dBConnection.dataSet.Tables["DeparturesTo"];
            cmbBookReturnFrom.DisplayMember = "airport_code";
            cmbBookReturnFrom.ValueMember = "guid";

            cmbBookReturnTo.DataSource = dBConnection.dataSet.Tables["DeparturesFrom"];
            cmbBookReturnTo.DisplayMember = "airport_code";
            cmbBookReturnTo.ValueMember = "guid";

            //Combox for Leg from will always equal destination od departure 
            cmbBookLegFrom.DataSource = dBConnection.dataSet.Tables["DeparturesTo"];
            cmbBookLegFrom.DisplayMember = "airport_code";
            cmbBookLegFrom.ValueMember = "guid";

            //Combox for secdond leg destination
            cmbBookLegTo.DataSource = dBConnection.dataSet.Tables["SecondLegTo"];
            cmbBookLegTo.DisplayMember = "airport_code";
            cmbBookLegTo.ValueMember = "guid";

            dBConnection.getAllFlights("currentFlights");
            dataGridBookFlightSrch.DataSource = dBConnection.dataSet.Tables["currentFlights"];

        }


        //Enable - disable return groupbox using the return check box
        private void chkBoxReturn_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBoxReturn.Checked)
            {
                gbBookingsReturn.Visible = true;
                chkBoxLeg.Checked = false;
            }
            else
            {
                gbBookingsReturn.Visible = false;
            }
        }

        //Enable - disable second leg groupbox using the second leg check box
        private void chkBoxLeg_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBoxLeg.Checked)
            {
                gbBookingsLeg.Visible = true;
                chkBoxReturn.Checked = false;
            }
            else
            {
                gbBookingsLeg.Visible = false;
            }

        }

        //close the application when clicking the exit button
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //close application when clicking the windown close "x"
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Application.Exit();
        }

        //indicate a flight is available visually
        private void indicateFlightAvailabe(GroupBox groupBox)
        {
            if (dataGridBookFlightSrch.Rows.Count > 1)
            {
                groupBox.BackColor = Color.MediumAquamarine;

            }
            else
            {
                lbViewFlights.Text = "No available flights";
                groupBox.BackColor = Color.Salmon;
                gbBookingsDepart.BackColor = Color.Salmon;

            }
        }

        //Departure groupbox
        private void btnBookDepartSrch_Click(object sender,  EventArgs e)
        {
            //DB connection
            DBConnection dBConnection = new DBConnection();

            //change flight viw lable to reflect current selection
            lbViewFlights.Text = "Available Flights";

            //query database
            dBConnection.checkAvailablFlights(cmbBookDepartFrom.SelectedValue.ToString(), cmbBookDepartTo.SelectedValue.ToString(), dateBookDepart.Value.ToString("dd/MM/yyyy"), "availDepart");

            //fill the datagrid 
            dataGridBookFlightSrch.DataSource = dBConnection.dataSet.Tables["availDepart"];

            //visual indication of available flights
            indicateFlightAvailabe(gbBookingsDepart);

            //fill departure route object with data and enable the run flights and secondleg
            if (gbBookingsDepart.BackColor.Equals(Color.MediumAquamarine))
            {
                departRoute.flightFrom = cmbBookDepartFrom.Text;
                departRoute.flightTo = cmbBookDepartTo.Text;
                departRoute.flightDate = dateBookDepart.Value.ToString("dd/MM/yyyy");
                departRoute.departTime = dataGridBookFlightSrch.Rows[0].Cells[4].Value.ToString();
                departRoute.arriveTime = dataGridBookFlightSrch.Rows[0].Cells[5].Value.ToString();
                
                chkBoxReturn.Enabled = true;
                chkBoxLeg.Enabled = true;
            }
            else
            {
                chkBoxReturn.Checked = false;
                chkBoxLeg.Checked = false;
                chkBoxReturn.Enabled = false;
                chkBoxLeg.Enabled = false;
            }
            
        }

        //return groupbox
        private void btnBookReturnSrch_Click(object sender, EventArgs e)
        {
            DBConnection dBConnection = new DBConnection();

            lbViewFlights.Text = "Available Flights";
            dBConnection.checkAvailablFlights(cmbBookReturnFrom.SelectedValue.ToString(), cmbBookReturnTo.SelectedValue.ToString(), dateBookReturn.Value.ToString("dd/MM/yyyy"), "availReturn");

            dataGridBookFlightSrch.DataSource = dBConnection.dataSet.Tables["availReturn"];

            indicateFlightAvailabe(gbBookingsReturn);

            if (gbBookingsReturn.BackColor.Equals(Color.MediumAquamarine))
            {
                returnRoute.flightFrom = cmbBookReturnFrom.Text;
                returnRoute.flightTo = cmbBookReturnTo.Text;
                returnRoute.flightDate = dateBookReturn.Value.ToString("dd/MM/yyyy");
                returnRoute.departTime = dataGridBookFlightSrch.Rows[0].Cells[4].Value.ToString();
                returnRoute.arriveTime = dataGridBookFlightSrch.Rows[0].Cells[5].Value.ToString();

            }

        }
        //second leg groupbox
        private void btnBookLegSrch_Click(object sender, EventArgs e)
        {
            DBConnection dBConnection = new DBConnection();



            lbViewFlights.Text = "Available Flights";

            dBConnection.checkAvailablFlights(cmbBookLegFrom.SelectedValue.ToString(), cmbBookLegTo.SelectedValue.ToString(), dateBookLeg.Value.ToString("dd/MM/yyyy"), "availLeg");
            dataGridBookFlightSrch.DataSource = dBConnection.dataSet.Tables["availLeg"];

            indicateFlightAvailabe(gbBookingsLeg);

            if (gbBookingsLeg.BackColor.Equals(Color.MediumAquamarine))
            {
                secondLegRoute.flightFrom = cmbBookLegFrom.Text;
                secondLegRoute.flightTo = cmbBookLegTo.Text;
                secondLegRoute.flightDate = dateBookLeg.Value.ToString("dd/MM/yyyy");
                secondLegRoute.departTime = dataGridBookFlightSrch.Rows[0].Cells[4].Value.ToString();
                secondLegRoute.arriveTime = dataGridBookFlightSrch.Rows[0].Cells[5].Value.ToString();
        
            }

        }

        //method to switch back to current flights on the data grid
        private void btnShowCurrentFlights_Click(object sender, EventArgs e)
        {
            DBConnection dBConnection = new DBConnection();
            lbViewFlights.Text = "Current Flights";
            dBConnection.getAllFlights("currentFlights");
            dataGridBookFlightSrch.DataSource = dBConnection.dataSet.Tables["currentFlights"];
        }

        
        //Create a booking
        private void btnBookFlight_Click(object sender, EventArgs e)
        {
            bool enableBookingConfirmPassenger = false;
            bool enableBookingConfirmRoute =  false;

            //check that the flight are valid routes
            if((gbBookingsDepart.BackColor == Color.Salmon || gbBookingsDepart.BackColor == Color.Transparent))
            {
                MessageBox.Show("Please select a valid flight");
            }

            if((gbBookingsReturn.BackColor.Equals(Color.Transparent) || gbBookingsLeg.BackColor.Equals(Color.Transparent)) && (chkBoxReturn.Checked || chkBoxLeg.Checked))
            {
                MessageBox.Show("Please select a valid flight");
            }

            if (txtBoxFirstNameBook.Text != "" && txtBoxLastNameBook.Text != "" && txtBoxPassportNumBook.Text != "" && txtBoxPhoneNumBook.Text != "" && txtBoxEmailBook.Text != "")
            {

                passenger.FirstName = txtBoxFirstNameBook.Text;
                passenger.LastName = txtBoxLastNameBook.Text;
                passenger.PassportNum = txtBoxPassportNumBook.Text;
                passenger.phoneNumber = txtBoxPhoneNumBook.Text;
                passenger.email = txtBoxEmailBook.Text;
                
            
                enableBookingConfirmPassenger = true;
            }
            else
            {
                MessageBox.Show("Please enter all passenger details");
            }
           
            

            //generate a booking number
            String bookingRefNum = booking.generateBookingNum(8);

            //fill the bookings object with the relavent routes based on flights available that are selected  
            if (gbBookingsDepart.BackColor.Equals(Color.MediumAquamarine) && !gbBookingsReturn.BackColor.Equals(Color.MediumAquamarine) && !gbBookingsLeg.BackColor.Equals(Color.MediumAquamarine))
            {
                booking.flyDepart = departRoute;
                enableBookingConfirmRoute = true;
            }
            if( (gbBookingsDepart.BackColor.Equals(Color.MediumAquamarine) && gbBookingsReturn.BackColor.Equals(Color.MediumAquamarine)))
            {
                booking.flyDepart = departRoute;
                booking.flyReturn = returnRoute;
                enableBookingConfirmRoute = true;
            }
            if ((gbBookingsDepart.BackColor.Equals(Color.MediumAquamarine) && gbBookingsLeg.BackColor.Equals(Color.MediumAquamarine)))
            {
                booking.flyDepart = departRoute;
                booking.flySecondLeg = secondLegRoute;
                enableBookingConfirmRoute = true;
            }

            if ((gbBookingsReturn.BackColor.Equals(Color.Transparent) || gbBookingsLeg.BackColor.Equals(Color.Transparent)) && (chkBoxReturn.Checked || chkBoxLeg.Checked))
            {
                MessageBox.Show("Second leg, or return flight has not been selected ");
                enableBookingConfirmRoute = false;
            }

            if (enableBookingConfirmPassenger == true && enableBookingConfirmRoute == true)
            {
                //New booking Confirmation form object to capture the booking information
                BookingConfirmation bookingConfirmation = new BookingConfirmation();

               
                //populate passenger details from the fields in the main form to the booking confirmation form
                bookingConfirmation.txtBoxPassengerName.Text = passenger.FirstName + " " + passenger.LastName;
                bookingConfirmation.txtBoxPassportNum.Text = passenger.PassportNum;
                bookingConfirmation.txtBoxBookingRefNum.Text = bookingRefNum;
                bookingConfirmation.email = passenger.email;
                bookingConfirmation.phone = passenger.phoneNumber;

                bookingConfirmation.txtBoxDateBooked.Text = todayDate;
            

                //populate the departure flight details from the departure route object to the booking confirmation form
                bookingConfirmation.txtBoxDepartFrom.Text = booking.flyDepart.flightFrom;
                bookingConfirmation.txtBoxDepartFromTime.Text = booking.flyDepart.departTime;
                bookingConfirmation.txtBoxDepartTo.Text = booking.flyDepart.flightTo;
                bookingConfirmation.txtBoxDepartToTime.Text = booking.flyDepart.arriveTime;
                bookingConfirmation.txtBoxDepartDate.Text = booking.flyDepart.flightDate;

                //Populate seat class for depature flight
                if (radioBookEccoDepart.Checked)
                {
                    bookingConfirmation.txtBoxDepartSeatClass.Text = radioBookEccoDepart.Text;

                }else if (radioBookBusinDepart.Checked)
                {
                    bookingConfirmation.txtBoxDepartSeatClass.Text = radioBookBusinDepart.Text;
                }
                else
                {
                    bookingConfirmation.txtBoxDepartSeatClass.Text = radioBookFirstDepart.Text;
                }

                //Display return/second leg booking confirmation form gruopbox is the return or second leg checkboxxes are checked
                if (chkBoxReturn.Checked || chkBoxLeg.Checked)
                {
                    bookingConfirmation.gbBookingConfirmLeg.Visible = true;
                }

                //fill second flight option with return details
                if (chkBoxReturn.Checked && gbBookingsReturn.BackColor.Equals(Color.MediumAquamarine))
                {
                    bookingConfirmation.txtBoxLegFrom.Text = booking.flyReturn.flightFrom;
                    bookingConfirmation.txtBoxLegFromTime.Text = booking.flyReturn.departTime;
                    bookingConfirmation.txtBoxLegTo.Text = booking.flyReturn.flightTo;
                    bookingConfirmation.txtBoxLegToTime.Text = booking.flyReturn.arriveTime;
                    bookingConfirmation.txtBoxLegDate.Text = booking.flyReturn.flightDate;

                    //Populate seat class for depature flight
                    if (radioBookEccoReturn.Checked)
                    {
                        bookingConfirmation.txtBoxLegSeatClass.Text = radioBookEccoReturn.Text;

                    }
                    else if (radioBookBusinReturn.Checked)
                    {
                        bookingConfirmation.txtBoxLegSeatClass.Text = radioBookBusinReturn.Text;
                    }
                    else
                    {
                        bookingConfirmation.txtBoxLegSeatClass.Text = radioBookFirstReturn.Text;
                    }
                }

                //fill second flight option with the secod leg details
                if (chkBoxLeg.Checked && gbBookingsLeg.BackColor.Equals(Color.MediumAquamarine))
                {
                    //Change lable to show correct flight type (from return flight to second leg)
                    bookingConfirmation.lbBookingConfirnLeg.Text = "Second Leg";

                    bookingConfirmation.txtBoxLegFrom.Text = booking.flySecondLeg.flightFrom;
                    bookingConfirmation.txtBoxLegFromTime.Text = booking.flySecondLeg.arriveTime;
                    bookingConfirmation.txtBoxLegTo.Text = booking.flySecondLeg.flightTo;
                    bookingConfirmation.txtBoxLegToTime.Text = booking.flySecondLeg.arriveTime;
                    bookingConfirmation.txtBoxLegDate.Text = booking.flySecondLeg.flightDate;

                    //Populate seat class for depature flight
                    if (radioBookEccoLeg.Checked)
                    {
                        bookingConfirmation.txtBoxLegSeatClass.Text = radioBookEccoLeg.Text;

                    }
                    else if (radioBookBusinLeg.Checked)
                    {
                        bookingConfirmation.txtBoxLegSeatClass.Text = radioBookBusinLeg.Text;
                    }
                    else
                    {
                        bookingConfirmation.txtBoxLegSeatClass.Text = radioBookFirstLeg.Text;
                    }
                }

                bookingConfirmation.Show();
            }
        }


        //Add airports 
        private void btnAddAirport_Click(object sender, EventArgs e)
        {
            if(txtBoxAddAirportName.Text != "" && txtBoxAddAirportCode.Text != "")
            {
                DBConnection dBConnection = new DBConnection();

                string airportGuid = Guid.NewGuid().ToString();

                dBConnection.insertIntoAirports(airportGuid, cmbSelectCountry.SelectedValue.ToString(), txtBoxAddAirportCode.Text, txtBoxAddAirportName.Text);

                MessageBox.Show("Airport Added");


                //tables to repopulate the combo boxes and datagrids
                dBConnection.getAllAirports("AdminAirportsList");
                dBConnection.getAirportsCodes("AdinAirportCodes");

                //get airport for admin tab
                dataGridListOfAirports.DataSource = dBConnection.dataSet.Tables["AdminAirportsList"];
                cmbDeleteAirport.DataSource = dBConnection.dataSet.Tables["AdinAirportCodes"];
                cmbDeleteAirport.DisplayMember = "airport_code";


                //Reset fleilds
                txtBoxAddCountry.Text = "";
                txtBoxAddAirportCode.Text = "";
                txtBoxAddAirportCode.Text = "";

                //methods to get all airports and airprt codes
                dBConnection.getAirportsCodes("DeparturesFrom");
                dBConnection.getAirportsCodes("DeparturesTo");
                dBConnection.getAirportsCodes("SecondLegTo");

                //mehod to get all country names for the admin tab
                dBConnection.getAllCountryNames("CountryNames");

                cmbSelectCountry.DataSource = dBConnection.dataSet.Tables["countryNames"];
                cmbSelectCountry.DisplayMember = "country_name";
                cmbSelectCountry.ValueMember = "guid";

                //Fill departures Comboxes
                cmbBookDepartFrom.DataSource = dBConnection.dataSet.Tables["DeparturesFrom"];
                cmbBookDepartFrom.DisplayMember = "airport_code";
                cmbBookDepartFrom.ValueMember = "guid";

                cmbBookDepartTo.DataSource = dBConnection.dataSet.Tables["DeparturesTo"];
                cmbBookDepartTo.DisplayMember = "airport_code";
                cmbBookDepartTo.ValueMember = "guid";

                //comboboxes for return flights will always be the reverse of deparute flights
                cmbBookReturnFrom.DataSource = dBConnection.dataSet.Tables["DeparturesTo"];
                cmbBookReturnFrom.DisplayMember = "airport_code";
                cmbBookReturnFrom.ValueMember = "guid";

                cmbBookReturnTo.DataSource = dBConnection.dataSet.Tables["DeparturesFrom"];
                cmbBookReturnTo.DisplayMember = "airport_code";
                cmbBookReturnTo.ValueMember = "guid";

                //Combox for Leg from will always equal destination od departure 
                cmbBookLegFrom.DataSource = dBConnection.dataSet.Tables["DeparturesTo"];
                cmbBookLegFrom.DisplayMember = "airport_code";
                cmbBookLegFrom.ValueMember = "guid";

                //Combox for secdond leg destination
                cmbBookLegTo.DataSource = dBConnection.dataSet.Tables["SecondLegTo"];
                cmbBookLegTo.DisplayMember = "airport_code";
                cmbBookLegTo.ValueMember = "guid";

                dBConnection.getAllFlights("currentFlights");
                dataGridBookFlightSrch.DataSource = dBConnection.dataSet.Tables["currentFlights"];
            }
            else
            {
                MessageBox.Show("Please enter all fields to the Add Airport");
            }
            

        }

        //delete airports
        private void btnDeleteAirport_Click(object sender, EventArgs e)
        {
            //new connection
            DBConnection dBConnection = new DBConnection();

            dBConnection.deleteAirport(cmbDeleteAirport.Text);

            MessageBox.Show("Airport Deleted");
      
            //tables to repopulate the combo boxes and datagrids
            dBConnection.getAllAirports("AdminAirportsList");
            dBConnection.getAirportsCodes("AdinAirportCodes");

            //get airport for admin tab
            dataGridListOfAirports.DataSource = dBConnection.dataSet.Tables["AdminAirportsList"];
            cmbDeleteAirport.DataSource = dBConnection.dataSet.Tables["AdinAirportCodes"];
            cmbDeleteAirport.DisplayMember = "airport_code";
        }

       

        private void btnAddcountry_Click(object sender, EventArgs e)
        {
            if(txtBoxAddCountry.Text != "")
            {
                DBConnection dBConnection = new DBConnection();

                string countryGuid = Guid.NewGuid().ToString();

                dBConnection.insertIntoCountry(countryGuid, txtBoxAddCountry.Text);

                MessageBox.Show("Country Added");

                dBConnection.getAllAirports("AdminAirportsList");
                dBConnection.getAirportsCodes("AdinAirportCodes");
                dBConnection.getAllCountryNames("countryNames");

                //update airport for admin tab 
                dataGridListOfAirports.DataSource = dBConnection.dataSet.Tables["AdminAirportsList"];
                cmbDeleteAirport.DataSource = dBConnection.dataSet.Tables["AdinAirportCodes"];
                cmbDeleteAirport.DisplayMember = "airport_code";

                cmbSelectCountry.DataSource = dBConnection.dataSet.Tables["countryNames"];
                cmbSelectCountry.DisplayMember = "country_name";
                cmbSelectCountry.ValueMember = "guid";

                //Reset fleilds
                txtBoxAddCountry.Text = "";
                txtBoxAddAirportCode.Text = "";
                txtBoxAddAirportCode.Text = "";
            }
            else
            {
                MessageBox.Show("Please enter a country name");
            }
            
        }

       

        private void btnViewBookingSearch_Click(object sender, EventArgs e)
        {
            DBConnection dBConnection = new DBConnection();

            dBConnection.getSelectedBookings("currentBooking", cmbBookingRefNum.Text);
            txtBoxPsngFirstName.Text = dBConnection.dataSet.Tables["currentBooking"].Rows[0]["First Name"].ToString();
            txtBoxPsngLastName.Text = dBConnection.dataSet.Tables["currentBooking"].Rows[0]["Last Name"].ToString();

            string flightStringFromDatabase = dBConnection.dataSet.Tables["currentBooking"].Rows[0]["flight"].ToString();


            txtBoxPsngFlight.Text = dBConnection.dataSet.Tables["currentBooking"].Rows[0]["flight"].ToString();
            txtBoxPasngBookingRefNum.Text = dBConnection.dataSet.Tables["currentBooking"].Rows[0]["Ref Num"].ToString();

            dBConnection.getAllBooking("AllBookings");
            cmbBookingRefNum.DataSource = dBConnection.dataSet.Tables["AllBookings"];
            cmbBookingRefNum.DisplayMember = "Ref Num";
            cmbBookingRefNum.ValueMember = "Ref Num";
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            
            System.Diagnostics.Process.Start(@"C:\data\cyanair help files\cyanairHelp.html");
        }

        private void btnClearFields_Click(object sender, EventArgs e)
        {
            txtBoxFirstNameBook.Text = "";
            txtBoxLastNameBook.Text = "";
            txtBoxPassportNumBook.Text = "";
            txtBoxPhoneNumBook.Text = "";
            txtBoxEmailBook.Text = "";
        }

        //test
      
    }
}
