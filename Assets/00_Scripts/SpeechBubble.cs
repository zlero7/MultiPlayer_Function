using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{
    [Range(0.0f, 5.0f)] // 인스턴스에서 허용되는 Y 오프셋 범위 지정
    public float yPosFloat = 2.0f; // 말풍선이 플레이어 위로 올라갈 기본 높이

    [HideInInspector] public Transform target; // 말풍선이 따라갈 대상 Transform(인스펙터 숨김)

    public Text SpeechText; // 말풍선에 표시할 텍스트 컴포넌트 참조
    Animator animator; // Animator 컴포넌트 캐시용 필드
    Coroutine coroutine; // 현재 실행 중인 코루틴 레퍼런스 저장

    // 플레이어 액터 번호로 초기화 ( 메서드명은 원본 유지 )
    // 대상을 못 찾으면 target은 null로 남고, 재시도는 BubbleUIManager가 담당(비활성 오브젝트라 코루틴 사용 불가)
    public void Initialize(int actorNumber)
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>(); // 현재 게임 오브젝트에서 Animator를 가져옴
        }

        // 액터 번호로 대상 플레이어 Transform 찾기
        target = FindPlayerTransformByActorNumber(actorNumber);
    }

    // 액터 번호로 PlayerController의 Transform 탐색
    private Transform FindPlayerTransformByActorNumber(int targetActorNumber)
    {
        // 모든 PlayerController를 찾아 배열로 반환
        PlayerController[] allPlayers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

        foreach (PlayerController player in allPlayers) // 각 플레이어를 순회
        {
            if (player.OwnerActorNumber == targetActorNumber) // 액터 번호가 일치하면
            {
                return player.transform; // 해당 플레이어의 Transform 반환
            }
        }

        return null; // 일치하는 플레이어가 없으면 null 반환
    }

    // 말풍선 텍스트 설정 및 표시 제어
    public void SetText(string message)
    {

        if (coroutine != null) // 이미 숨김/표시 코루틴이 실행중이면
        {
            StopCoroutine(coroutine); // 기존 코루틴 중지
            coroutine = StartCoroutine(HideCoroutine(0.0f, () =>
            {
                SpeechText.text = message; // 텍스트 내용 갱신
                animator.Play("SpeechBubble_Open"); // 열기 애니메이션 재생 ( 애니메이션명은 원본 유지 )
                coroutine = StartCoroutine(HideCoroutine(3.0f, null)); // 일정 시간 후 자동 숨김 코루틴 시작
            }));
            return; // 이후 로직 실행 중단
        }
        SpeechText.text = message; // 텍스트 설정
        animator.Play("SpeechBubble_Open"); // 열기 애니메이션 재생
        coroutine = StartCoroutine(HideCoroutine(3.0f, null)); // 3초후 숨김 코루틴 시작
    }

    // 일정 시간 후 숨기고 선택적 액션 실행하는 코루틴
    IEnumerator HideCoroutine(float timer, Action action)
    {
        yield return new WaitForSeconds(timer); // 지정된 시간만큼 대기
        animator.Play("SpeechBubble_Hide"); // 숨김 애니메이션 재생
        yield return new WaitForSeconds(0.3f); // 애니메이션이 끝날 때까지 추가 대기
        if (action != null)
        {
            action?.Invoke(); // 콜백 실행
            yield break; // 코루틴 종료
        }
        else
        {
            gameObject.SetActive(false); // 콜백이 없으면 게임 오브젝트 비활성화
        }

        coroutine = null; // 코루틴 레퍼런스 초기화
    }

    // 프레임의 후반에서 말풍선 위치를 카메라 기준 화면 좌표로 업데이트
    private void LateUpdate()
    {
        if (target != null) // 대상이 할당되어 있으면
        {
            // 대상의 월드 포지션에 y 오프셋 추가
            Vector3 targetPosition = target.position + new Vector3(0.0f, yPosFloat, 0.0f);

            // 월드 좌표를 스크린 좌표로 변환하여 위치 적용
            transform.position = Camera.main.WorldToScreenPoint(targetPosition);
        }
    }
}