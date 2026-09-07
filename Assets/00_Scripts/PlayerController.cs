using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController characterController; // 캐릭터 이동 처리용 Component 참조 변수
    private Animator animator; // 애니메이터 ( 애니메이션 제어 ) 참조 변수

    // 소유자(플레이어) 고유 번호를 읽기 전용으로 외부 공개
    public int OwnerActorNumber { get; private set; }

    public float speed; // 이동 속도 ( 인스펙터에서 설정 )
    PhotonView view; // PhotonView 컴포넌트 참조 ( 네트워킹 동기화용 )


    public void Initialize(int actorNumber) // 초기화 메서드
    {
        if (isMinePhoton()) // 이 뷰가 로컬 플레이어의 것인지 확인
        {
            OwnerActorNumber = actorNumber; // 로컬 소유자 번호 설정

            // 모든 클라이언트에 RPC로 소유자 번호 전파 ( 버퍼링 포함 )
            // RpcTarget.AllBuffered: 현재 클라이언트와 나중에 접속하는 클라이언트 모두에게 RPC 호출을 전송
            view.RPC("SetActorNumber", RpcTarget.AllBuffered, actorNumber);
        }
    }

    public bool isMinePhoton() // 현재 PhotonView가 로컬 플레이어 소유인지 반환
    {
        return view.IsMine; // PhotonView의 IsMine값을 그대로 반환
    }

    // RPC : Remote Procedure Call - 네트워크 상의 다른 플레이어가 실행 중인 특정 메서드 호출
    [PunRPC] // 이 메서드는 RPC로 호출될 수 있음을 표시
    public void SetActorNumber(int actorNumber) // 네트워크로 전달된 소유자 번호를 설정하는 RPC메서드
    {
        OwnerActorNumber = actorNumber; // 전달받은 번호를 로컬에 저장
    }

    private void Awake()
    {
        // 같은 GameObjact에서 CharacterController 컴포넌트 가져오기
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>(); // 같은 GameObject에서 Animator 컴포넌트 가져오기
        view = GetComponent<PhotonView>();

        // Register this object as the owner's TagObject.
        // TagObject is a client-local-only field (not networked),
        // so every client must set it locally for every player object it sees.
        if (view.Owner != null)
        {
            view.Owner.TagObject = gameObject;
        }
    }

    private void Update()
    {
        if (!view.IsMine) return; // 이 인스턴스가 로컬 소유가 아니면 입력과 이동 처리를 하지 않음

        float h = Input.GetAxis("Horizontal"); // 수평 입력 값 가져오기 ( A/D, 좌우 키 )
        float v = Input.GetAxis("Vertical"); // 수직 입력 값 가져오기 ( W/S , 상하 키 )

        Vector3 movement = new Vector3(h, 0, v); // 입력을 기반으로 한 이동 벡터 생성 ( Y 는 0 )

        if (movement.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg; // 이동 벡터의 방향을 각도로 변환
            transform.rotation = Quaternion.Euler(0, targetAngle, 0); // 플레이어를 이동 방향으로 회전시킴 ( Y좌표만 변경 )

            characterController.Move(movement * speed * Time.deltaTime); // CharacterController로 실제 이동 처리

            animator.SetFloat("Movement", movement.magnitude); // 애니메이터에 이동 크기 값 전달하여 애니메이션 제어
        }
        else
        {
            animator.SetFloat("Movement", 0); // 입력이 없으면 애니메이터에 0 전달하여 대기 애니메이션으로 전환
        }
    }
}