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

        //  private static OutputPort _Led;
        //          _Led = new OutputPort(Pins.ONBOARD_LED, false);



        public static void Main()
        {
            RelayController _relay = new RelayController();

            OperationMode operationMode = new OperationMode();

            OutputPort board = new OutputPort(Pins.ONBOARD_LED, true);



            while (true)
            {
                bool _relayState = false;
                OperationModes _OperationMode = operationMode.CurrentMode;

                bool lightLevelOK = ExternalTemp.TempLevelOK();
                bool tempLevelOK = true;

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