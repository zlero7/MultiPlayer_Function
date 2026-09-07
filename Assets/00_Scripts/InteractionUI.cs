using UnityEngine;
using System;

// 상호작용 상태를 정의하는 열거형
public enum Interaction_State
{
    Player, // 플레이어 상호작용 상태
}

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private float yPosFloat; // 에디터에서 설정 가능한 Y 오프셋 값
    Animator animator; // Animator 컴포넌트 참조 변수
    PlayerController playerController; // 대상 플레이어의 컴포넌트 참조

    Interaction_State m_State; // 현재 상호작용 상태 저장 필드
    public InteractionButtonUI[] interactionButtons; // 상호작용 버튼 UI 배열 공개 필드

    private void Awake()
    {
        // 같은 게임 오브젝트에서 Animator 컴포넌트 획득
        animator = GetComponent<Animator>();
    }

    // 매 프레임 호출: UI 위치를 플레이어에 맞춰 업데이트
    private void Update()
    {
        if (playerController != null)
        {
            // 플레이어 위치에 Y 오프셋을 더해 타깃 월드 좌표 계산
            Vector3 targetPosition = playerController.transform.position + new Vector3(0.0f, yPosFloat, 0.0f);
            // 월드 좌표를 스크린 좌표로 변환하여 UI 위치 설정
            transform.position = Camera.main.WorldToScreenPoint(targetPosition);
        }
    }

    // 초기화 메서드: 컨트롤러와 상태 설정 및 버튼 초기화
    public void Initialize(PlayerController controller, Interaction_State state)
    {
        playerController = controller; // 전달된 플레이어 컨트롤러 저장
        m_State = state; // 전달된 상호작용 상태 저장

        // 상태에 따른 액션 배열 생성/조회
        var actions = InteractionActions(state);

        // 모든 버튼에 대해 반복
        for (int i = 0; i < interactionButtons.Length; i++)
        {
            // 각 버튼을 해당 액션으로 초기화
            interactionButtons[i].Initialize(actions[i]);
        }

        animator.Play("Hexagon_Open"); // 오픈 애니메이션 재생
    }

    // 비활성화 애니메이션 재생 시작
    public void DeactiveObject()
    {
        // Animator가 존재하고, 해당 Animator의 게임 오브젝트가 활성화 되어 있으며 , 
        // ANimator 컴포넌트가 활성화된 경우에만 애니메이션을 재생하도록 안전 검사
        if (animator != null && animator.gameObject.activeInHierarchy && animator.isActiveAndEnabled)
        {
            animator.Play("Hexagon_Hide"); // 숨김 애니메이션 재생
        }
    }

    // 즉시 게임 오브젝트 비활성화 ( 간단 람다 표현 )
    public void Deactive() => gameObject.SetActive(false);

    // 주어진 상태에 따라 액션 배열을 구성하여 반환
    private Action_State[] InteractionActions(Interaction_State state)
    {
        // 고정 길이 6의 액션 배열 초기화
        Action_State[] actions = new Action_State[6];

        // 상태별로 액션을 할당
        switch (state)
        {
            case Interaction_State.Player: // 플레이어 상태의 경우
                actions[0] = Action_State.InviteParty; // 첫 버튼 : 파티 초대
                actions[1] = Action_State.Trade; // 두번째 버튼 : 거래
                actions[2] = Action_State.InviteGuild; // 세번째 버튼 : 길드 초대
                break; // 해당 케이스 종료
        }
        return actions; // 구성된 액션 배열 반환
    }
}
