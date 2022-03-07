//tabs=4
// --------------------------------------------------------------------------------
// TODO fill in this information for your driver, then remove this line!
//
// ASCOM Focuser driver for Homebrew.Skywatcher
//
// Description:	Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam 
//				nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam 
//				erat, sed diam voluptua. At vero eos et accusam et justo duo 
//				dolores et ea rebum. Stet clita kasd gubergren, no sea takimata 
//				sanctus est Lorem ipsum dolor sit amet.
//
// Implements:	ASCOM Focuser interface version: 1.0
// Author:		(XXX) Your N. Here <your@email.here>
//
// Edit Log:
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// dd-mmm-yyyy	XXX	1.0.0	Initial edit, from ASCOM Focuser Driver template
// --------------------------------------------------------------------------------
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ASCOM;
using ASCOM.DeviceInterface;

using System.Diagnostics;
using ASCOM.Utilities;

namespace ASCOM.Homebrew.Skywatcher
{
    //
    // Your driver's ID is ASCOM.Homebrew.Skywatcher.Focuser
    //
    // The Guid attribute sets the CLSID for ASCOM.Homebrew.Skywatcher.Focuser
    // The ClassInterface/None addribute prevents an empty interface called
    // _Focuser from being created and used as the [default] interface
    //
    [Guid("57010fb5-21b9-4ed2-b899-df7de571414a")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Focuser :  IFocuserV2
    {
        //
        // Driver ID and descriptive string that shows in the Chooser
        //
        private static string s_csDriverID = "ASCOM.Homebrew.Skywatcher.Focuser";
        // TODO Change the descriptive string for your driver then remove this line
        private static string s_csDriverDescription = "Homebrew.Skywatcher Focuser";


        FocusController _controller;
        Profile _profile;
        private bool _reversed;
        private bool _absolute;

        //
        // Constructor - Must be public for COM registration!
        //
        public Focuser()
        {
            _profile = new Profile();
            _profile.DeviceType = "Focuser";
            SetFlags();
        }

        #region ASCOM Registration
        //
        // Register or unregister driver for ASCOM. This is harmless if already
        // registered or unregistered. 
        //
        private static void RegUnregASCOM(bool bRegister)
        {
            Profile P = new Profile();
            P.DeviceType = "Focuser";					//  Requires Helper 5.0.3 or later
            if (bRegister)
                P.Register(s_csDriverID, s_csDriverDescription);
            else
                P.Unregister(s_csDriverID);
            try										// In case Helper becomes native .NET
            {
                P.Dispose();
            }
            catch (Exception) { }
            P = null;
        }

        [ComRegisterFunction]
        public static void RegisterASCOM(Type t)
        {
            RegUnregASCOM(true);
        }

        [ComUnregisterFunction]
        public static void UnregisterASCOM(Type t)
        {
            RegUnregASCOM(false);
        }
        #endregion


        #region IFocuser Members

        public ArrayList SupportedActions { get; private set; }

        public bool Absolute
        {
            get
            {
                return false;
            }
        }

        public void Dispose()
        {
            
        }

        public void Halt()
        {
            if (!Link)
                throw new InvalidOperationException("Focuser link not activated");
            _controller.Halt();
        }

        public bool IsMoving
        {
            get
            {
                return _controller.IsMoving();
            }
        }

        public bool Link
        {
            get
            {
                return (_controller != null);
            }
            set
            {
                if (_controller != null)
                    _controller.Dispose();
                if (value)
                {
                    BuildController();
                }
                else
                {
                    _controller = null;
                }
            }
        }

        private void BuildController()
        {
            if (string.IsNullOrEmpty(GetPort()))
            {
                SetupDialog();
            }
            try
            {
                _controller = new FocusController(GetPort());
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                SetupDialog();
                _controller = new FocusController(GetPort());
            }
        }

        private string GetPort()
        {
            return GetValue("port");
        }

        private void SetPort(string portName)
        {
            SetValue("port", portName);
        }

        public int MaxIncrement
        {
            get
            {
                return _controller.MaxMove();
            }
        }

        public int MaxStep
        {
            get
            {
                return 100000;
            }
        }

        public void Move(int val)
        {
            if (!Link)
                throw new InvalidOperationException("Focuser link not activated");

            _controller.Reversed = _reversed;

            _controller.MoveRelative(val);
        }

        public bool Connected
        {
            get
            {
                return Link;
            }
            set
            {
                Link = value;
            }
        }
        public string Description { get; private set; }
        public string DriverInfo { get; private set; }
        public string DriverVersion { get; private set; }
        public short InterfaceVersion { get; private set; }
        public string Name { get; private set; }

        public int Position
        {
            get
            {
                if (!Link)
                    throw new InvalidOperationException("Focuser link not activated");
                return 0;
            }
        }

        public void SetupDialog()
        {
            SetupDialogForm sf = new SetupDialogForm(GetPort(), _reversed, _absolute, 100);
            if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SetPort(sf.GetSelectedPort());
                SetValue("reverse", sf.IsReversed().ToString());
                if (_controller != null)
                {
                    _controller.Dispose();
                    BuildController();
                }
                SetFlags();
            }
        }

        public string Action(string ActionName, string ActionParameters)
        {
            throw new System.NotImplementedException();
        }

        public void CommandBlind(string Command, bool Raw = false)
        {
            throw new System.NotImplementedException();
        }

        public bool CommandBool(string Command, bool Raw = false)
        {
            throw new System.NotImplementedException();
        }

        public string CommandString(string Command, bool Raw = false)
        {
            throw new System.NotImplementedException();
        }

        private void SetValue(string key, string value)
        {
            _profile.WriteValue(s_csDriverID, key, value ?? "", "");
        }

        private string GetValue(string key)
        {
            return _profile.GetValue(s_csDriverID, key, "");
        }


        private void SetFlags()
        {
            _reversed = GetValue("reverse") == "True";
            _absolute = GetValue("absolute") == "True";

        }



        public double StepSize
        {
            get
            {
                return 1;
            }
        }

        public bool TempCompAvailable
        {
            get
            {
                return false;
            }
        }


        #endregion


        //
        // PUBLIC COM INTERFACE IFocuser IMPLEMENTATION
        //

        #region IFocuser Members


        public bool TempComp
        {
            get { return false; }
            set { throw new PropertyNotImplementedException("TempComp", true); }
        }

        public double Temperature
        {
            // TODO Replace this with your implementation
            get { throw new PropertyNotImplementedException("Temperature", false); }
        }

        #endregion
    }
}
