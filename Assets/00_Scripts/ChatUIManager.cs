using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatUIManager : MonoBehaviour
{
    public static ChatUIManager Instance; // 싱글톤 인스턴스 참조 ( 전액 접근용 )

    private void Awake() // Awake 단계 (오브젝트 초기화)에서 싱글톤 설정
    {
        if (Instance == null)
        {
            Instance = this; // 인스턴스가 비어있다면 현재 인스턴스를 할당
        }
    }

    public RectTransform content; // 채팅 콘텐츠를 담는 RectTransform ( 스크롤 컨텐츠 )
    public ScrollRect scrollRect; // 채팅 스크롤을 제어하는 ScrollRect 참조
    public InputField chatInputField; // 사용자가 메시지를 입력하는 InputField
    public Text chatLogText; // 채팅 로그를 표시하는 Text 컴포넌트
    public int MaxMessage; // 보관할 최대 메시지 수 제한

    // 내부적으로 보관하는 메시지 리스트 초기화
    private List<string> chatMessages = new List<string>();

    private void Update() // 매 프레임 호출되는 Update 메서드
    {
        // 현재 선택된 UI 요소가 채팅 입력란인지 확인
        if (EventSystem.current.currentSelectedGameObject == chatInputField.gameObject &&
            Input.GetKeyDown(KeyCode.Return)) // 엔터 키가 눌렸는지 확인
        {
            SendChatMessage(); // 메시지 전송 처리 호출
        } 
    }

    private void SendChatMessage() // 입력된 메시지를 전송하고 입력란을 초기화하는 메서드
    {
        string message = chatInputField.text; // 입력란의 현재 텍스트를 로컬 변수에 저장
        if (!string.IsNullOrEmpty(message)) // 메시지가 비어있지 않은지 확인
        {
            // ChatManager 싱글톤을 통해 메시지 전송 호출 ( 네트워크 / 로컬 처리 담당 )
            ChatManager.instance.SendMessageToChat(message);

            chatInputField.text = ""; // 입력란을 빈 문자열로 초기화

            chatInputField.ActivateInputField(); // 입력 필드에 포커스를 다시 부여 ( 연속 입력 가능 )
        }
    }

    public void DisplayMessage(string Message) // 외부에서 호출하여 새로운 메시지를 UI에 추가하는 메서드
    {
        chatMessages.Add(Message); // 내부 메시지 리스트에 새 메시지 추가

        if (chatMessages.Count > MaxMessage) // 메시지 수가 최대 허용치를 초과하면
        {
            chatMessages.RemoveAt(0); // 가장 오래된 메시지( 리스트의 첫 항목 )를 제거
        }

        scrollRect.verticalNormalizedPosition = 0.0f; // 스크롤을 아래로 고정 ( 최신 메시지가 보이도록 )
        UpdateChatLog(); // 화면에 표시되는 채팅 로그 업데이터 호출
    }

    private void UpdateChatLog() // 내부 메시지 리스트를 Text 컴포넌트에 반영하고 컨텐츠 크기를 조정하는 메서드
    {
        chatLogText.text = string.Join("\n", chatMessages); // 메시지들을 줄바꿈으로 연결하여 Text에 설정

        // 콘텐츠 높이를 텍스트 높이에 맞춰 조정 ( 여유 공간 포함 )
        content.sizeDelta = new Vector2(content.sizeDelta.x, 
            chatLogText.GetComponent<RectTransform>().sizeDelta.y + 100.0f);
    }
}
