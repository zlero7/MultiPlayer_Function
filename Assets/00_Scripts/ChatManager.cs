using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;
using System.Linq;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    public static ChatManager instance; // 싱글톤 인스턴스 저장용 public 정적 필드

    void Awake()
    {
        // 인스턴스가 없으면 현재 인스턴스를 할당 ( 싱글톤 간단 구현 )
        if (instance == null) instance = this;
    }

    private ChatClient chatClient; // Photon Chat 클라이언트 인ㅅ느턴스 저장용 필드
    private string chatChannel = "GlobalChannel"; // 기본 채팅 채널 이름을 문자열로 초기화

    public void Initialize() // 채팅 초기화용 공개 메서드
    {
        if (string.IsNullOrEmpty(PhotonNetwork.NickName)) // PhotonNetwork의 닉네임이 비어있으면
        {
            // 기본 닉네임을 ActorNumber기반으로 설정
            PhotonNetwork.NickName = $"Player_{PhotonNetwork.LocalPlayer.ActorNumber}";
        }

        // IChatClientListener로 이 객체를 전달하여 ChatClient 인스턴스 생성
        chatClient = new ChatClient(this);

        // Chat AppId로 연결 시도, 앱 버전과 인증 값 ( 닉네임 ) 전달
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
            PhotonNetwork.AppVersion, new AuthenticationValues(PhotonNetwork.NickName));
    }

    private void Update() // Unity Update 콜백 ( 매 프레임 호출 )
    {
        chatClient?.Service(); // chatClient가 null이 아니면 Service 호출해 네트워크 메시지 처리
    }

    public void SendMessageToChat(string message) // 채팅으로 메시지 전송하는 공개 메서드
    {
        if (!string.IsNullOrEmpty(message)) // 메시지가 null 또는 빈 문자열이 아닌지 확인
        {
            chatClient.PublishMessage(chatChannel, message); // 지정된 채널에 메시지 개시
        }
    }

    #region ChatClient_Interface
    public void DebugReturn(DebugLevel level, string message)
    {
        switch (level)
        {
            case DebugLevel.ERROR:
                Debug.LogError($"Photon CHat Error : {message}"); // 에러 레벨이면 에러 로그 출력
                break;
            case DebugLevel.WARNING:
                Debug.LogWarning($"Photon CHat Warning : {message}"); // 경고 레벨이면 경고 로그 출력
                break;
            default:
                Debug.Log($"Photon Chat : {message}"); // 그 외에는 일반 로그 출력
                break;
        }
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"Chat State Changed : {state}"); // 상태 변경 로그
        // 
        // COnnectedToNameServer : Name Server와의 연결된 상태
        // Authenticated : 인증이 완료되어 채팅 서버와 연결할 준비가 된 상태
        // Disconnected : 연결이 끊긴 상태
        // ConnectedToFrontEnd : Front- End 서버와 연결된 상태

        switch (state)
        {
            case ChatState.ConnectedToNameServer:
                Debug.Log("Connected to Name Server"); // Name Server 연결된 로그
                break;
            case ChatState.Authenticated:
                Debug.Log("Authenticated successfully."); // 인증 완료 로그
                break;
            case ChatState.Disconnected:
                Debug.Log("Disconnected from Chat Server"); // 연결 끊김 경고 로그
                break; 
            case ChatState.ConnectedToFrontEnd:
                Debug.Log("Connected to Front End Server"); // FrontEnd 연결 로그
                break;
            default:
                Debug.Log($"Unhandled Chat State : {state}"); // 처리되지 않은 상태 로그
                break;
        }
    }

    public void OnConnected()
    {
        Debug.Log("Photon Connected!"); // 연결 완료 로그 출력
        chatClient.Subscribe(new string[] { chatChannel }); // 기본 채널을 구독 요청
    }

    public void OnDisconnected()
    {
        Debug.Log("Photon Disconnected!"); // 연결 끊김 로그 출력
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for(int i = 0; i < senders.Length; i++) // 받은 메시지 배열을 순회
        {
            // 보낸 사람과 메시지를 조합하여 문자열 생성
            string receivedMessage = $"{senders[i]} : {messages[i]}";
            Debug.Log($"[{channelName}] {receivedMessage}"); // 채널과 수신 메시지 로그 출력

            ChatUIManager.Instance.DisplayMessage(receivedMessage); // UI 매니저에 메시지 표시 요청

            // 발신자 닉네임으로 해당 플레이어를 찾아 말풍선 표시
            Photon.Realtime.Player senderPlayer = PhotonNetwork.PlayerList.FirstOrDefault(p => p.NickName == senders[i]);
            if (senderPlayer != null)
            {
                BubbleUIManager.instance.ShowBubbleForPlayer(senderPlayer.ActorNumber, messages[i].ToString());
            }
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        for(int i = 0; i < channels.Length; i++)
        {
            if (results[i])
            {
                Debug.Log($"채널 {channels[i]} 구독 성공");
            }
            else
            {
                Debug.LogWarning($"채널 {channels[i]} 구독 실패");
            }
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
