using System;
using Midi;
using vJoyInterfaceWrap;
using System.Collections.Generic;

namespace midi2controller
{
    class Launchpad
    {

        InputDevice launchpadInput = null;
        OutputDevice launchpadOutput = null;
        static public vJoy joystick;
        static public vJoy.JoystickState iReport;
        static public uint id = 1;
        List<List<Pad>> padMatrix;
        List<List<Pitch>> pitchLookupTable = new List<List<Pitch>>() {
            new List<Pitch>() { Pitch.CNeg1, Pitch.CSharpNeg1, Pitch.DNeg1, Pitch.DSharpNeg1, Pitch.ENeg1, Pitch.FNeg1, Pitch.FSharpNeg1, Pitch.GNeg1, Pitch.GSharpNeg1},
            new List<Pitch>() { Pitch.E0, Pitch.F0, Pitch.FSharp0, Pitch.G0, Pitch.GSharp0, Pitch.A0, Pitch.ASharp0, Pitch.B0, Pitch.C1},
            new List<Pitch>() { Pitch.GSharp1, Pitch.A1, Pitch.ASharp1, Pitch.B1, Pitch.C2, Pitch.CSharp2, Pitch.D2, Pitch.DSharp2, Pitch.E2},
            new List<Pitch>() { Pitch.C3, Pitch.CSharp3, Pitch.D3, Pitch.DSharp3, Pitch.E3, Pitch.F3, Pitch.FSharp3, Pitch.G3, Pitch.GSharp3},
            new List<Pitch>() { Pitch.E4, Pitch.F4, Pitch.FSharp4, Pitch.G4, Pitch.GSharp4, Pitch.A4, Pitch.ASharp4, Pitch.B4, Pitch.C5},
            new List<Pitch>() { Pitch.GSharp5, Pitch.A5, Pitch.ASharp5, Pitch.B5, Pitch.C6, Pitch.CSharp6, Pitch.D6, Pitch.DSharp6, Pitch.E6},
            new List<Pitch>() { Pitch.C7, Pitch.CSharp7, Pitch.D7, Pitch.DSharp7, Pitch.E7, Pitch.F7, Pitch.FSharp7, Pitch.G7, Pitch.GSharp7},
            new List<Pitch>() { Pitch.E8, Pitch.F8, Pitch.FSharp8, Pitch.G8, Pitch.GSharp8, Pitch.A8, Pitch.ASharp8, Pitch.B8, Pitch.C9}
        };
        

        public Launchpad()
        {
            joystick = new vJoy();
            iReport = new vJoy.JoystickState();
            padMatrix = new List<List<Pad>>();
            for(int i = 0; i < 8; i++)
            {
                padMatrix.Add(new List<Pad>());
                for(int j = 0; j < 9; j++)
                {
                    padMatrix[i].Add(new Pad(Channel.Channel1));
                }
            }
        }

        public void NoteOn(NoteOnMessage msg)
        {
            if (msg.Velocity != 0)
            {
                Console.WriteLine("Channel: " + msg.Channel);
                Console.WriteLine("Note: " + msg.Pitch);
                Console.WriteLine("Velocity: " + msg.Velocity);
                Console.WriteLine("-------------------");
                SendNote(Channel.Channel1, msg.Pitch, (int)PadColor.FULL_GREEN);
                joystick.SetBtn(true, id, PitchToButtonNum(msg.Pitch));
            }
            else
            {
                SendNote(Channel.Channel1, msg.Pitch, (int)PadColor.FULL_ORANGE);
                joystick.SetBtn(false, id, PitchToButtonNum(msg.Pitch));
            }
        }

        public void FindAndRegisterLaunchpad()
        {
            for (int i = 0; i < InputDevice.InstalledDevices.Count; i++)
            {
                if (InputDevice.InstalledDevices[i].Name.Contains("Launchpad"))
                {
                    Console.WriteLine("Found: " + InputDevice.InstalledDevices[i].Name);
                    launchpadInput = InputDevice.InstalledDevices[i];
                    launchpadInput.Open();
                }
            }

            for (int i = 0; i < OutputDevice.InstalledDevices.Count; i++)
            {
                if (OutputDevice.InstalledDevices[i].Name.Contains("Launchpad"))
                {
                    Console.WriteLine("Found: " + OutputDevice.InstalledDevices[i].Name);
                    launchpadOutput = OutputDevice.InstalledDevices[i];
                    launchpadOutput.Open();
                }
            }

            if (launchpadInput == null || launchpadOutput == null)
            {
                Console.WriteLine("Failed to find input or output! Press any key to continue...");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        public void SendNote(Channel channel, Pitch pitch, int vel)
        {
            launchpadOutput.SendNoteOn(channel, pitch, vel);
        }

        public void StartSendingAndRecieving()
        {
            launchpadInput.NoteOn += new InputDevice.NoteOnHandler(NoteOn);
            launchpadInput.StartReceiving(null);
            bool exit = false;

            while (!exit)
            {
                for (int i = 0; i < padMatrix.Count; i++)
                {
                    for (int j = 0; j < padMatrix[i].Count; j++)
                    {
                        Pitch pitch = LookUpPitch(i, j);
                        SendNote(Channel.Channel1, pitch, (int)PadColor.FULL_ORANGE);
                    }
                }
            }
        }

        public void ResetColors()
        {
            foreach (Pitch pitch in Enum.GetValues(typeof(Pitch)))
            {
                SendNote(Channel.Channel1, pitch, 12);
            }
        }

        public Pitch LookUpPitch(int x, int y)
        {
            return pitchLookupTable[x][y];
        }

        public uint PitchToButtonNum(Pitch pitch)
        {
            uint counter = 1;
            for (int i = 0; i < pitchLookupTable.Count; i++)
            {
                for (int j = 0; j < pitchLookupTable[i].Count; j++)
                {
                    if (pitchLookupTable[i][j].Equals(pitch))
                    {
                        return counter;
                    }
                    counter++;
                }
            }
            return counter;
        }

        public void SetUpJoystick()
        {
            joystick = new vJoy();
            iReport = new vJoy.JoystickState();

            if (id <= 0 || id > 16)
            {
                Console.WriteLine("Illegal device ID {0}\nExit!", id);
                return;
            }

            // Get the driver attributes (Vendor ID, Product ID, Version Number)
            if (!joystick.vJoyEnabled())
            {
                Console.WriteLine("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
                return;
            }
            else
            {
                Console.WriteLine("Vendor: {0}\nProduct :{1}\nVersion Number:{2}\n", joystick.GetvJoyManufacturerString(), joystick.GetvJoyProductString(), joystick.GetvJoySerialNumberString());
            }

            // Get the state of the requested device
            VjdStat status = joystick.GetVJDStatus(id);
            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    Console.WriteLine("vJoy Device {0} is already owned by this feeder\n", id);
                    break;
                case VjdStat.VJD_STAT_FREE:
                    Console.WriteLine("vJoy Device {0} is free\n", id);
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    Console.WriteLine("vJoy Device {0} is already owned by another feeder\nCannot continue\n", id);
                    return;
                case VjdStat.VJD_STAT_MISS:
                    Console.WriteLine("vJoy Device {0} is not installed or disabled\nCannot continue\n", id);
                    return;
                default:
                    Console.WriteLine("vJoy Device {0} general error\nCannot continue\n", id);
                    return;
            };

            int nButtons = joystick.GetVJDButtonNumber(id);

            // Test if DLL matches the driver
            UInt32 DllVer = 0, DrvVer = 0;
            bool match = joystick.DriverMatch(ref DllVer, ref DrvVer);
            if (match)
            {
                Console.WriteLine("Version of Driver Matches DLL Version ({0:X})\n", DllVer);
            }
            else
            {
                Console.WriteLine("Version of Driver ({0:X}) does NOT match DLL Version ({1:X})\n", DrvVer, DllVer);
            }

            // Acquire the target
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id))))
            {
                Console.WriteLine("Failed to acquire vJoy device number {0}.\n", id);
                return;
            }
            else
            {
                Console.WriteLine("Acquired: vJoy device number {0}.\n", id);
            }

            joystick.ResetVJD(id);
        }
    }
}
