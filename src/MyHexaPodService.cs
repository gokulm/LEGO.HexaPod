using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;
using submgr = Microsoft.Dss.Services.SubscriptionManager;
using brick = Microsoft.Robotics.Services.Sample.Lego.Nxt.Brick.Proxy;
using motor = Microsoft.Robotics.Services.Sample.Lego.Nxt.Motor.Proxy;
using sonarsensor = Microsoft.Robotics.Services.Sample.Lego.Nxt.SonarSensor.Proxy;
using touchsensor = Microsoft.Robotics.Services.Sample.Lego.Nxt.TouchSensor.Proxy;
using wpf = Microsoft.Ccr.Adapters.Wpf;
using System.Windows;

namespace MyHexaPodService
{
    public enum Direction
    {
        Front,
        Back,
        Stop
    }

    [Contract(Contract.Identifier)]
    [DisplayName("MyHexaPodService")]
    [Description("MyHexaPodService service (no description provided)")]
    class MyHexaPodService : DsspServiceBase
    {
        /// <summary>
        /// Service state
        /// </summary>
        [ServiceState]
        MyHexaPodServiceState _state = new MyHexaPodServiceState();

        /// <summary>
        /// Main service port
        /// </summary>
        [ServicePort("/MyHexaPodService", AllowMultipleInstances = true)]
        MyHexaPodServiceOperations _mainPort = new MyHexaPodServiceOperations();

        [SubscriptionManagerPartner]
        submgr.SubscriptionManagerPort _submgrPort = new submgr.SubscriptionManagerPort();

        /// <summary>
        /// NxtBrick partner
        /// </summary>
        [Partner("NxtBrick", Contract = brick.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        brick.NxtBrickOperations _nxtBrickPort = new brick.NxtBrickOperations();
        brick.NxtBrickOperations _nxtBrickNotify = new brick.NxtBrickOperations();

        /// <summary>
        /// NxtMotor partner
        /// </summary>
        [Partner("NxtMotor", Contract = motor.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        motor.MotorOperations _nxtMotorPort = new motor.MotorOperations();
        motor.MotorOperations _nxtMotorNotify = new motor.MotorOperations();

        /// <summary>
        /// NxtUltrasonicSensor partner
        /// </summary>
        [Partner("NxtUltrasonicSensor", Contract = sonarsensor.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        sonarsensor.UltrasonicSensorOperations _nxtUltrasonicSensorPort = new sonarsensor.UltrasonicSensorOperations();
        sonarsensor.UltrasonicSensorOperations _nxtUltrasonicSensorNotify = new sonarsensor.UltrasonicSensorOperations();

        /// <summary>
        /// NxtTouchSensor1 partner
        /// </summary>
        [Partner("NxtTouchSensor1", Contract = touchsensor.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        touchsensor.TouchSensorOperations _nxtTouchSensor1Port = new touchsensor.TouchSensorOperations();
        touchsensor.TouchSensorOperations _nxtTouchSensor1Notify = new touchsensor.TouchSensorOperations();

        /// <summary>
        /// NxtTouchSensor2 partner
        /// </summary>
        [Partner("NxtTouchSensor2", Contract = touchsensor.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        touchsensor.TouchSensorOperations _nxtTouchSensor2Port = new touchsensor.TouchSensorOperations();
        touchsensor.TouchSensorOperations _nxtTouchSensor2Notify = new touchsensor.TouchSensorOperations();
        
        //wpf.WpfServicePort _wpfServicePort;
        Port<DateTime> _timerPort = new Port<DateTime>();
        bool isRotata = true;

        /// <summary>
        /// Service constructor
        /// </summary>
        public MyHexaPodService(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
        }

        /// <summary>
        /// Service start
        /// </summary>
        protected override void Start()
        {

            // 
            // Add service specific initialization here
            // 

            SpawnIterator(DoStart);
        }

        private IEnumerator<ITask> DoStart()
        {
            var ultraSonicSubscribe = this._nxtUltrasonicSensorPort.Subscribe(_nxtUltrasonicSensorNotify);
            var touchSensor1Subscribe = this._nxtTouchSensor1Port.Subscribe(_nxtTouchSensor1Notify);
            var touchSensor2Subscribe = this._nxtTouchSensor2Port.Subscribe(_nxtTouchSensor2Notify);

            yield return (Choice)ultraSonicSubscribe;
            yield return (Choice)touchSensor1Subscribe;
            yield return (Choice)touchSensor1Subscribe;

            // Activate independent tasks
            Activate<ITask>(
                Arbiter.Receive<sonarsensor.SonarSensorUpdate>(true, _nxtUltrasonicSensorNotify, SonarSensorUpdateHandler),
                Arbiter.Receive<touchsensor.TouchSensorUpdate>(true, _nxtTouchSensor1Notify, TouchSensor1UpdateHandler),
                Arbiter.Receive<touchsensor.TouchSensorUpdate>(true, _nxtTouchSensor2Notify, TouchSensor2UpdateHandler)
            );

            // Start operation handlers and insert into directory service.
            StartHandlers();
        }

        private void StartHandlers()
        {
            // Activate message handlers for this service and insert into the directory.
            base.Start();

        }

        /// <summary>
        /// Handles Subscribe messages
        /// </summary>
        /// <param name="subscribe">the subscribe request</param>
        [ServiceHandler]
        public void SubscribeHandler(Subscribe subscribe)
        {
            SubscribeHelper(_submgrPort, subscribe.Body, subscribe.ResponsePort);
        }

        private void SonarSensorUpdateHandler(sonarsensor.SonarSensorUpdate updateReadings)
        {
            if (updateReadings.Body.Distance <= 25 && isRotata)
            {
                RotateMotor(Direction.Back);
            }
            else if (updateReadings.Body.Distance > 25 && isRotata)
            {
                RotateMotor(Direction.Front);
            }
        }

        private void TouchSensor1UpdateHandler(touchsensor.TouchSensorUpdate sensorReadings)
        {
            if (sensorReadings.Body.TouchSensorOn)
            {
                RotateMotor(Direction.Front); 
            }
        }

        private void TouchSensor2UpdateHandler(touchsensor.TouchSensorUpdate sensorReadings)
        {
            if (sensorReadings.Body.TouchSensorOn)
            {
                RotateMotor(Direction.Stop);
            }
        }

        public void RotateMotor(Direction direction)
        {
            int targetPower = 1;

            switch (direction)
            {
                case Direction.Front:
                    targetPower = 1;
                    isRotata = true;
                    break;
                case Direction.Back:
                    targetPower = -1;
                    isRotata = true;
                    break;
                case Direction.Stop:
                    targetPower = 0;
                    isRotata = false;
                    break;
            }

            _nxtMotorPort.RotateForDuration(targetPower, -1);
        }

    }
}


