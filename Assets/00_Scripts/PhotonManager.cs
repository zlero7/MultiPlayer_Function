using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    // 테스트/서비스 환경에서 유저들이 서로 다른 리전으로 흩어져 매칭되지 않는 문제를 막기 위해 리전 고정
    private const string FixedRegionCode = "kr"; // 필요 시 실제 서비스 리전 코드로 변경
    private const string RoomName = "MainRoom"; // 고정 방 이름으로 결정적인 매치메이킹 보장

    // 순간적인 네트워크 끊김(백그라운드 스로틀링, 와이파이 순단 등) 발생 시 즉시 퇴장 처리되지 않도록 재접속 유예 시간 부여
    private const int PlayerTtlMs = 60000; // 60초 안에 재접속하면 방/캐릭터 상태 유지

    private void Start() // MonoBehaviour 시작시 호출되는 초기화 메서드
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = FixedRegionCode; // 리전 고정 (비어있으면 클라이언트마다 다른 리전으로 자동 배정될 수 있음)
        PhotonNetwork.ConnectUsingSettings(); // Photon 설정을 사용해 마스터 서버에 연결 시도
    }

    public override void OnConnectedToMaster() // 마스터 서버에 연결되면 호출되면 콜백 오버라이드
    {
        Debug.Log("포톤 마스터 서버에 연결하였습니다."); // 연결 성공 로그 출력
        // 고정된 이름의 방에 참가하거나, 없으면 생성 (랜덤 매치메이킹의 리전/타이밍 race condition 방지)
        PhotonNetwork.JoinOrCreateRoom(RoomName, new RoomOptions
        {
            MaxPlayers = 2,
            PlayerTtl = PlayerTtlMs // 순간적인 연결 끊김에 재접속 유예를 줘서 바로 방에서 제거되지 않게 함
        }, TypedLobby.Default);
    }

    public override void OnJoinedRoom() // 방에 성공적으로 입장했을 때 호출되는 콜백
    {
        Debug.Log("룸에 접속하였습니다."); // 룸 입장 로그 출력
        SpawnPlayer(); // 플레이어 스폰 처리 호출

        ChatManager.instance.Initialize(); // 채팅 UI 초기화 호출
        BubbleUIManager.instance.InitializeBubble(); // 말풍선 UI 초기화 호출
    }

    // 연결이 끊겼을 때 호출되는 콜백 - 원인을 로그로 남기고 자동 재접속 시도
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Photon 연결이 끊겼습니다. 원인: {cause}. 재접속을 시도합니다.");

        // 서버 점검 등 재접속이 의미 없는 경우가 아니면 재접속 시도
        if (cause != DisconnectCause.ApplicationQuit)
        {
            PhotonNetwork.ConnectUsingSettings(); // 마스터 서버 재연결부터 다시 시도
        }
    }

    // Instantiate - 생성자
    // Destory - 파괴자

    void SpawnPlayer() // 플레이어 오브젝트를 생성하고 초기화하는 로컬 메서드
    {
        // 랜덤 스폰 위치 생성
        Vector3 spanPostion = new Vector3(Random.Range(-5.0f, 5.0f), 0.0f, Random.Range(-5.0f, 5.0f));
        // 네트워크 상에서 플레이어 프리팹 인스턴스화
        GameObject playerObject = PhotonNetwork.Instantiate("PlayerPrefab", spanPostion, Quaternion.identity);

        // 로컬 플레이어의 TagObject에 생성된 플레이어 오브젝트 참조 저장
        // PhotonNetwork.LocalPlayer.TagObject = playerObject;

        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber; // 로컬 플레이어의 고유 ActorNumber 가져오기
        playerObject.GetComponent<PlayerController>().Initialize(actorNumber); // 플레이어 컨트롤러 초기화
    }
}
