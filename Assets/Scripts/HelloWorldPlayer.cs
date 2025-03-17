using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldPlayer : NetworkBehaviour {
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
        public SpriteRenderer renderer; //Used to apply a red tint to the character as they take more damage
        public Animator animator;
        public NetworkVariable<int> symbol = new NetworkVariable<int>(); //The symbol the character throws
        public GameObject uiElement;
        public float momentum = 0;
        public float speed = 2f;
        public float direction = 0;
        public NetworkVariable<float> damage = new NetworkVariable<float>(); //How much dmg the player has taken
        public NetworkVariable<float> isWalking = new NetworkVariable<float>(); //If they are walking, animation control
        public AudioSource audioSource;
        public AudioClip[] Trumpet;
        public NetworkVariable<int> tumpetToot = new NetworkVariable<int>(); //Which trumpet toot was tooted, - value means to play it

        public override void OnNetworkSpawn() {
            
            if (IsOwner) {
                Move();
            } else { //If not owner, then disable control
                enabled = false;
            }
            
            renderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            //renderer.color = <the color to change it to>
            GameObject uiInstance = Instantiate(uiElement);
            FollowPlayerUI followPlayerUI = uiInstance.GetComponent<FollowPlayerUI>();
            followPlayerUI.playerTransform = transform;
            SymbolChangeRpc(Random.Range(0,2));
        }

        public void Move() {
            SubmitPositionRequestRpc();
        }

        public void Damage() {
            SubmitAttackRequestRpc();
        }

        [Rpc(SendTo.Server)]
        public void SymbolChangeRpc(int i) {
            symbol.Value = i;
        }

        [Rpc(SendTo.Server)]
        void SubmitPositionRequestRpc(RpcParams rpcParams = default) {
            var nPosition = GetPositionOnPlane();
            transform.position = nPosition;
            Position.Value = nPosition;
            //transform.scale.x = 1/-1;
        }

        Vector3 GetPositionOnPlane() {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            
            Vector3 movement = new Vector3(x, y, 0);
            movement = Vector3.ClampMagnitude(movement, 1);
            var v_anim = 0f;

            if(x != 0 || y != 0) {
                v_anim = 1f;
                //UpdateAnimatonRpc(1f);
                //animator.SetFloat("xHorizontal",1f);
            } else {
                //UpdateAnimatonRpc(0f);
                //animator.SetFloat("xHorizontal",0f);
            }

            if(x != 0) {
                Vector3 modifScale = transform.localScale;
                if((x > 0 && modifScale.x < 0) || (x < 0 && modifScale.x > 0)) {
                    modifScale.x *= -1;
                }
                transform.localScale = modifScale;
            }

            UpdateAnimatonRpc(v_anim);
            return Position.Value + movement * speed * Time.deltaTime;
        }

        static Vector3 GetRandomPositionOnPlane() {
            return new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 1f);
        }

        [Rpc(SendTo.Server)]
        void UpdateAnimatonRpc(float anim, RpcParams rpcParams = default) {
            isWalking.Value = anim;
            //animator.SetFloat("xHorizontal", anim);
        }

        [Rpc(SendTo.Server)]
        void SubmitAttackRequestRpc(RpcParams rpcParams = default) {
            
        }

        void Update() {
            float scale = Mathf.Clamp(damage.Value/100,0,1);
            renderer.color = new Color(1-scale, 1-scale*0.6f, 1-scale*0.6f);
            transform.position = Position.Value;
            animator.SetFloat("xHorizontal", isWalking.Value);
            if(IsOwner) {
                if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
                    Move();
                }
                if(Input.GetKeyDown(KeyCode.Alpha1)) {
                    PlayTrumpetRpc(0);
                }
                if(Input.GetKeyDown(KeyCode.Alpha2)) {
                    PlayTrumpetRpc(1);
                }
                if(Input.GetKeyDown(KeyCode.Alpha3)) {
                    PlayTrumpetRpc(2);
                }
                if(Input.GetKeyDown(KeyCode.Alpha4)) {
                    PlayTrumpetRpc(3);
                }
                if(Input.GetKeyDown(KeyCode.Alpha5)) {
                    PlayTrumpetRpc(4);
                }
                if(Input.GetKeyDown(KeyCode.Alpha6)) {
                    PlayTrumpetRpc(5);
                }
            }
        }

        [Rpc(SendTo.Everyone)]
        private void PlayTrumpetRpc(int sfx, RpcParams rpcParams = default) {
            audioSource.PlayOneShot(Trumpet[sfx], 0.5f);
        }

        private void OnTriggerEnter(Collider collider) {
            if(IsServer) {
                HandleCollision(collider.gameObject);
            }
        }

        private void HandleCollision(GameObject obj) {
            if(obj.gameObject.tag == "DeathPlane") {
                //Reset damage, give few frames of invinc, and wait a few secs before tp back to middle
                DeathPlaneRpc();
                //audioSource.PlayOneShot(ouchie, volume);
            } else if(obj.gameObject.tag == "Player") {

            }
            //CollisionClientRpc(obj);
        }

        [Rpc(SendTo.Everyone)]
        private void DeathPlaneRpc(RpcParams rpcParams = default) {
            damage.Value = 0f;
            renderer.color = Color.white;
            Position.Value = new Vector3(0,0,0);
            transform.position = Position.Value;
        }

        [Rpc(SendTo.NotServer)]
        private void CollisionClientRpc(RpcParams rpcParams = default) {
            
        }
    }
}