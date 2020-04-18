/**********************************************************
 * Demo for Standalone SDK.Created by Darcy on Oct.15 2009*
***********************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;  //Its for MySQL 


namespace AttLogs
{
    public partial class AttLogsMain : Form
    {
        public AttLogsMain()
        {
            InitializeComponent();
        }

        //Create Standalone SDK class dynamicly.
        public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();

        /********************************************************************************************************************************************
        * Before you refer to this demo,we strongly suggest you read the development manual deeply first.                                           *
        * This part is for demonstrating the communication with your device.There are 3 communication ways: "TCP/IP","Serial Port" and "USB Client".*
        * The communication way which you can use duing to the model of the device.                                                                 *
        * *******************************************************************************************************************************************/
        #region Communication
        private bool bIsConnected = false;//the boolean value identifies whether the device is connected
        private int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.

        //If your device supports the TCP/IP communications, you can refer to this.
        //when you are using the tcp/ip communication,you can distinguish different devices by their IP address.
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (txtIP.Text.Trim() == "" || txtPort.Text.Trim() == "")
            {
                MessageBox.Show("IP and Port cannot be null", "Error");
                return;
            }
            int idwErrorCode = 0;

            Cursor = Cursors.WaitCursor;
            if (btnConnect.Text == "DisConnect")
            {
                axCZKEM1.Disconnect();
                bIsConnected = false;
                btnConnect.Text = "Connect";
                lblState.Text = "Current State:DisConnected";
                Cursor = Cursors.Default;
                return;
            }

            bIsConnected = axCZKEM1.Connect_Net(txtIP.Text, Convert.ToInt32(txtPort.Text));
            if (bIsConnected == true)
            {
                btnConnect.Text = "DisConnect";
                btnConnect.Refresh();
                lblState.Text = "Current State:Connected";
                iMachineNumber = 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                axCZKEM1.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                MessageBox.Show("Unable to connect the device,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            Cursor = Cursors.Default;
        }

        //If your device supports the SerialPort communications, you can refer to this.
        private void btnRsConnect_Click(object sender, EventArgs e)
        {
            if (cbPort.Text.Trim() == "" || cbBaudRate.Text.Trim() == "" || txtMachineSN.Text.Trim() == "")
            {
                MessageBox.Show("Port,BaudRate and MachineSN cannot be null", "Error");
                return;
            }
            int idwErrorCode = 0;
            //accept serialport number from string like "COMi"
            int iPort;
            string sPort = cbPort.Text.Trim();
            for (iPort = 1; iPort < 10; iPort++)
            {
                if (sPort.IndexOf(iPort.ToString()) > -1)
                {
                    break;
                }
            }

            Cursor = Cursors.WaitCursor;
            if (btnRsConnect.Text == "Disconnect")
            {
                axCZKEM1.Disconnect();
                bIsConnected = false;
                btnRsConnect.Text = "Connect";
                btnRsConnect.Refresh();
                lblState.Text = "Current State:Disconnected";
                Cursor = Cursors.Default;
                return;
            }

            iMachineNumber = Convert.ToInt32(txtMachineSN.Text.Trim());//when you are using the serial port communication,you can distinguish different devices by their serial port number.
            bIsConnected = axCZKEM1.Connect_Com(iPort, iMachineNumber, Convert.ToInt32(cbBaudRate.Text.Trim()));

            if (bIsConnected == true)
            {
                btnRsConnect.Text = "Disconnect";
                btnRsConnect.Refresh();
                lblState.Text = "Current State:Connected";

                axCZKEM1.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                MessageBox.Show("Unable to connect the device,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }

            Cursor = Cursors.Default;
        }

        //If your device supports the USBCLient, you can refer to this.
        //Not all series devices can support this kind of connection.Please make sure your device supports USBClient.
        //Connect the device via the virtual serial port created by USBClient
        private void btnUSBConnect_Click(object sender, EventArgs e)
        {
            int idwErrorCode = 0;

            Cursor = Cursors.WaitCursor;

            if (btnUSBConnect.Text == "Disconnect")
            {
                axCZKEM1.Disconnect();
                bIsConnected = false;
                btnUSBConnect.Text = "Connect";
                btnUSBConnect.Refresh();
                lblState.Text = "Current State:Disconnected";
                Cursor = Cursors.Default;
                return;
            }

            SearchforUSBCom usbcom = new SearchforUSBCom();
            string sCom = "";
            bool bSearch = usbcom.SearchforCom(ref sCom);//modify by Darcy on Nov.26 2009
            if (bSearch == false)//modify by Darcy on Nov.26 2009
            {
                MessageBox.Show("Can not find the virtual serial port that can be used", "Error");
                Cursor = Cursors.Default;
                return;
            }

            int iPort;
            for (iPort = 1; iPort < 10; iPort++)
            {
                if (sCom.IndexOf(iPort.ToString()) > -1)
                {
                    break;
                }
            }

            iMachineNumber = Convert.ToInt32(txtMachineSN2.Text.Trim());
            if (iMachineNumber == 0 || iMachineNumber > 255)
            {
                MessageBox.Show("The Machine Number is invalid!", "Error");
                Cursor = Cursors.Default;
                return;
            }

            int iBaudRate = 115200;//115200 is one possible baudrate value(its value cannot be 0)
            bIsConnected = axCZKEM1.Connect_Com(iPort, iMachineNumber, iBaudRate);

            if (bIsConnected == true)
            {
                btnUSBConnect.Text = "Disconnect";
                btnUSBConnect.Refresh();
                lblState.Text = "Current State:Connected";
                axCZKEM1.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                MessageBox.Show("Unable to connect the device,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }

            Cursor = Cursors.Default;
        }

        #endregion

        /*************************************************************************************************
        * Before you refer to this demo,we strongly suggest you read the development manual deeply first.*
        * This part is for demonstrating operations with(read/get/clear) the attendance records.         *
        * ************************************************************************************************/
        #region AttLogs

        //Download the attendance records from the device(For both Black&White and TFT screen devices).
        private void btnGetGeneralLogData_Click(object sender, EventArgs e)
        {
            if (bIsConnected == false)
            {
                MessageBox.Show("Please connect the device first", "Error");
                return;
            }

            string sdwEnrollNumber = "";
          //  int idwTMachineNumber = 0;
          //  int idwEMachineNumber = 0;
            int idwVerifyMode = 0;
            int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            int idwWorkcode = 0;

            int idwErrorCode = 0;
          //  int iGLCount = 0;
          //  int iIndex = 0;


            //This is my connection string i have assigned the database file address path  
            // MySqlConnection connection;
            // string cs = "datasource=localhost;port=3306;username=root;password=";            
            // connection = new MySqlConnection(cs);
            //   connection.Open();

            //     MySqlCommand command = new MySqlCommand();






            string MyConnection2 = "datasource=localhost;port=3306;username=root;password=";
            //This is my insert query in which i am taking input from the user through windows forms  

















            Cursor = Cursors.WaitCursor;
            lvLogs.Items.Clear();
            axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device
            if (axCZKEM1.ReadGeneralLogData(iMachineNumber))//read all the attendance records to the memory
            {
                while (axCZKEM1.SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out idwVerifyMode,
                           out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                {







                    string Query = "insert into hms.attn_log_temp(user_e_idd,attn_log_date,attn_time) values('" + sdwEnrollNumber + "', '" + idwYear.ToString() + "-" + idwMonth.ToString("D2") + "-" + idwDay.ToString("D2") + "', '" + idwHour.ToString("D2") + ":" + idwMinute.ToString("D2") + ":" + idwSecond.ToString("D2") + "');";
                    //This is  MySqlConnection here i have created the object and pass my connection string.  
                    MySqlConnection MyConn2 = new MySqlConnection(MyConnection2);
                    //This is command class which will handle the query and connection object.  
                    MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
                    MySqlDataReader MyReader2;
                    MyConn2.Open();
                    MyReader2 = MyCommand2.ExecuteReader();     // Here our query will be executed and data saved into the database.  

                    MyConn2.Close();









                    /**
                    iGLCount++;
                    lvLogs.Items.Add(iGLCount.ToString());
                    lvLogs.Items[iIndex].SubItems.Add(sdwEnrollNumber);//modify by Darcy on Nov.26 2009
                    lvLogs.Items[iIndex].SubItems.Add(idwVerifyMode.ToString());
                    lvLogs.Items[iIndex].SubItems.Add(idwInOutMode.ToString());
                    lvLogs.Items[iIndex].SubItems.Add(idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                    lvLogs.Items[iIndex].SubItems.Add(idwWorkcode.ToString());
                    iIndex++; 
                     **/

                    // command.CommandText = "insert into hms.attn_log_temp(user_e_idd,attn_log_date,attn_time)"+"VALUES(@emp_id, @date_log, @time_log);";
                    //This is  MySqlConnection here i have created the object and pass my connection string.


                    //    command.Parameters["emp_id"].Value = sdwEnrollNumber;
                    //   command.Parameters["date_log"].Value = idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString();
                    //   command.Parameters["time_log"].Value = idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString();

                    //                   command.Connection = connection;

                    //        command.ExecuteNonQuery();


                    /**
                    MySqlConnection MyConn2 = new MySqlConnection(MyConnection2);
                    //This is command class which will handle the query and connection object.  
                    MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
                    MySqlDataReader MyReader2;
                    MyConn2.Open();
                    MyReader2 = MyCommand2.ExecuteReader();  **/
                    // Here our query will be executed and data saved into the database.  

                }
                
                lvLogs.Items.Clear();
                axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device
                if (axCZKEM1.ClearGLog(iMachineNumber))
                {
                    axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
                    MessageBox.Show("All Attendance Download and Clear form Matchine!", "Success");
                }
                
            }
            else
            {
                Cursor = Cursors.Default;
                axCZKEM1.GetLastError(ref idwErrorCode);

                if (idwErrorCode != 0)
                {
                    MessageBox.Show("Reading data from terminal failed,ErrorCode: " + idwErrorCode.ToString(), "Error");
                }
                else
                {
                    MessageBox.Show("No data from terminal returns!", "Error");
                }
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device
            Cursor = Cursors.Default;
        }

        //Clear all attendance records from terminal (For both Black&White and TFT screen devices).
        private void btnClearGLog_Click(object sender, EventArgs e)
        {
            if (bIsConnected == false)
            {
                MessageBox.Show("Please connect the device first", "Error");
                return;
            }
            int idwErrorCode = 0;

            lvLogs.Items.Clear();
            axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device
            if (axCZKEM1.ClearGLog(iMachineNumber))
            {
                axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
                MessageBox.Show("All att Logs have been cleared from teiminal!", "Success");
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device
        }

        //Get the count of attendance records in from ternimal(For both Black&White and TFT screen devices).
        private void btnGetDeviceStatus_Click(object sender, EventArgs e)
        {
            if (bIsConnected == false)
            {
                MessageBox.Show("Please connect the device first", "Error");
                return;
            }
            int idwErrorCode = 0;
            int iValue = 0;

            axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device
            if (axCZKEM1.GetDeviceStatus(iMachineNumber, 6, ref iValue)) //Here we use the function "GetDeviceStatus" to get the record's count.The parameter "Status" is 6.
            {
                MessageBox.Show("The count of the AttLogs in the device is " + iValue.ToString(), "Success");
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device
        }
        #endregion

        private void lvLogs_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}