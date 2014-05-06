using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace PoolPumpTimer
{
    public class AmbientLight
    {
        private static readonly AnalogInput _light = new AnalogInput(Cpu.AnalogChannel.ANALOG_3);

        private AmbientLight()
        {

        }


        private static double GetLight()
        {

            double sampleSize = 10;
            double val = 0;
            for (int i = 0; i < sampleSize; i++)
            {
                val += _light.Read();
            }

            return val / sampleSize;
        }


        public static bool LightLevelOK() 
        { 
        // return true thill i get a better reading
            return true;
        }

    }
}
