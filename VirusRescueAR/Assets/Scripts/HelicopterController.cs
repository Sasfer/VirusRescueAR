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

        [Header("Level")]
        public int level = 1;

        [Header("View")]
        // to helicopter model
        public AudioSource HelicopterSound;
        public AudioSource FondoSound;
        public AudioSource WinSound;
        public AudioSource GameOverSound;
        public AudioSource CollisionSound;
        public AudioSource DestructionSound;
        public AudioSource PersonSound;

        public Rigidbody HelicopterModel;
        public HeliRotorController MainRotorController;
        public HeliRotorController SubRotorController;
        public DustAirController DustAirController;

        private float minHeight, maxHeight, realHeight;
        [Header("Fly Settings")]
        public LayerMask GroundMaskLayer = 4;
        public float TurnForce = 0.015f;
        public float ForwardForce = 0.05f;
        public float ForwardTiltForce = 0.1f;
        public float TurnTiltForce = 0.15f;
        private float EffectiveHeight;

        public float turnTiltForcePercent = 0.0075f;
        public float turnForcePercent = 0.0065f;

        private int zona1, zona2, zona3;
        private int zona4, zona5, zona6, zona7;
        private int zona8, zona9, zona10, zona11, zona12;

        [Header("Person")]
        public GameObject personZ_1;
        public GameObject personZ_2;
        public GameObject personZ_3;
        public GameObject personZ_4;
        public GameObject personZ_5;
        public GameObject personZ_6;
        public GameObject personZ_7;
        public GameObject personZ_8;
        public GameObject personZ_9;
        public GameObject personZ_10;
        public GameObject personZ_11;

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
            WinGame();
            FinishDamage();
            Visualize();
        }

        void Start()
        {
            if(level == 1)
                time = 360f;
            else if(level == 2)
                time = 540f;
            else if(level == 3)
                time = 720f;

            dano = 0.0f;
            persona = 0;
            capacidad = 0;
            //Nivel 1
            zona1 = 2;
            zona2 = 2;
            zona3 = 1;
            //Nivel 2
            zona4 = 1;
            zona5 = 2;
            zona6 = 3;
            zona7 = 2;
            //Nivel 3
            zona8 = 2;
            zona9 = 3;
            zona10 = 1;
            zona11 = 3;
            zona12 = 2;

            EffectiveHeight = 0.5f;
            minHeight = transform.position.y;
            maxHeight = transform.position.y + 0.4f;
            TurnForce = 0.015f;
            ForwardForce = 0.05f;
            ForwardTiltForce = 0.1f;
            TurnTiltForce = 0.15f;

            turnTiltForcePercent = 0.0075f;
            turnForcePercent = 0.0065f;
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
                    FondoSound.volume = 0.0f;
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

            /*
            if(EffectiveHeight < 0){
                var upForce = 1 - Mathf.Clamp(EffectiveHeight / HelicopterModel.transform.position.y, 0, 1);
                upForce = Mathf.Lerp(0f, EngineForce, upForce) * HelicopterModel.mass;
                HelicopterModel.AddRelativeForce(Vector3.up * upForce);
            }
            else{
                var upForce = 1 - Mathf.Clamp(HelicopterModel.transform.position.y / EffectiveHeight, 0, 1);
                upForce = Mathf.Lerp(0f, EngineForce, upForce) * HelicopterModel.mass;
                HelicopterModel.AddRelativeForce(Vector3.up * upForce);
            }*/

            realHeight = maxHeight - transform.position.y;
            realHeight = 0.4f - realHeight;
            var upForce = 1 - Mathf.Clamp(realHeight / EffectiveHeight, 0, 1);
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
                if(transform.position.y <= maxHeight)
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
                if(transform.position.y <= maxHeight)
                    EngineForce += 0.1f;
            }
            else
            {
                if (GetInput(jumpButton) < 0)
                    EngineForce -= 0.12f;
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
            if(dano >= 100.0f){
                FondoSound.volume = 0.0f;
                GameOverSound.volume = 0.1f;
                DestructionSound.Play();
                buttons.SetActive(false);
                exitB.SetActive(true);
                resetB.SetActive(true);
                pauseB.SetActive(false);
                gameOverL.SetActive(true);
                damageHeliL.SetActive(true);   
            }
             
        }

        private void OnTriggerEnter(Collider objeto)
        {
            switch(objeto.tag)
            {
/*****************************************************************************************************************/
/*****************************************************************************************************************/
/*****************************************************************************************************************/
                case "zona1":
                    if(zona1 > 0){
                        if(capacidad == 0 || capacidad == 1 || capacidad == 2)
                        {
                            if(zona1 == 2)
                            {
                                capacidad = capacidad + 2;
                                personZ_1.SetActive(false);
                                personZ_2.SetActive(false);
                                zona1 = 0;
                                PersonSound.Play();
                            }
                            else if(zona1 == 1)
                            {
                                zona1 = 0;
                                capacidad = capacidad + 1;
                                if(personZ_1.activeInHierarchy == true)
                                {
                                    personZ_1.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_2.activeInHierarchy == true)
                                {
                                    personZ_2.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                            
                        }
                        else if(capacidad == 3)
                        {
                            zona1 = zona1 - 1;
                            capacidad = capacidad + 1;
                            if(personZ_1.activeInHierarchy == true)
                            {
                                personZ_1.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(personZ_2.activeInHierarchy == true)
                            {
                                personZ_2.SetActive(false);
                                PersonSound.Play();
                            }
                        }
                        else
                        {
                            capacidadL.SetActive(true);
                        }
                    }
                    break;
/*****************************************************************************************************************/
/*****************************************************************************************************************/
/*****************************************************************************************************************/
                case "zona2":
                    if(zona2 > 0){
                        if(capacidad == 0 || capacidad == 1 || capacidad == 2)
                        {
                            if(zona2 == 2)
                            {
                                capacidad = capacidad + 2;
                                personZ_3.SetActive(false);
                                personZ_4.SetActive(false);
                                zona2 = 0;
                                PersonSound.Play();
                            }
                            else if(zona2 == 1)
                            {
                                zona2 = 0;
                                capacidad = capacidad + 1;
                                if(personZ_3.activeInHierarchy == true)
                                {
                                    personZ_3.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_4.activeInHierarchy == true)
                                {
                                    personZ_4.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                            
                        }
                        else if(capacidad == 3)
                        {
                            zona2 = zona2 - 1;
                            capacidad = capacidad + 1;
                            if(personZ_3.activeInHierarchy == true)
                            {
                                personZ_3.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(personZ_4.activeInHierarchy == true)
                            {
                                personZ_4.SetActive(false);
                                PersonSound.Play();
                            }
                        }
                        else
                        {
                            capacidadL.SetActive(true);
                        }
                    }
                    break;
/*****************************************************************************************************************/
/*****************************************************************************************************************/
/*****************************************************************************************************************/
                case "zona3":
                    if(zona3 > 0)
                    {
                        if(capacidad == 0 || capacidad == 1 || capacidad == 2 || capacidad == 3)
                        {
                            capacidad = capacidad + 1;
                            zona3 = 0;
                            personZ_5.SetActive(false);
                            PersonSound.Play();
                        }
                        else
                        {
                            capacidadL.SetActive(true);
                        }
                    }
                    break;
/*****************************************************************************************************************/
/*****************************************************************************************************************/
/*****************************************************************************************************************/
                case "zona4":
                    if(zona4 > 0)
                    {
                        if(capacidad == 0 || capacidad == 1 || capacidad == 2 || capacidad == 3)
                        {
                            capacidad = capacidad + 1;
                            zona4 = 0;
                            personZ_1.SetActive(false);
                            PersonSound.Play();
                        }
                        else
                        {
                            capacidadL.SetActive(true);
                        }
                    }
                    break;
/*****************************************************************************************************************/
/*****************************************************************************************************************/
/*****************************************************************************************************************/
                case "zona5":
                    if(zona5 > 0){
                        if(capacidad == 0 || capacidad == 1 || capacidad == 2)
                        {
                            if(zona5 == 2)
                            {
                                capacidad = capacidad + 2;
                                zona5 = zona5 - 2;
                                personZ_2.SetActive(false);
                                personZ_3.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(zona5 == 1)
                            {
                                capacidad = capacidad + 1;
                                zona5 = zona5 - 1;
                                if(personZ_2.activeInHierarchy == true)
                                {
                                    personZ_2.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_3.activeInHierarchy == true)
                                {
                                    personZ_3.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                            
                        }
                        else if(capacidad == 3)
                        {
                            capacidad = capacidad + 1;
                            zona5 = zona5 - 1;
                            if(personZ_2.activeInHierarchy == true)
                            {
                                personZ_2.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(personZ_3.activeInHierarchy == true)
                            {
                                personZ_3.SetActive(false);
                                PersonSound.Play();
                            }
                        }
                        else
                        {
                            capacidadL.SetActive(true);
                        }
                    }
                    break;
/*****************************************************************************************************************/
/*****************************************************************************************************************/
/*****************************************************************************************************************/
                case "zona6":
                    if(zona6 > 0){
                        if(capacidad == 0 || capacidad == 1){
                            if(zona6 == 3){
                                capacidad = capacidad + 3;
                                zona6 = zona6 - 3;
                                personZ_4.SetActive(false);
                                PersonSound.Play();
                                personZ_5.SetActive(false);
                                PersonSound.Play();
                                personZ_6.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(zona6 == 2){
                                capacidad = capacidad + 2;
                                zona6 = zona6 - 2;
                                if(personZ_4.activeInHierarchy == true){
                                    personZ_4.SetActive(false);
                                    PersonSound.Play();
                                    if(personZ_5.activeInHierarchy == true){
                                        personZ_5.SetActive(false);
                                        PersonSound.Play();
                                    }
                                    else if(personZ_6.activeInHierarchy == true){
                                        personZ_6.SetActive(false);
                                        PersonSound.Play();
                                    }
                                }
                                else if(personZ_5.activeInHierarchy == true){
                                    personZ_5.SetActive(false);
                                    PersonSound.Play();
                                    
                                    personZ_6.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                            else if(zona6 == 1){
                                capacidad = capacidad + 1;
                                zona6 = zona6 - 1;
                                if(personZ_4.activeInHierarchy == true){
                                    personZ_4.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_5.activeInHierarchy == true){
                                    personZ_5.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_6.activeInHierarchy == true){
                                    personZ_6.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                        }
                        else if(capacidad == 2){
                            if(zona6 == 3 || zona6 == 2){
                                capacidad = capacidad + 2;
                                zona6 = zona6 - 2;
                                if(personZ_4.activeInHierarchy == true){
                                    personZ_4.SetActive(false);
                                    PersonSound.Play();
                                    if(personZ_5.activeInHierarchy == true){
                                        personZ_5.SetActive(false);
                                        PersonSound.Play();
                                    }
                                    else if(personZ_6.activeInHierarchy == true){
                                        personZ_6.SetActive(false);
                                        PersonSound.Play();
                                    }
                                }
                                else if(personZ_5.activeInHierarchy == true){
                                    personZ_5.SetActive(false);
                                    PersonSound.Play();
                                    
                                    personZ_6.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                            else if(zona6 == 1){
                                capacidad = capacidad + 1;
                                zona6 = zona6 - 1;
                                if(personZ_4.activeInHierarchy == true){
                                    personZ_4.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_5.activeInHierarchy == true){
                                    personZ_5.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_6.activeInHierarchy == true){
                                    personZ_6.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                        }
                        else if(capacidad == 3){
                            capacidad = capacidad + 1;
                            zona6 = zona6 - 1;
                            if(personZ_4.activeInHierarchy == true){
                                personZ_4.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(personZ_5.activeInHierarchy == true){
                                personZ_5.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(personZ_6.activeInHierarchy == true){
                                personZ_6.SetActive(false);
                                PersonSound.Play();
                            }
                        }
                        else{
                            capacidadL.SetActive(true);
                        }
                    }
                    break;
/*****************************************************************************************************************/
/*****************************************************************************************************************/
/*****************************************************************************************************************/
            case "zona7":
                    if(zona7 > 0){
                        if(capacidad == 0 || capacidad == 1 || capacidad == 2)
                        {
                            if(zona7 == 2)
                            {
                                capacidad = capacidad + 2;
                                zona7 = zona7 - 2;
                                personZ_7.SetActive(false);
                                personZ_8.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(zona7 == 1)
                            {
                                capacidad = capacidad + 1;
                                zona7 = zona7 - 1;
                                if(personZ_7.activeInHierarchy == true)
                                {
                                    personZ_7.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_8.activeInHierarchy == true)
                                {
                                    personZ_8.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                            
                        }
                        else if(capacidad == 3)
                        {
                            capacidad = capacidad + 1;
                            zona7 = zona7 - 1;
                            if(personZ_7.activeInHierarchy == true)
                            {
                                personZ_7.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(personZ_8.activeInHierarchy == true)
                            {
                                personZ_8.SetActive(false);
                                PersonSound.Play();
                            }
                        }
                        else
                        {
                            capacidadL.SetActive(true);
                        }
                    }
                    break;
/*****************************************************************************************************************/
/*****************************************************************************************************************/
/*****************************************************************************************************************/            
            case "zona8":
                    if(zona8 > 0){
                        if(capacidad == 0 || capacidad == 1 || capacidad == 2)
                        {
                            if(zona8 == 2)
                            {
                                capacidad = capacidad + 2;
                                zona8 = zona8 - 2;
                                personZ_1.SetActive(false);
                                personZ_2.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(zona8 == 1)
                            {
                                capacidad = capacidad + 1;
                                zona8 = zona8 - 1;
                                if(personZ_1.activeInHierarchy == true)
                                {
                                    personZ_1.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_2.activeInHierarchy == true)
                                {
                                    personZ_2.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                            
                        }
                        else if(capacidad == 3)
                        {
                            capacidad = capacidad + 1;
                            zona8 = zona8 - 1;
                            if(personZ_1.activeInHierarchy == true)
                            {
                                personZ_1.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(personZ_2.activeInHierarchy == true)
                            {
                                personZ_2.SetActive(false);
                                PersonSound.Play();
                            }
                        }
                        else
                        {
                            capacidadL.SetActive(true);
                        }
                    }
                    break;
/*****************************************************************************************************************/
/*****************************************************************************************************************/
/*****************************************************************************************************************/
            case "zona9":
                    if(zona9 > 0){
                        if(capacidad == 0 || capacidad == 1){
                            if(zona9 == 3){
                                capacidad = capacidad + 3;
                                zona9 = zona9 - 3;
                                personZ_3.SetActive(false);
                                PersonSound.Play();
                                personZ_4.SetActive(false);
                                PersonSound.Play();
                                personZ_5.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(zona9 == 2){
                                capacidad = capacidad + 2;
                                zona9 = zona9 - 2;
                                if(personZ_3.activeInHierarchy == true){
                                    personZ_3.SetActive(false);
                                    PersonSound.Play();
                                    if(personZ_4.activeInHierarchy == true){
                                        personZ_4.SetActive(false);
                                        PersonSound.Play();
                                    }
                                    else if(personZ_5.activeInHierarchy == true){
                                        personZ_5.SetActive(false);
                                        PersonSound.Play();
                                    }
                                }
                                else if(personZ_4.activeInHierarchy == true){
                                    personZ_4.SetActive(false);
                                    PersonSound.Play();
                                    
                                    personZ_5.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                            else if(zona9 == 1){
                                capacidad = capacidad + 1;
                                zona9 = zona9 - 1;
                                if(personZ_3.activeInHierarchy == true){
                                    personZ_3.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_4.activeInHierarchy == true){
                                    personZ_4.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_5.activeInHierarchy == true){
                                    personZ_5.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                        }
                        else if(capacidad == 2){
                            if(zona9 == 3 || zona9 == 2){
                                capacidad = capacidad + 2;
                                zona9 = zona9 - 2;
                                if(personZ_3.activeInHierarchy == true){
                                    personZ_3.SetActive(false);
                                    PersonSound.Play();
                                    if(personZ_4.activeInHierarchy == true){
                                        personZ_4.SetActive(false);
                                        PersonSound.Play();
                                    }
                                    else if(personZ_5.activeInHierarchy == true){
                                        personZ_5.SetActive(false);
                                        PersonSound.Play();
                                    }
                                }
                                else if(personZ_4.activeInHierarchy == true){
                                    personZ_4.SetActive(false);
                                    PersonSound.Play();
                                    
                                    personZ_5.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                            else if(zona9 == 1){
                                capacidad = capacidad + 1;
                                zona9 = zona9 - 1;
                                if(personZ_3.activeInHierarchy == true){
                                    personZ_3.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_4.activeInHierarchy == true){
                                    personZ_4.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_5.activeInHierarchy == true){
                                    personZ_5.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                        }
                        else if(capacidad == 3){
                            capacidad = capacidad + 1;
                            zona9 = zona9 - 1;
                            if(personZ_3.activeInHierarchy == true){
                                personZ_3.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(personZ_4.activeInHierarchy == true){
                                personZ_4.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(personZ_5.activeInHierarchy == true){
                                personZ_5.SetActive(false);
                                PersonSound.Play();
                            }
                        }
                        else{
                            capacidadL.SetActive(true);
                        }
                    }
                    break;
/*****************************************************************************************************************/
/*****************************************************************************************************************/
/*****************************************************************************************************************/
                case "zona10":
                    if(zona10 > 0)
                    {
                        if(capacidad == 0 || capacidad == 1 || capacidad == 2 || capacidad == 3)
                        {
                            capacidad = capacidad + 1;
                            zona10 = 0;
                            personZ_6.SetActive(false);
                            PersonSound.Play();
                        }
                        else
                        {
                            capacidadL.SetActive(true);
                        }
                    }
                    break;
/*****************************************************************************************************************/
/*****************************************************************************************************************/
/*****************************************************************************************************************/
                case "zona11":
                    if(zona11 > 0){
                        if(capacidad == 0 || capacidad == 1){
                            if(zona11 == 3){
                                capacidad = capacidad + 3;
                                zona11 = zona9 - 3;
                                personZ_7.SetActive(false);
                                PersonSound.Play();
                                personZ_8.SetActive(false);
                                PersonSound.Play();
                                personZ_9.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(zona11 == 2){
                                capacidad = capacidad + 2;
                                zona11 = zona11 - 2;
                                if(personZ_7.activeInHierarchy == true){
                                    personZ_7.SetActive(false);
                                    PersonSound.Play();
                                    if(personZ_8.activeInHierarchy == true){
                                        personZ_8.SetActive(false);
                                        PersonSound.Play();
                                    }
                                    else if(personZ_9.activeInHierarchy == true){
                                        personZ_9.SetActive(false);
                                        PersonSound.Play();
                                    }
                                }
                                else if(personZ_8.activeInHierarchy == true){
                                    personZ_8.SetActive(false);
                                    PersonSound.Play();
                                    
                                    personZ_9.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                            else if(zona11 == 1){
                                capacidad = capacidad + 1;
                                zona11 = zona11 - 1;
                                if(personZ_7.activeInHierarchy == true){
                                    personZ_7.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_8.activeInHierarchy == true){
                                    personZ_8.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_9.activeInHierarchy == true){
                                    personZ_9.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                        }
                        else if(capacidad == 2){
                            if(zona11 == 3 || zona11 == 2){
                                capacidad = capacidad + 2;
                                zona11 = zona11 - 2;
                                if(personZ_7.activeInHierarchy == true){
                                    personZ_7.SetActive(false);
                                    PersonSound.Play();
                                    if(personZ_8.activeInHierarchy == true){
                                        personZ_8.SetActive(false);
                                        PersonSound.Play();
                                    }
                                    else if(personZ_9.activeInHierarchy == true){
                                        personZ_9.SetActive(false);
                                        PersonSound.Play();
                                    }
                                }
                                else if(personZ_8.activeInHierarchy == true){
                                    personZ_8.SetActive(false);
                                    PersonSound.Play();
                                    
                                    personZ_9.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                            else if(zona11 == 1){
                                capacidad = capacidad + 1;
                                zona11 = zona11 - 1;
                                if(personZ_7.activeInHierarchy == true){
                                    personZ_7.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_8.activeInHierarchy == true){
                                    personZ_8.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_9.activeInHierarchy == true){
                                    personZ_9.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                        }
                        else if(capacidad == 3){
                            capacidad = capacidad + 1;
                            zona11 = zona11 - 1;
                            if(personZ_7.activeInHierarchy == true){
                                personZ_7.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(personZ_8.activeInHierarchy == true){
                                personZ_8.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(personZ_9.activeInHierarchy == true){
                                personZ_9.SetActive(false);
                                PersonSound.Play();
                            }
                        }
                        else{
                            capacidadL.SetActive(true);
                        }
                    }
                    break;
/*****************************************************************************************************************/
/*****************************************************************************************************************/
/*****************************************************************************************************************/
                case "zona12":
                    if(zona12 > 0){
                        if(capacidad == 0 || capacidad == 1 || capacidad == 2)
                        {
                            if(zona12 == 2)
                            {
                                capacidad = capacidad + 2;
                                zona12 = zona12 - 2;
                                personZ_10.SetActive(false);
                                personZ_11.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(zona12 == 1)
                            {
                                capacidad = capacidad + 1;
                                zona12 = zona12 - 1;
                                if(personZ_10.activeInHierarchy == true)
                                {
                                    personZ_10.SetActive(false);
                                    PersonSound.Play();
                                }
                                else if(personZ_11.activeInHierarchy == true)
                                {
                                    personZ_11.SetActive(false);
                                    PersonSound.Play();
                                }
                            }
                            
                        }
                        else if(capacidad == 3)
                        {
                            capacidad = capacidad + 1;
                            zona12 = zona12 - 1;
                            if(personZ_10.activeInHierarchy == true)
                            {
                                personZ_10.SetActive(false);
                                PersonSound.Play();
                            }
                            else if(personZ_11.activeInHierarchy == true)
                            {
                                personZ_11.SetActive(false);
                                PersonSound.Play();
                            }
                        }
                        else
                        {
                            capacidadL.SetActive(true);
                        }
                    }
                    break;
/*****************************************************************************************************************/
/*****************************************************************************************************************/
/*****************************************************************************************************************/
                case "helipuerto":
                    persona = persona + capacidad;
                    capacidad = 0;
                    capacidadL.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        private void WinGame()
        {
            if(level == 1)
            {
                if(persona == 5)
                {
                    FondoSound.volume = 0.0f;
                    WinSound.volume = 0.1f;
                    winL.SetActive(true);
                    nextLevelB.SetActive(true);
                    buttons.SetActive(false);
                    pauseB.SetActive(false);
                    resetB.SetActive(false);
                }  
            }
            else if(level == 2)
            {
                if(persona == 8)
                {
                    FondoSound.volume = 0.0f;
                    WinSound.volume = 0.1f;
                    winL.SetActive(true);
                    nextLevelB.SetActive(true);
                    buttons.SetActive(false);
                    pauseB.SetActive(false);
                    resetB.SetActive(false);
                }  
            }
            else if(level == 3)
            {
                if(persona == 11)
                {
                    FondoSound.volume = 0.0f;
                    WinSound.volume = 0.1f;
                    winL.SetActive(true);
                    nextLevelB.SetActive(true);
                    buttons.SetActive(false);
                    pauseB.SetActive(false);
                    resetB.SetActive(false);
                }  
            }
        }

        private void OnCollisionEnter(Collision objeto)
        {
            switch(objeto.gameObject.tag)
            {
                case "suelo":
                    IsOnGround = true;
                    break;
                case "montana":
                    dano = dano + 5.0f;
                    CollisionSound.Play();
                    if(dano >= 100.0f){
                        FinishDamage();
                    }
                    break;
                case "casa":
                    dano = dano + 4.0f;
                    CollisionSound.Play();
                    if(dano >= 100.0f){
                        FinishDamage();
                    }
                    break;
                case "arbol":
                    dano = dano + 3.0f;
                    CollisionSound.Play();
                    if(dano >= 100.0f){
                        FinishDamage();
                    }
                    break;
                case "roca":
                    dano = dano + 1.0f;
                    CollisionSound.Play();
                    if(dano >= 100.0f){
                        FinishDamage();
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnCollisionExit(Collision objeto)
        {
            switch(objeto.gameObject.tag)
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

            if (UIViewController.runtime.Level != null)
            {
                    UIViewController.runtime.Level.text = string.Format("Nivel [ {0} ] ", (int)level);
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
                if(level == 1)
                    UIViewController.runtime.Person.text = string.Format("Rescatados [ {0}/5 ] %", (int)persona);
                else if(level == 2)
                    UIViewController.runtime.Person.text = string.Format("Rescatados [ {0}/8 ] %", (int)persona);
                else if(level == 3)
                    UIViewController.runtime.Person.text = string.Format("Rescatados [ {0}/11 ] %", (int)persona);
            }

            if (UIViewController.runtime.EngineForceView != null)
            {
                if((int)EngineForce >= 0)
                    UIViewController.runtime.EngineForceView.text = string.Format("Fuerza motor [ {0} ] ", (int)EngineForce);
            }

            if (UIViewController.runtime.HeigthView != null)
            {   
                realHeight = maxHeight - transform.position.y;
                realHeight = 0.4f - realHeight;
                UIViewController.runtime.HeigthView.text = string.Format("Altura [ {0} ] m", (float)Math.Round((double)(realHeight * 100.0f),4));
            }
        }
    }
}