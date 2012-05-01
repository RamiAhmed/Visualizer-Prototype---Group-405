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
        public int inAmplitude,
                    inFrequency,
                    inPitch,
                    inBrightness,
                    inFundamentalFrequency;
        public float inNoise,
                      inPeakAmplitude,
                      inLoudness;

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

            Console.WriteLine("OSC Client: " + sOscServer.TransmissionType.ToString());
        }

        public void stopOSCServer()
        {
            sOscServer.Stop();
        }

        void sOscServer_MessageReceived(object sender, OscMessageReceivedEventArgs OSCEvent)
        {
            Console.WriteLine("\nMessage Length: " + OSCEvent.Message.Data.Length);

            inPitch = OSCEvent.Message.At<int>(0);
            inAmplitude = OSCEvent.Message.At<int>(1);
            inLoudness = OSCEvent.Message.At<float>(2);
            inBrightness = OSCEvent.Message.At<int>(3);
            inNoise = OSCEvent.Message.At<float>(4);
            if (OSCEvent.Message.At<int>(5) != 0)
            {
                inFundamentalFrequency = OSCEvent.Message.At<int>(5);
            }
            inPeakAmplitude = OSCEvent.Message.At<float>(6);

            Console.WriteLine("Pitch: " + inPitch.ToString());
            Console.WriteLine("Amplitude: " + inAmplitude.ToString());
            Console.WriteLine("Loudness: " + inLoudness.ToString());
            Console.WriteLine("Brightness: " + inBrightness.ToString());
            Console.WriteLine("Noise: " + inNoise.ToString());
            Console.WriteLine("Fundamental Frequency: " + inFrequency.ToString());
            Console.WriteLine("Peak amplitude: " + inPeakAmplitude.ToString());
        }
    }
}
