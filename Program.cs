#define ROBUST


using System;
using System.Collections.Generic;
using System.Text;
using Midi;
using vJoyInterfaceWrap;


namespace midi2controller
{
    class Program
    {
        static void Main(string[] args)
        {
            Launchpad launchpad = new Launchpad();
            launchpad.FindAndRegisterLaunchpad();
            launchpad.SetUpJoystick();
            launchpad.StartSendingAndRecieving();
        }
    }
}
