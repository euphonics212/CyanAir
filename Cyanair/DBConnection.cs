using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;


namespace Cyanair
{
    public class DBConnection
    {
        static string connString = "Data Source = C:/data/cyanair.db"; // db file path

        public SQLiteConnection cyanairDBconn = new SQLiteConnection(connString); //create SQlite onject

        public DataSet dataSet = new DataSet(); // create a data set to store the db tables

        public DataTable dataTable = new DataTable(); // create datatable to store individual tables

        public SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(); // create SQlite data adapter
       
        //check the db file exist 
        public bool validateDBfileExist()
        {
            if(!File.Exists("C:/data/cyanair.db"))
            {
                MessageBox.Show("Database could not be found");
                return false;
            }
            else
            {
                return true;
            }
        }

        //open db connection if it is closed, and not if it is open
        public void openDbConnection()
        {
            if(cyanairDBconn.State != System.Data.ConnectionState.Open)
            {
                cyanairDBconn.Open();
            }
        }

        //close db connection if it is open and not if it closed
        public void closeDbConnection()
        {
            if (cyanairDBconn.State != System.Data.ConnectionState.Closed)
            {
                cyanairDBconn.Close();
            }
        }

        //method to get all airports
        public void getAirportsCodes(string tableName)
        {
            string cmdString = "SELECT * FROM Airports ORDER BY airport_code;";// SQL command
            dataAdapter = new SQLiteDataAdapter(cmdString, cyanairDBconn);
            dataAdapter.Fill(dataSet, tableName);
            
        }
        public void getAllAirports(string tableName)
        {
            string cmdString = @"SELECT airport_name AS 'Airport Name',
                                        airport_code AS 'Airport Code',
		                                country_name AS 'Country Name'
                                FROM Airports, Country
                                WHERE
                                        country_guid = Country.guid
                                        ORDER BY
                                        country_name; ";// SQL command
            dataAdapter = new SQLiteDataAdapter(cmdString, cyanairDBconn);
            dataAdapter.Fill(dataSet, tableName);
        }

        //method to get all users, booking agents with and without admin rights 
       public void getAllUsers(string tableName)
       {
            string cmdString = "SELECT * FROM BookingAgents;";// SQL command
            dataAdapter = new SQLiteDataAdapter(cmdString, cyanairDBconn);
            dataAdapter.Fill(dataSet, tableName);

       }
       public void getAllFlights(string tableName)
        {
            string cmdString = @"SELECT DepAirport.airport_code AS DeptureAirport, 
                                        DestAirport.airport_code AS DestinationAirport, 
		                                CountryDept.country_name AS DeptCntry,
		                                CountryDest.country_name AS DestCntry,
		                                route.depart_time AS 'Depart Time', 
                                        Route.arrive_time AS 'Arrive Time',
                                        flight_date AS Date
                                FROM Route
                                        LEFT JOIN Airports AS DepAirport ON  DepAirport.guid = depart_guid
                                        LEFT JOIN Airports AS DestAirport ON DestAirport.guid = dest_guid
                                        LEFT JOIN Country AS CountryDept ON     
                                        LEFT JOIN Country AS CountryDest ON CountryDest.guid = DestAirport.country_guid; ";
            dataAdapter = new SQLiteDataAdapter(cmdString, cyanairDBconn);
            dataAdapter.Fill(dataSet, tableName);
        }

        //method to check all available flights in the database
        public void checkAvailablFlights(string flightFrom, string flightTo, string flightDate, string tableName)
        {
           
            string cmdString = @"SELECT DepAirport.airport_code AS DeptureAirport, 
                                        DestAirport.airport_code AS DestinationAirport, 
		                                CountryDept.country_name AS DeptCntry,
		                                CountryDest.country_name AS DestCntry,
		                                route.depart_time AS 'Depart Time', 
                                        Route.arrive_time AS 'Arrive Time',
                                        flight_date AS Date
                                FROM Route
                                        LEFT JOIN Airports AS DepAirport ON  DepAirport.guid = depart_guid
                                        LEFT JOIN Airports AS DestAirport ON DestAirport.guid = dest_guid
                                        LEFT JOIN Country AS CountryDept ON CountryDept.guid = DepAirport.country_guid
                                        LEFT JOIN Country AS CountryDest ON CountryDest.guid = DestAirport.country_guid																									
                                WHERE 
                                        depart_guid = '" + flightFrom + "' AND dest_guid = '"+ flightTo + "' AND flight_date = '" + flightDate + "' AND depart_time; ";
            dataAdapter = new SQLiteDataAdapter(cmdString, cyanairDBconn);
            dataAdapter.Fill(dataSet, tableName);
        }
       
        //method to insert a passenger into the database
       public void insertIntoPassenger(string psngGuid, string firstname, string lastname, string email, string phone, string passport)
       {
            string cmdString = @"INSERT INTO Passengers (guid, passenger_firstname, passenger_lastname, email, phone_num, passport_num)
                                            VALUES('" + psngGuid + "', '" + 
                                                        firstname + "', '" + 
                                                        lastname + "', '" + 
                                                        email + "', '" + 
                                                        phone + "', '" + 
                                                        passport + "');";

            SQLiteCommand sQLiteCommand = new SQLiteCommand(cmdString, cyanairDBconn);
            openDbConnection();
            sQLiteCommand.ExecuteNonQuery();
        }


        //method to get all country names
        public void getAllCountryNames(string tableName)
        {             
            string cmdString = @" SELECT guid, country_name FROM Country; ";
            dataAdapter = new SQLiteDataAdapter(cmdString, cyanairDBconn);
            dataAdapter.Fill(dataSet, tableName);
        }

        //method to insert into bookings
        public void insertIntoBookings(string guid, string bookingRefNum, string passengerGuid, string flight, string dateBooked)
        {
            string cmdString = @"INSERT INTO Bookings (guid, booking_ref_num, passenger_guid, flight, date_booked) 
	                                    VALUES ('" + guid + "', '" +
                                                     bookingRefNum + "', " +
                                       "(SELECT guid from Passengers WHERE guid='" + passengerGuid + "'), '" +
                                                    flight + "', '" +
                                                    dateBooked + "' );";

            SQLiteCommand sQLiteCommand = new SQLiteCommand(cmdString, cyanairDBconn);
            openDbConnection();
            sQLiteCommand.ExecuteNonQuery();
        }


        //method to insert country into country table
        public void insertIntoCountry(string guid, string countryName)
        {
            string cmdString = @"INSERT INTO country(guid, country_name)
		                                VALUES('" + guid + "', '" +
                                                    countryName + "');";

            SQLiteCommand sQLiteCommand = new SQLiteCommand(cmdString, cyanairDBconn);
            openDbConnection();
            sQLiteCommand.ExecuteNonQuery();
           
        }

        //method to insert airports into airport table
        public void insertIntoAirports(string guid, string countryGuid, string airportCode,string  airportName)
        {
            string cmdString = @"INSERT INTO Airports (guid, country_guid, airport_code, airport_name) 
	                                    VALUES ('" + guid + "', " +
                                        "(SELECT guid from Country WHERE guid='"+ countryGuid +  "'), '" + 
                                                     airportCode + "', '" +
                                                     airportName + "' );";

            SQLiteCommand sQLiteCommand = new SQLiteCommand(cmdString, cyanairDBconn);
            openDbConnection();
            sQLiteCommand.ExecuteNonQuery();
        }

        //method to delete airports
        public void deleteAirport(string airportCode)
        {
            string cmdString = @"DELETE FROM Airports
                                 WHERE airport_code = '"+ airportCode + "';";

            SQLiteCommand sQLiteCommand = new SQLiteCommand(cmdString, cyanairDBconn);
            openDbConnection();
            sQLiteCommand.ExecuteNonQuery();
        }

        public void getAllBookingsDataGrid(string tableName)
        {
            string cmdString = @"SELECT booking_ref_num AS 'Ref Num', passenger_firstname AS 'First Name', passenger_lastname AS 'Last Name'
                                        FROM Bookings
                                        LEFT JOIN Passengers ON Passengers.guid = passenger_guid ";// SQL command
            dataAdapter = new SQLiteDataAdapter(cmdString, cyanairDBconn);
            dataAdapter.Fill(dataSet, tableName);
        }

        public void getAllBooking(string tableName)
        {
            string cmdString = @"SELECT Bookings.guid, booking_ref_num AS 'Ref Num', passenger_firstname AS 'First Name', passenger_lastname AS 'Last Name', flight AS Flight
                                        FROM Bookings
                                        LEFT JOIN Passengers ON Passengers.guid = passenger_guid";// SQL command
            dataAdapter = new SQLiteDataAdapter(cmdString, cyanairDBconn);
            dataAdapter.Fill(dataSet, tableName);
        }

        public void getSelectedBookings(string tableName, string bookingRefNum)
        {
            string cmdString = @"SELECT Bookings.guid, booking_ref_num AS 'Ref Num', passenger_firstname AS 'First Name', passenger_lastname AS 'Last Name', flight AS 'Flight'
                                        FROM Bookings
                                        LEFT JOIN Passengers ON Passengers.guid = passenger_guid
										WHERE (booking_ref_num = '" + bookingRefNum +"')";// SQL command
            dataAdapter = new SQLiteDataAdapter(cmdString, cyanairDBconn);
            dataAdapter.Fill(dataSet, tableName);
        }
    }
}
