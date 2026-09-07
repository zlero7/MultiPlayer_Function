using UnityEngine;
using System;
using UnityEngine.UI;

public class InteractionButtonUI : MonoBehaviour
{
    public Image LineImage; // 선 또는 배경용 Image 컴포넌트 참조
    public Image IconImage; // 버튼 아이콘을 표시할 Image 컴포넌트 참조
    public Text ButtonName; // 버튼에 표시할 텍스트(이름) 참조

    Button button; // 내부에서 사용할 Button 컴포넌트 캐시

    Action_State m_Action; // 버튼에 연결된 동작 상태를 저장하는 필드

    private void Awake()
    {
        // 현재 게임 오브젝트에서 Button 컴포넌트를 찾아 캐시
        button = GetComponent<Button>();
    }

    // 특정 상태로 버튼을 초기화하는 공개 메서드
    public void Initialize(Action_State state)
    {
        // 전달된 상태를 내부 필드에 저장
        m_Action = state;

        // 상태가 None이면 비활성 처리
        if (m_Action == Action_State.None)
        {
            // 이미지 색을 검정 ( 알파 유지 )으로 변경하여 비활성화 표시
            GetComponent<Image>().color = new Color(0, 0, 0, GetComponent<Image>().color.a);
            return; // 초기화 중단
        }

        // 아이콘 이미지 게임 오브젝트 활성화
        IconImage.gameObject.SetActive(true);
        // 상태 이름으로 스프라이트를 가져와 아이콘에 할당
        IconImage.sprite = ActionHolder.GetAtlas(state.ToString());

        button.onClick.RemoveAllListeners(); // 기존 클릭 리스너 모두 제거(중복 방지)
        // 클릭시 해당 상태에 대응하는 액션 실행하도록 리스너 추가
        button.onClick.AddListener(() => ActionHolder.Actions[state]());
    }
}
