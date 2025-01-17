using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using System.Collections.Generic;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private bool m_IsRunning;
        [SerializeField] private float initialWalkSpeed;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_CrouchSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 3f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private float interactDistance; // interaction distance

        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private float highBobRange;
        [SerializeField] private float lowBobRange;
        [SerializeField] private float walkVHRatio;
        [SerializeField] private float runVHRatio;
        [SerializeField] private float walkHVRatio;
        [SerializeField] private float runHVRatio;
        [SerializeField] private float crouchVHRatio;
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
        [SerializeField] private bool jumpEnabled; // when checked enables jumping.
        [SerializeField] private bool landingSoundEnabled; // when checked enables landing sound.
        [SerializeField] private KeyCode interactKey; // interaction key.
        [SerializeField] private KeyCode flashlightKey; // flashlight key.

        [Header("Bed")]
        [SerializeField] private float height;
        [SerializeField] private float crouchHeight;
        [SerializeField] private float underBedHeight;

        private Camera m_Camera;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private bool m_Jump;
        private bool m_Jumping;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private AudioSource m_AudioSource;

        [Header("Hart")]
        [SerializeField] private float lookAtHartHeightAdjustment = 0.2f;
        private GameObject hart;
        private Quaternion spinToHart;

        private Vector3 lastPosition;
        private Vector3 bedHidingSpotPosition;

        private Light flashlight;

        [Header("Player Status")]
        [SerializeField] private bool isCaught;
        [SerializeField] private bool isUnderBed;
        [SerializeField] private bool isCrouching;

        [Header("Keys")]

        [SerializeField] private bool hasDrawerKey;
        [SerializeField] private bool hasFrontDoorKey;
        [SerializeField] private bool hasBackDoorKey;
        [SerializeField] private bool hasLandryBasementKey;

        public Quaternion SpinToHart { set { spinToHart = value; } }
        public bool IsCaught { get { return isCaught; } set { isCaught = value; } }
        public bool M_IsRunning { get { return m_IsRunning; } set { m_IsRunning = value; } }

        // Use this for initialization
        private void Start()
        {
            m_WalkSpeed = initialWalkSpeed;
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
            m_MouseLook.Init(transform, m_Camera.transform);

            hart = GameObject.FindGameObjectWithTag("Hart");
            flashlight = m_Camera.GetComponent<Light>();
            height = transform.localScale.y;
        }


        // Update is called once per frame
        private void Update()
        {
#if UNITY_EDITOR
            // Draw interaction line
            Debug.DrawRay(m_Camera.transform.position, m_Camera.transform.forward * interactDistance, Color.green);
#endif

            if(Input.GetKeyDown(interactKey)) {
                if(!isUnderBed)
                {
                    InteractionCheck();
                }
            }

            if(Input.GetKeyDown(flashlightKey)) {
                flashlight.enabled = !flashlight.enabled;
            }

            if(!isCaught) {
                RotateView();
            }

            // the jump state needs to read here to make sure it is not missed
            if(!m_Jump)
            {
                if(jumpEnabled) {
                    m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
                }
            }

            if(Input.GetButtonDown("Crouch") && !IsCaught) {
                isCrouching = !isCrouching;
                if(isCrouching) {
                    m_CharacterController.height = crouchHeight;
                    m_WalkSpeed = m_CrouchSpeed;
                    //m_Camera.transform.localPosition = new Vector3(0, crouchHeight, 0);
                }
                else {
                    m_CharacterController.height = height;
                    m_WalkSpeed = initialWalkSpeed;
                    //m_Camera.transform.localPosition = new Vector3(0, height, 0);
                }
            }

            if(!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                if(landingSoundEnabled)
                {
                    PlayLandingSound();
                }
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if(!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;

            // rotate to Hart.
            if(isCaught) {
                LookAtHart();
            }
        }


        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            if(isCaught) {
                //change height to stand up.
                transform.localScale = new Vector3(
                    transform.localScale.x,
                    height,
                    transform.localScale.z);
            }
            else {
                if(isUnderBed) {
                    transform.position = bedHidingSpotPosition;
                    if(Input.GetKeyDown(interactKey)) {
                        // get out of bed.
                        transform.position = lastPosition;
                        isUnderBed = !isUnderBed;
                        //change height to get out of bed.
                        transform.localScale = new Vector3(
                            transform.localScale.x,
                            height,
                            transform.localScale.z);
                    }
                }
                else {
                    float speed;
                    GetInput(out speed);
                    // always move along the camera forward as it is the direction that it being aimed at
                    Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

                    // get a normal for the surface that is being touched to move along it
                    RaycastHit hitInfo;
                    Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                                       m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
                    desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

                    m_MoveDir.x = desiredMove.x * speed;
                    m_MoveDir.z = desiredMove.z * speed;


                    if(m_CharacterController.isGrounded) {
                        m_MoveDir.y = -m_StickToGroundForce;

                        if(m_Jump) {
                            m_MoveDir.y = m_JumpSpeed;
                            PlayJumpSound();
                            m_Jump = false;
                            m_Jumping = true;
                        }
                    }
                    else {
                        m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
                    }
                    m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

                    if(isCrouching) {
                        m_Camera.transform.localPosition = new Vector3(0, crouchHeight, 0);
                    }
                    else {
                        m_Camera.transform.localPosition = new Vector3(0, height, 0);
                        
                    }

                    ProgressStepCycle(speed);
                    UpdateCameraPosition(speed);

                    m_MouseLook.UpdateCursorLock();
                }
            }
        }

        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if(m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                             Time.fixedDeltaTime;
            }

            if(!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if(!m_CharacterController.isGrounded || m_IsWalking)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if(!m_UseHeadBob)
            {
                return;
            }
            if(m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            //speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;

            if(m_IsWalking) {
                speed = m_WalkSpeed;
                m_HeadBob.VerticaltoHorizontalRatio = walkVHRatio;
                m_HeadBob.HorizontaltoVerticalRatio = walkHVRatio;
                m_HeadBob.SetHorizontalBobRange = highBobRange;
                m_HeadBob.SetVerticalBobRange = lowBobRange;
            }
            else {
                speed = m_RunSpeed;
                m_HeadBob.VerticaltoHorizontalRatio = runVHRatio;
                m_HeadBob.HorizontaltoVerticalRatio = runHVRatio;
                m_HeadBob.SetHorizontalBobRange = highBobRange;
                m_HeadBob.SetVerticalBobRange = highBobRange;
            }

            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if(m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if(m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }


        private void RotateView()
        {
                m_MouseLook.LookRotation(transform, m_Camera.transform);
        }

        private void LookAtHart()
        {
            Vector3 direction = new Vector3(hart.transform.position.x,
                hart.transform.position.y - lookAtHartHeightAdjustment,
                hart.transform.position.z) - transform.position;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), 5 * Time.deltaTime);
            m_Camera.transform.rotation = new Quaternion(0f, 0f, 0f, 0);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if(m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if(body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }

        private void InteractionCheck()
        {
            RaycastHit hit;
            int layerMask = 1 << 9;
            layerMask = ~layerMask;
            if(Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out hit, interactDistance, layerMask))
            {

                #region door interactions

                if(hit.transform.GetComponent<Door>())
                {
                    Door door = hit.transform.GetComponent<Door>();

                    if(door.IsLocked)
                    {
                        switch(door.DoorID)
                        {
                            case "FrontDoor":
                                if(hasFrontDoorKey) { door.Interact(); } else { door.Locked(); }
                                break;

                            case "BackDoor":
                                if(hasBackDoorKey) { door.Interact(); } else { door.Locked(); }
                                break;
                            case "LaundryBasement":
                                if(hasLandryBasementKey) { door.Interact(); } else { door.Locked(); }
                                break;
                            default:
                                Debug.Log("Unknown door.");
                                break;
                        }
                    }
                    else
                    { door.Interact(); }
                }

                #endregion

                #region drawer interactions

                if(hit.transform.GetComponent<Drawer>())
                {
                    Drawer drawer = hit.transform.GetComponent<Drawer>();
                    if(drawer.IsLocked)
                    {
                        if(hasDrawerKey)
                        {
                            drawer.Interact();
                            // consume drawer key.
                            hasDrawerKey = false;
                        }
                    }
                    else
                    {
                        drawer.Interact();
                    }
                }

                #endregion

                #region item interactions

                if(hit.transform.GetComponent<Item>())
                {
                    Item item = hit.transform.GetComponent<Item>();

                    switch(item.ItemName)
                    {
                        case "KeyFrontDoor":
                            hasFrontDoorKey = true;
                            break;
                        case "KeyBackDoor":
                            hasBackDoorKey = true;
                            break;
                        case "":
                            Debug.Log("No item");
                            break;
                    }

                    Destroy(hit.transform.gameObject);
                }
                #endregion

                #region bed interactions

                if(hit.transform.GetComponent<Bed>())
                {
                    Bed bed = hit.transform.GetComponent<Bed>();

                    if(!isUnderBed)
                    {
                        // change height to fit under bed.
                        transform.localScale = new Vector3(
                            transform.localScale.x,
                            underBedHeight,
                            transform.localScale.z);
                        // go under bed.
                        lastPosition = transform.position;
                        bedHidingSpotPosition = bed.transform.Find("HidingSpot").transform.position;
                        isUnderBed = !isUnderBed;
                    }
                }

                #endregion

            }
        }
    }
}
