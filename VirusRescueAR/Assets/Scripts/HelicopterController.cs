using UnityEngine;
using UnityEngine.UI;
 using System;

namespace NewHelicopter
{

    public class HelicopterController : MonoBehaviour
    {

        private float time, dano;
        private int persona, capacidad;
        [Header("Game")]
        public bool pause = false;
        public GameObject buttons;
        public GameObject gameOverL;
        public GameObject damageHeliL;
        public GameObject timeFinish;
        public GameObject winL;
        public GameObject capacidadL;
        public GameObject exitB;
        public GameObject resetB;
        public GameObject pauseB;
        public GameObject nextLevelB;

        private string horizontalAxis = "Horizontal";
        private string verticalAxis = "Vertical";
        private string jumpButton = "Jump";

        [Header("Inputs")]
        public bool isVirtualJoystick = false;

        [Header("View")]
        // to helicopter model
        public AudioSource HelicopterSound;
        public AudioSource WinSound;
        public AudioSource GameOverSound;
        public AudioSource CollisionSound;
        public AudioSource DestructionSound;

        public Rigidbody HelicopterModel;
        public HeliRotorController MainRotorController;
        public HeliRotorController SubRotorController;
        public DustAirController DustAirController;

        private float minHeight, maxHeight, realHeight;
        [Header("Fly Settings")]
        public LayerMask GroundMaskLayer = 4;
        public float TurnForce = 0.04f;
        public float ForwardForce = 0.2f;
        public float ForwardTiltForce = 0.4f;
        public float TurnTiltForce = 0.7f;
        public float EffectiveHeight = 0.4f;

        public float turnTiltForcePercent = 0.01f;
        public float turnForcePercent = 0.013f;

        private int zona1;
        private int zona2;
        private int zona3;
        [Header("Person")]
        public GameObject personZ1_1;
        public GameObject personZ1_2;
        public GameObject personZ2_1;
        public GameObject personZ2_2;
        public GameObject personZ3_1;

        private float _engineForce;
        public float EngineForce
        {
            get { return _engineForce; }
            set
            {
                MainRotorController.RotarSpeed = value * 80;
                SubRotorController.RotarSpeed = value * 40;
                HelicopterSound.pitch = Mathf.Clamp(value / 40, 0, 1.2f);
                _engineForce = value;
            }
        }

        private float distanceToGround ;
        public float DistanceToGround
        {
            get {return distanceToGround; }
        }

        private Vector3 pointToGround;
        public Vector3 PointToGround
        {
            get { return pointToGround; }
        }


        private Vector2 hMove = Vector2.zero;
        private Vector2 hTilt = Vector2.zero;
        private float hTurn = 0f;
        public bool IsOnGround = true;

        void FixedUpdate()
        {
            ProcessingInputs();
            LiftProcess();
            MoveProcess();
            TiltProcess();
            TimeProcess();

            Visualize();
        }

        void Start()
        {
            time = 180f;
            dano = 0.0f;
            persona = 0;
            capacidad = 0;
            zona1 = 2;
            zona2 = 2;
            zona3 = 1;
            EffectiveHeight = transform.position.y + 0.1f;
            minHeight = transform.position.y;
            maxHeight = transform.position.y + 0.43f;
            GroundMaskLayer = 4;
            TurnForce = 0.04f;
            ForwardForce = 0.2f;
            ForwardTiltForce = 0.4f;
            TurnTiltForce = 0.7f;

            turnTiltForcePercent = 0.01f;
            turnForcePercent = 0.013f;
        }

        private void TimeProcess()
        {
            if(pauseB.activeInHierarchy == true)
            {
                if(time > 0f)
                {
                    time -= Time.deltaTime;
                }
                else
                {
                    buttons.SetActive(false);
                    gameOverL.SetActive(true);
                    timeFinish.SetActive(true);
                    exitB.SetActive(true);
                    resetB.SetActive(true);
                    pauseB.SetActive(false);
                    HelicopterSound.mute = true;
                    GameOverSound.volume = 0.1f;
                }
            }
        }

        private void MoveProcess()
        {
            var turn = TurnForce * Mathf.Lerp(hMove.x, hMove.x * (turnTiltForcePercent - Mathf.Abs(hMove.y)), Mathf.Max(0f, hMove.y));
            hTurn = Mathf.Lerp(hTurn, turn, Time.fixedDeltaTime * TurnForce);
            HelicopterModel.AddRelativeTorque(0f, hTurn * HelicopterModel.mass, 0f);
            HelicopterModel.AddRelativeForce(Vector3.forward * Mathf.Max(0f, hMove.y * ForwardForce * HelicopterModel.mass));
        }

        private void LiftProcess()
        {
            // to ground distance
            RaycastHit hit;
            var direction = transform.TransformDirection(Vector3.down);
            
            var ray = new Ray(transform.position, direction);
            if (Physics.Raycast(ray, out hit, 300, GroundMaskLayer))
            {
                Debug.DrawLine(transform.position, hit.point, Color.cyan);
                distanceToGround = hit.distance;
                pointToGround = hit.point;

                //isOnGround = hit.distance < 2f;
            } 


            var upForce = 1 - Mathf.Clamp(HelicopterModel.transform.position.y / EffectiveHeight, 0, 1);
            upForce = Mathf.Lerp(0f, EngineForce, upForce) * HelicopterModel.mass;
            HelicopterModel.AddRelativeForce(Vector3.up * upForce);
        }

        private void TiltProcess()
        {
            hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * TurnTiltForce, Time.deltaTime);
            hTilt.y = Mathf.Lerp(hTilt.y, hMove.y * ForwardTiltForce, Time.deltaTime);
            HelicopterModel.transform.localRotation = Quaternion.Euler(hTilt.y, HelicopterModel.transform.localEulerAngles.y, -hTilt.x);
        }

        private void ProcessingMobileInputs()
        {
            if (!IsOnGround)
            {
                hMove.x = Input.GetAxis( horizontalAxis);
                hMove.y = Input.GetAxis( verticalAxis);
            }
            else
            {
                hMove.x = 0;
                hMove.y = 0;
            }

            if (Input.GetAxis(jumpButton) > 0)
            {
                EngineForce += 0.1f;
            }
            else
            if (Input.GetAxis(jumpButton) < 0)
            {
                EngineForce -= 0.12f;
            }
        }


        private void ProcessingInputs()
        {
            if (!IsOnGround)
            {
                hMove.x = GetInput( horizontalAxis);
                hMove.y = GetInput( verticalAxis);
            }

            if (GetInput(jumpButton) > 0)
            {
                EngineForce += 0.08f;
            }
            else
            {
                if (GetInput(jumpButton) < 0)
                    EngineForce -= 0.10f;
            }

        }

        private float GetInput(string input)
        {
            if(isVirtualJoystick)
                return SimpleInput.GetAxis(input);
            else
                return Input.GetAxis(input);
        }
        
        private void FinishDamage()
        {
            HelicopterSound.mute = true;
            GameOverSound.volume = 0.1f;
            buttons.SetActive(false);
            exitB.SetActive(true);
            resetB.SetActive(true);
            pauseB.SetActive(false);
            gameOverL.SetActive(true);
            damageHeliL.SetActive(true); 
        }

        private void OnTriggerEnter(Collider objeto)
        {
            switch(objeto.tag)
            {
                case "suelo":
                    IsOnGround = true;
                    break;
                case "montana":
                    dano = dano + 3.0f;
                    if(dano >= 100.0f){
                        FinishDamage();
                    }
                    break;
                case "casa":
                    dano = dano + 2.0f;
                    if(dano >= 100.0f){
                        FinishDamage();
                    }
                    break;
                case "arbol":
                    dano = dano + 1.0f;
                    if(dano >= 100.0f){
                        FinishDamage();
                    }
                    break;
                case "roca":
                    dano = dano + 0.5f;
                    if(dano >= 100.0f){
                        FinishDamage();
                    }
                    break;
                case "zona":
                    if(zona1 > 0){
                        if(capacidad == 0 || capacidad == 1 || capacidad == 2)
                        {
                            if(zona1 == 2)
                            {
                                capacidad = capacidad + 2;
                                personZ1_1.SetActive(false);
                                personZ1_2.SetActive(false);
                                zona1 = 0;
                            }
                            else if(zona1 == 1)
                            {
                                zona1 = 0;
                                capacidad = capacidad + 1;
                                if(personZ1_1.activeInHierarchy == true)
                                    personZ1_1.SetActive(false);
                                else if(personZ1_2.activeInHierarchy == true)
                                    personZ1_2.SetActive(false);
                            }
                            
                        }
                        else if(capacidad == 3)
                        {
                            zona1 = zona1 - 1;
                            capacidad = capacidad + 1;
                            if(personZ1_1.activeInHierarchy == true)
                                personZ1_1.SetActive(false);
                            else if(personZ1_2.activeInHierarchy == true)
                                personZ1_2.SetActive(false);
                        }
                        else
                        {
                            capacidadL.SetActive(true);
                        }
                    }
                    break;
                case "zona2":
                    if(zona2 > 0){
                        if(capacidad == 0 || capacidad == 1 || capacidad == 2)
                        {
                            if(zona2 == 2)
                            {
                                capacidad = capacidad + 2;
                                personZ2_1.SetActive(false);
                                personZ2_2.SetActive(false);
                                zona2 = 0;
                            }
                            else if(zona2 == 1)
                            {
                                zona2 = 0;
                                capacidad = capacidad + 1;
                                if(personZ2_1.activeInHierarchy == true)
                                    personZ2_1.SetActive(false);
                                else if(personZ2_2.activeInHierarchy == true)
                                    personZ2_2.SetActive(false);
                            }
                            
                        }
                        else if(capacidad == 3)
                        {
                            zona2 = zona2 - 1;
                            capacidad = capacidad + 1;
                            if(personZ2_1.activeInHierarchy == true)
                                personZ2_1.SetActive(false);
                            else if(personZ2_2.activeInHierarchy == true)
                                personZ2_2.SetActive(false);
                        }
                        else
                        {
                            capacidadL.SetActive(true);
                        }
                    }
                    break;
                case "zona3":
                    if(zona3 > 0)
                    {
                        if(capacidad == 0 || capacidad == 1 || capacidad == 2 || capacidad == 3)
                        {
                            capacidad = capacidad + 1;
                            zona3 = 0;
                            personZ3_1.SetActive(false);
                        }
                        else
                        {
                            capacidadL.SetActive(true);
                        }
                    }
                    break;
                case "helipuerto":
                    persona = persona + capacidad;
                    capacidad = 0;
                    capacidadL.SetActive(false);
                    if(persona == 5)
                    {
                        HelicopterSound.mute = true;
                        WinSound.volume = 0.1f;
                        winL.SetActive(true);
                        nextLevelB.SetActive(true);
                        buttons.SetActive(false);
                        pauseB.SetActive(false);
                        resetB.SetActive(false);
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnTriggerExit(Collider objeto)
        {
            switch(objeto.tag)
            {
                case "suelo":
                    IsOnGround = false;
                    break;
                default:
                    break;
            }
        }

        private void Visualize()
        {
            if (DustAirController != null)
            {
                DustAirController.ProgressEngineValue(EngineForce);
                DustAirController.VisualizeDustGround(DistanceToGround, PointToGround);
            }

            if (UIViewController.runtime.Time != null)
            {
                    UIViewController.runtime.Time.text = string.Format("Tiempo [ {0} ] ", (int)time);
            }

            if (UIViewController.runtime.Damage != null)
            {
                UIViewController.runtime.Damage.text = string.Format("Dano [ {0} ] %", (int)dano);
            }

            if (UIViewController.runtime.Capacity != null)
            {
                    UIViewController.runtime.Capacity.text = string.Format("Capacidad [ {0}/4 ]", (int)capacidad);
            }

            if (UIViewController.runtime.Person != null)
            {
                    UIViewController.runtime.Person.text = string.Format("Rescatados [ {0}/5 ] %", (int)persona);
            }

            if (UIViewController.runtime.EngineForceView != null)
            {
                if((int)EngineForce >= 0)
                    UIViewController.runtime.EngineForceView.text = string.Format("Fuerza motor [ {0} ] ", (int)EngineForce);
            }

            if (UIViewController.runtime.HeigthView != null)
            {   
                UIViewController.runtime.HeigthView.text = string.Format("Altura [{0}] m", (float)Math.Round((double)(transform.position.y),4));
            }
        }
    }
}