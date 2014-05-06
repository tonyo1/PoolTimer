using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
namespace PoolPumpTimer
{
    public class Program
    {    
        public static void Main()
        {
            RelayController _relay = new RelayController();

            OperationMode _operationMode = new OperationMode();

            OutputPort _onBoardLED = new OutputPort(Pins.ONBOARD_LED, true);

            
            while (true)
            {
                bool _relayState = false;
                OperationModes _OperationMode = _operationMode.CurrentMode;

                // these two look ups are in the main loop to 
                // light the leds to indicate state to the user
                // without effecting the relay and even if the running state is "off"                
                bool tempLevelOK = ExternalTemp.TempLevelOK();
                bool lightLevelOK = AmbientLight.LightLevelOK();

                if (_OperationMode == OperationModes.Prog)
                {
                    _relayState = lightLevelOK & tempLevelOK;
                }

                if (_OperationMode == OperationModes.Off)
                {
                    _relayState = false;
                }

                if (_OperationMode == OperationModes.On)
                {
                    _relayState = true;
                }
                _relay.TurnOnOffRelay(_relayState);
            }
        }
    }
}