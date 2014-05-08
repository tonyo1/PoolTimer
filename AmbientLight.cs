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
        private static bool _cachedLightState = false;
        private static readonly AnalogInput _lightSensor = new AnalogInput(Cpu.AnalogChannel.ANALOG_3);
        private static readonly AnalogInput _lightThresholdSensor = new AnalogInput(Cpu.AnalogChannel.ANALOG_4);
        private static readonly OutputPort _lightLevelLED = new OutputPort(Pins.GPIO_PIN_D4, false);

        private AmbientLight()
        {

        }


        private static double GetLight()
        {

            double sampleSize = 10;
            double val = 0;
            for (int i = 0; i < sampleSize; i++)
            {
                val += _lightSensor.Read();
            }

            return val / sampleSize;
        }


        public static bool LightLevelOK()
        {

            double lightLevel = GetLight();
            double lightThreshold = 1 - _lightThresholdSensor.Read();

            double indProp = lightThreshold / lightLevel;


            
            if (_cachedLightState)
            {

                if (indProp + .02 < 1)
                {
                    _cachedLightState = false;
                    _lightLevelLED.Write(_cachedLightState);
                    
                }
            }

            if (!_cachedLightState)
            {
                if (indProp - .02 > 1)
                {
                    _cachedLightState = true;
                    _lightLevelLED.Write(_cachedLightState);

                }
            }
   
            return _cachedLightState;
        }

    }
}
