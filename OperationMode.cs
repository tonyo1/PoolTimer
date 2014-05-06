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
    public class OperationMode
    {
        private bool _cachedState = false;
        private InterruptPort _powerButton;
        private readonly OutputPort _powerLED;
        private OperationModes _operationMode = OperationModes.Off;
        Thread t1;

        public OperationMode()
        {
            _powerButton = new InterruptPort(Pins.GPIO_PIN_D1, _cachedState, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
            _powerButton.OnInterrupt += new NativeEventHandler(button_OnInterrupt);
            this._powerLED = new OutputPort(Pins.GPIO_PIN_D0, false);
            t1 = new Thread(new ThreadStart(blinkey));
            t1.Priority = ThreadPriority.Lowest;
        }

        DateTime _buttonPresstime = DateTime.Now;
        public OperationModes CurrentMode
        {
            get
            {       
                return _operationMode;
            }
        }
        private void button_OnInterrupt(uint port, uint data, DateTime time)
        {

            if (data == 1 && _buttonPresstime.AddMilliseconds(300) < time)
            {
                if (t1.ThreadState == ThreadState.Running)
                {
                    t1.Suspend();
                    while (t1.ThreadState != ThreadState.Suspended)
                    {
                        // spin until the blinky stops
                    }
                }
                _buttonPresstime = DateTime.Now;
                switch (CurrentMode)
                {
                    case OperationModes.Off:
                        _operationMode = OperationModes.On;
                        PowerLedSet(true);
                        break;
                    case OperationModes.On:
                        PowerLedSet(false);
                        _operationMode = OperationModes.Prog;
                        if (t1.ThreadState == ThreadState.Unstarted)
                        {
                            t1.Start();
                        }
                        else
                        {
                            t1.Resume();
                        }
                        break;
                    case OperationModes.Prog:
                        _operationMode = OperationModes.Off;
                        PowerLedSet(false);
                        break;
                }
              
            }
        }

        private void blinkey()
        {
            DateTime _time = DateTime.Now;
            bool blinkyBool = false;
            while (true)
            {
                if (_time.AddMilliseconds(350) <= DateTime.Now)
                {
                    blinkyBool = (blinkyBool) ? false : true;
                    _time = DateTime.Now;
                    PowerLedSet(blinkyBool);
                }
            }
        }

        private void PowerLedSet(bool IsOn)
        {
            if (_cachedState != IsOn)
            { 
              _powerLED.Write(IsOn);
              _cachedState = IsOn;
            }        
        }

        
      
    }
}