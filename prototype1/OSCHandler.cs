using System;
using System.Net;
using System.IO;
using System.Collections.Generic;

using Bespoke.Common;
using Bespoke.Common.Osc;

namespace prototype1
{
    class OSCHandler
    {
        /* OSC Variables */
        private OscServer sOscServer;
        private readonly IPAddress ServerAddress = IPAddress.Parse("224.25.26.27");
        private readonly int ServerPort = 12000;

        /* Sound properties */
        static public int inBrightness;
        static public float inNoise,
                      inPeakAmplitude,
                      inLoudness,
                      inPitch,
                      inFundamentalFrequency,
                      inAmplitude;

        // DEBUG
        private static bool debug = false;

        public OSCHandler()
        {
            init();
        }

        private void init()
        {
            sOscServer = new OscServer(TransportType.Udp, IPAddress.Loopback, ServerPort, ServerAddress, Bespoke.Common.Net.TransmissionType.Multicast);
            sOscServer.RegisterMethod("/test");
            sOscServer.MessageReceived += new OscMessageReceivedHandler(sOscServer_MessageReceived);
            OscPacket.LittleEndianByteOrder = false;
            sOscServer.Start();

            if (debug)
            {
                Console.WriteLine("OSC Client: " + sOscServer.TransmissionType.ToString());
            }
        }

        public void stopOSCServer()
        {
            sOscServer.Stop();
        }

        private static void sOscServer_MessageReceived(object sender, OscMessageReceivedEventArgs OSCEvent)
        {
            if (debug)
            {
                Console.WriteLine("\nMessage Length: " + OSCEvent.Message.Data.Length);
            }

            inPitch = OSCEvent.Message.At<float>(0);
            inAmplitude = OSCEvent.Message.At<float>(1);
            inLoudness = OSCEvent.Message.At<float>(2);
            inBrightness = OSCEvent.Message.At<int>(3);
            inNoise = OSCEvent.Message.At<float>(4);
            if (OSCEvent.Message.At<float>(5) != 0)
            {
                inFundamentalFrequency = OSCEvent.Message.At<float>(5);
            }
            inPeakAmplitude = OSCEvent.Message.At<float>(6);

            if (debug)
            {
                Console.WriteLine("Pitch: " + inPitch.ToString());
                Console.WriteLine("Amplitude: " + inAmplitude.ToString());
                Console.WriteLine("Loudness: " + inLoudness.ToString());
                Console.WriteLine("Brightness: " + inBrightness.ToString());
                Console.WriteLine("Noise: " + inNoise.ToString());
                Console.WriteLine("Fundamental Frequency: " + inFundamentalFrequency.ToString());
                Console.WriteLine("Peak amplitude: " + inPeakAmplitude.ToString());
            }
        }
    }
}
