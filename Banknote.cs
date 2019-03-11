using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace ICTL77F
{
    public class Banknote : IBanknote
    {
        private SerialPort serial;

        private event EventHandler EventOnReadBankNote;
        private event EventHandler EventOnReadStatus;

        public Banknote(EventHandler hBill, EventHandler hStatus, string PortName, int BaudRate = 9600)
        {
            serial = new SerialPort
            {
                PortName = PortName,
                BaudRate = BaudRate,
                StopBits = StopBits.One,
                DataBits = 8,
                Parity = Parity.Even


            };
            serial.DataReceived += new SerialDataReceivedEventHandler(Serialport_DataReceived);
            EventOnReadBankNote += hBill;
            EventOnReadStatus += hStatus;
        }

        private void Serialport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serial.IsOpen)
            {
                try
                {
                    Thread.Sleep(1000);
                    if (serial.BytesToRead > 0)
                    {
                        int bytes = serial.BytesToRead;
                        byte[] byteData = new byte[bytes];
                        serial.Read(byteData, 0, bytes);

                        string hexString = BitConverter.ToString(byteData);
                        string data = hexString.Replace("-", "").Trim().ToUpper();

                        if (!String.IsNullOrEmpty(data))
                        {
                            Console.WriteLine(data);

                            try
                            {

                                if (data.ToUpper() == "808F")  //Reset and Power Up
                                {
                                    try
                                    {
                                        byte[] bytestosend = { 0x02 };

                                        serial.Write(bytestosend, 0, bytestosend.Length);
                                    }
                                    catch { }
                                }
                                else if (data.ToUpper().Substring(0, 2) == "81")
                                {


                                    if (data == "8140")
                                    {
                                        RaiseEventRead("20");
                                    }
                                    else if (data == "8141")
                                    {
                                        RaiseEventRead("50");
                                    }
                                    else if (data == "8142")
                                    {
                                        RaiseEventRead("100");
                                    }
                                    else if (data == "8143")
                                    {
                                        RaiseEventRead("500");
                                    }
                                    else if (data == "8144")
                                    {
                                        RaiseEventRead("1000");
                                    }
                                    byte[] bytestosend = { 0x02 };

                                    serial.Write(bytestosend, 0, bytestosend.Length);
                                }
                                else
                                {
                                    RaiseEventStatus(data);
                                }
                            }
                            catch { }
                        }


                    }

                }
                catch { }
            }
        }

        private void RaiseEventStatus(string status)
        {
            BILL_STATUS cashStatus = BILL_STATUS.ENABLED;
            switch (status)
            {
                case "3E":  //ACK  ACCEPTED RESPONSE
                    cashStatus = BILL_STATUS.ENABLED;
                    break;
                case "5E":
                    cashStatus = BILL_STATUS.DISABLED;
                    break;
                case "10":
                    break;
                case "11":
                    break;
                /*
                20 Motor Failure
                21 Checksum Error
                22 Bill Jam
                23 Bill Remove
                24 Stacker Open
                25 Sensor Problem
                27 Bill Fish
                28 Stacker Problem
                29 Bill Reject
                2A Invalid Command
                2E Reserved
                2F Response when Error Status is Exclusion
                */
                default:
                    cashStatus = BILL_STATUS.ERROR;
                    break;
            }
            if (this.EventOnReadStatus != null)
            {
                EventArgsBankNoteStatus eventArgsReadStatus = new EventArgsBankNoteStatus(cashStatus);
                this.EventOnReadStatus(this, eventArgsReadStatus);
            }
        }

        public void Close()
        {
            try
            {
                if (serial.IsOpen)
                {
                    try
                    {
                        byte[] bytestosend = { 0x5E };

                        serial.Write(bytestosend, 0, bytestosend.Length);
                    }
                    catch { }
                    serial.Close();
                }
            }
            catch { }
        }

        public bool Open()
        {
            if (!serial.IsOpen)
            {
                try
                {
                    serial.Open();

                    if (serial.IsOpen)
                    {
                        try
                        {
                            byte[] bytestosend = { 0x02 };

                            serial.Write(bytestosend, 0, bytestosend.Length);
                        }
                        catch { }
                        try
                        {
                            byte[] bytestosend = { 0x3E };

                            serial.Write(bytestosend, 0, bytestosend.Length);
                        }
                        catch { }
                        return true;
                    }
                }
                catch { }
            }
            return false;

        }
        private void RaiseEventRead(string banknote)
        {
            if (this.EventOnReadBankNote != null)
            {
                EventArgsReadBankNote eventArgsReadBankNote = new EventArgsReadBankNote
                {
                    BankNote = banknote
                };
                this.EventOnReadBankNote(this, eventArgsReadBankNote);
            }
        }
        public bool FireReadStatus()
        {
            if (serial.IsOpen)
            {
                try
                {

                    byte[] bytestosend = { 0x0C };

                    serial.Write(bytestosend, 0, bytestosend.Length);
                }
                catch { }
                return true;
            }
            return false;
        }

        public void Reset()
        {
            try
            {
                if (serial.IsOpen)
                {
                    try
                    {
                        byte[] bytestosend = { 0x02 };

                        serial.Write(bytestosend, 0, bytestosend.Length);
                    }
                    catch { }
                }
            }
            catch { }
        }

        public void Reject()
        {
            try
            {
                if (serial.IsOpen)
                {
                    try
                    {
                        byte[] bytestosend = { 0x8F };

                        serial.Write(bytestosend, 0, bytestosend.Length);
                    }
                    catch { }
                }
            }
            catch { }
        }

        public void Accept()
        {
            try
            {
                if (serial.IsOpen)
                {
                    try
                    {
                        byte[] bytestosend = { 0x02 };

                        serial.Write(bytestosend, 0, bytestosend.Length);
                    }
                    catch { }
                }
            }
            catch { }
        }

        public void Hold()
        {
            try
            {
                if (serial.IsOpen)
                {
                    try
                    {
                        byte[] bytestosend = { 0x18 };

                        serial.Write(bytestosend, 0, bytestosend.Length);
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
