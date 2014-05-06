using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace PoolPumpTimer
{
    public class ExternalTemp
    {
        private static bool _cachedLightLevelOK = false;
        private static readonly AnalogInput _thermoInput = new AnalogInput(Cpu.AnalogChannel.ANALOG_5);
        private static readonly AnalogInput _thermoSetter = new AnalogInput(Cpu.AnalogChannel.ANALOG_0);
        private static readonly OutputPort _thermoLED = new OutputPort(Pins.GPIO_PIN_D2, false);

        private ExternalTemp()
        {
        }

        private static double getVoltage_thermoInput(int sample = 10)
        {

            double reading = 0;
            DateTime lastRead = DateTime.MinValue;
            for (int i = 0; i < sample; )
            {
                if (lastRead.AddMilliseconds(10) <= DateTime.Now)
                {
                    // Read the value on the AIO port
                    reading += _thermoInput.Read();
                    i++;
                    lastRead = DateTime.Now;
                }
            }
            reading /= sample;
            
            return reading;
        }

        private static double getVoltage_thermoSetter(int sample = 10)
        {

            double reading = 0;
            DateTime lastRead = DateTime.MinValue;
            for (int i = 0; i < sample; )
            {
                if (lastRead.AddMilliseconds(10) <= DateTime.Now)
                {
                    // Read the value on the AIO port
                    reading += _thermoSetter.Read();
                    i++;
                    lastRead = DateTime.Now;
                }
            }
            reading /= sample;
            return reading;
        }

        public static bool TempLevelOK()
        {
            bool tempLevelOK = false;
            double settervoltage = (getVoltage_thermoSetter());
            
            double thermoInput = getVoltage_thermoInput(); // the higher the number the higher the temperature


            double celc = ((thermoInput * 3.3) - 0.500) * 100.0;
            double fer = celc;
            fer *= 9; fer /= 5; fer += 32;

            
            if (_cachedLightLevelOK)
            {
                tempLevelOK = fer >= StartTemperature(settervoltage) - 1;
                if (tempLevelOK != _cachedLightLevelOK)
                {
                    _cachedLightLevelOK = tempLevelOK;
                    _thermoLED.Write(tempLevelOK);
                }
                
            }

            if (!_cachedLightLevelOK)
            {
                tempLevelOK = fer >= StartTemperature(settervoltage) + 1;
                if (tempLevelOK != _cachedLightLevelOK)
                {
                    _cachedLightLevelOK = tempLevelOK;
                    _thermoLED.Write(tempLevelOK);
                }
            }
            return tempLevelOK;
        }



        private static double StartTemperature(double settervoltage)
        {
            if (settervoltage > .834) return 90;
            if (settervoltage > .668) return 85;
            if (settervoltage > .502) return 80;
            if (settervoltage > .366) return 75;
            if (settervoltage > .170) return 70;
            return 65;
        }

    }




    public class AmbientLight
    {
        private static readonly AnalogInput _light = new AnalogInput(Cpu.AnalogChannel.ANALOG_3);

        private AmbientLight()
        {

        }


        public static double GetLight()
        {

            double sampleSize = 10;
            double val = 0;
            for (int i = 0; i < sampleSize; i++)
            {
                val += _light.Read();
            }

            return val / sampleSize;
        }




    }
}
