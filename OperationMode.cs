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


        private DateTime btnLastClickedTime;
        private bool pwrButtonOn;
        private bool prgButtonOn;

        private OperationModes _operationMode;


        private InterruptPort _powerButton;
        private InterruptPort _programButton;
        private readonly OutputPort _powerLED;
        private readonly OutputPort _programLED;


        Thread t2;

        public OperationMode()
        {
            _powerButton = new InterruptPort(Pins.GPIO_PIN_D0, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeBoth);
            _programButton = new InterruptPort(Pins.GPIO_PIN_D1, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeBoth);

            _powerButton.OnInterrupt += _powerButton_OnInterrupt;
            _programButton.OnInterrupt += _programButton_OnInterrupt;

            this._powerLED = new OutputPort(Pins.GPIO_PIN_D8, true);
            this._programLED = new OutputPort(Pins.GPIO_PIN_D9, true);

            t2 = new Thread(new ThreadStart(blinkey));
            t2.Priority = ThreadPriority.Lowest;
            t2.Start();

            // if we power cycle in the on position stay off to force the button click
            _operationMode = OperationModes.Off;

        

        }

        void _powerButton_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (btnLastClickedTime.AddMilliseconds(50) < DateTime.Now)// debounce
            {
                btnLastClickedTime = DateTime.Now;
                Program.LastUpdateTime = DateTime.MinValue;
                PwrButtonOn = data2 == 0 ? false : true;
                if (PwrButtonOn) PrgButtonOn = _programButton.Read();
                onButtonStateChanged();

            }            
        }

        void _programButton_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (btnLastClickedTime.AddMilliseconds(50) < DateTime.Now)// debounce
            {
                btnLastClickedTime = DateTime.Now;
                Program.LastUpdateTime = DateTime.MinValue;
                PrgButtonOn = data2 == 0 ? false : true;
                onButtonStateChanged();
            }
        }

        public bool PwrButtonOn
        {
            get
            {
                return pwrButtonOn;
            }
            set
            {
                if (pwrButtonOn != value)
                {
                    pwrButtonOn = value;
                    onButtonStateChanged();
                }
                
            }
        }
        public bool PrgButtonOn
        {
            get
            {
                return prgButtonOn;
            }
            set
            {
                if (prgButtonOn != value)
                {
                    Debug.Print("PrgButtonOn val changed from " + prgButtonOn + " to " + value);
                    prgButtonOn = value;
                    onButtonStateChanged();
                }
            }
        }

        public OperationModes CurrentMode
        {
            get
            {

                return _operationMode;
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
                    if (_operationMode == OperationModes.Prog)
                    {
                        blinkyBool = (blinkyBool) ? false : true;
                        _time = DateTime.Now;
                        _programLED.Write(blinkyBool);
                    }
                }
            }
        }

        public void PowerOnBlink()
        {

            bool state = true;
            for (int i = 0; i < 10; i++)
            {
                _powerLED.Write(state);
                state = state ? false : true;
                Thread.Sleep(100);
            }
            onButtonStateChanged();
        }
        private void onButtonStateChanged()
        {
            if (!PwrButtonOn)
            {
                _operationMode = OperationModes.Off;
                _powerLED.Write(false);
                _programLED.Write(false);
                return;
            }
            else
            {
                _powerLED.Write(true);
                _programLED.Write(true);
            }

            if (PwrButtonOn)
            {
                if (PrgButtonOn)
                {
                    _operationMode = OperationModes.Prog;
                }
                else
                {
                    _operationMode = OperationModes.On;
                    _programLED.Write(true);
                }
                return;
            }

        }


    }
}