using UnityEngine;
using System;
using UnityEngine.UI;

public class InteractionButtonUI : MonoBehaviour
{
    public Image LineImage; // 선 또는 배경용 Image 컴포넌트 참조
    public Image IconImage; // 버튼 아이콘을 표시할 Image 컴포넌트 참조
    public Text ButtonName; // 버튼에 표시할 텍스트(이름) 참조

    public Action_State actionState; // 이 버튼이 어떤 액션에 대응되는지 (런타임에 Initialize로 설정됨)
    private Button button; // 실제 클릭 이벤트를 받을 Button 컴포넌트

    private void Awake()
    {
        button = GetComponent<Button>(); // 같은 게임 오브젝트의 Button 컴포넌트 획득
        if (button != null)
        {
            button.onClick.AddListener(OnClickButton); // 클릭 시 OnClickButton 호출되도록 리스너 등록
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}에 Button 컴포넌트가 없어 클릭 이벤트를 등록할 수 없습니다.");
        }
    }

    // InteractionUI가 상태별로 버튼을 구성할 때 호출 - 이 버튼이 어떤 액션을 실행할지 지정
    public void Initialize(Action_State state)
    {
        actionState = state; // 전달받은 액션 상태 저장
    }

    // 버튼 클릭 시 해당 Action_State에 등록된 액션을 실행
    private void OnClickButton()
    {
        if (ActionHolder.Actions.TryGetValue(actionState, out Action action) && action != null)
        {
            action.Invoke(); // 등록된 델리게이트 실행
        }
        else
        {
            Debug.LogWarning($"Action_State {actionState}에 등록된 액션이 없습니다.");
        }
    }
}
