using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace PoolPumpTimer
{
    public class RelayController
    {
        private bool _cachedRelayState = false;
        private readonly OutputPort _pumpRelay;
        DateTime lastUpdateTime = DateTime.Now;

        public RelayController()
        {
            this._pumpRelay = new OutputPort(Pins.GPIO_PIN_D3, false);
        }

        
        public void TurnOnOffRelay(bool relayState)
        {
        
            if (lastUpdateTime.AddSeconds(5) < DateTime.Now)
            {
                if (_cachedRelayState != relayState)
                {
                    _cachedRelayState = relayState;
                    _pumpRelay.Write(relayState);
                    lastUpdateTime = DateTime.Now;
                }

            }

        }

    }
}
