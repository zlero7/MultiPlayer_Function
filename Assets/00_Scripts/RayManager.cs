using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class RayManager : MonoBehaviour
{
    [SerializeField] private InteractionUI interactionUI; // 인게임 상호작용 UI 참조 ( 인스펙터 노출 )
    [SerializeField] private LayerMask playerLayer; // 플레이어 레이어 마스크
    [SerializeField] private GraphicRaycaster graphicRaycaster; // Canvas에 연결된 GraphicRaycaster 참조
    [SerializeField] private EventSystem eventSystem; // 현재 EventSystem 참조

    // 매 프레임 입력 검사
    private void Update()
    {
        // 왼쪽 클릭이고 UI 위가 아니면
        if (Input.GetMouseButtonDown(0))
        {
            interactionUI.DeactiveObject(); // 상호작용 UI 숨김 애니메이션 재생
        }
        if (Input.GetMouseButtonDown(1)) // 오른쪽 클릭이고 UI 위가 아니면
        {
            FindPlayerClick(); // 플레이어 클릭 처리 시도
        }
    }

    // 월드에서 플레이어를 클릭했는지 확인하고 처리
    private void FindPlayerClick()
    {
        // 화면 좌표를 월드 레이로 변환
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 물리 충돌 정보를 받을 구조체
        RaycastHit hit;

        // 플레이어 레이어로 물리 레이캐스트 실행
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, playerLayer))
        {
            // 히트한 콜라이더에서 플레이어 컴포넌트 획득
            PlayerController player = hit.collider.GetComponent<PlayerController>();

            // 플레이어 컴포넌트가 존재하면
            if (player != null)
            {
                // 상호작용 UI 활성화
                interactionUI.gameObject.SetActive(true);
                // UI 초기화 및 오픈 ( 메서드 이름 일치 확인 필요 )
                interactionUI.Initialize(player, Interaction_State.Player);
            }
            else
            {
                interactionUI.DeactiveObject(); // 플레이어가 아닌 경우 UI 숨김 애니메이션 재생
            }
        }
        else
        {
            interactionUI.DeactiveObject(); // 레이 적중이 없으면 UI 숨김 애니메이션 재생
        }
    }
}
