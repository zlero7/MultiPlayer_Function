using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;

public class BubbleUIManager : MonoBehaviourPunCallbacks
{
    public static BubbleUIManager instance = null; // 싱글톤 인스턴스 참조용 정적 필드

    private void Awake()
    {
        if (instance == null) // 인스턴스가 아직 없으면
        {
            instance = this; // 현재 객체를 싱글톤 인스턴스로 지정
        }
    }

    public GameObject bubblePrefab; // 생성할 말풍선 프리팹을 에디터에서 할당할 필드
    // 플레이어 ActorNumber를 키로 한 말풍선 컴포넌트 딕셔너리
    private Dictionary<int, SpeechBubble> playerBubbles = new Dictionary<int, SpeechBubble>();

    public void InitializeBubble() // (초기화) 모든 플레이어에 대해 말풍선 생성 시작 메서드
    {
        CreateBubbleForPlayer(PhotonNetwork.LocalPlayer.ActorNumber); // 로컬 플레이어용 말풍선 먼저 생성

        foreach (Player player in PhotonNetwork.PlayerList) // 현재 룸에 있는 모든 플레이어를 순회
        {
            if ((player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber))
            {
                CreateBubbleForPlayer(player.ActorNumber); // 해당 플레이어용 말풍선 생성
            }
        }
    }

    // 플레이어 입장 시 호출되는 Photon 콜백 오버라이드
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        CreateBubbleForPlayer(newPlayer.ActorNumber); // 새로운 플레이어 입장시 말풍선 생성
    }

    // 플레이어 퇴장 시 호출되는 Photon 콜백 오버라이드
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveBubbleForPlayer(otherPlayer.ActorNumber); // 퇴장한 플레이어의 말풍선 제거
    }

    // 특정 ActorNumber에 대한 말풍선 생성 도우미 메서드
    private void CreateBubbleForPlayer(int actorNumber)
    {
        if (!playerBubbles.ContainsKey(actorNumber)) // 이미 말풍선이 등록되어 있지 않다면
        {
            // 프리팹을 현재 오브젝트의 자식으로 인스턴스화
            GameObject bubble = Instantiate(bubblePrefab, transform);
            bubble.SetActive(false); // 초기에는 비활성화 상태로 설정

            // 생성된 오브젝트에서 SpeechBubble 컴포넌트 획득
            SpeechBubble speech = bubble.GetComponent<SpeechBubble>();

            // 말풍선 오브젝트가 비활성 상태라 SpeechBubble 자신은 코루틴을 돌릴 수 없으므로,
            // 항상 활성 상태인 BubbleUIManager(this)가 대상을 찾을 때까지 대신 재시도한다.
            StartCoroutine(BubbleByActorNumberDelay(speech, actorNumber));

            playerBubbles[actorNumber] = speech; // 딕셔너리에 ActorNumber와 말풍선 컴포넌트 저장
        }
    }

    // 대상 플레이어 오브젝트/OwnerActorNumber가 아직 네트워크 동기화되지 않았을 수 있으므로 찾을 때까지 반복 시도
    IEnumerator BubbleByActorNumberDelay(SpeechBubble speech, int actorNumber)
    {
        while (speech.target == null) // 아직 대상을 못 찾았다면
        {
            speech.Initialize(actorNumber); // 말풍선 초기화(대상 탐색) 시도
            if (speech.target != null)
            {
                break; // 찾았으면 즉시 종료
            }
            yield return new WaitForSeconds(0.1f); // 0.1초 간격으로 재시도
        }
    }

    // 특정 플레이어의 말풍선을 제거하는 메서드
    private void RemoveBubbleForPlayer(int actorNumber)
    {
        if (playerBubbles.ContainsKey(actorNumber))
        {
            Destroy(playerBubbles[actorNumber].gameObject); // 해당 컴포넌트가 붙은 오브젝트를 파괴
            playerBubbles.Remove(actorNumber); // 딕셔너리에서 항목 제거
        }
    }

    // 특정 플레이어에게 말풍선을 보여주고 텍스트를 설정하는 메서드
    public void ShowBubbleForPlayer(int actorNumber, string message)
    {
        if (playerBubbles.TryGetValue(actorNumber, out SpeechBubble bubble))
        {
            bubble.gameObject.SetActive(true); // 말풍선 오브젝트 활성화하여 표시
            bubble.GetComponent<SpeechBubble>().SetText(message); // 말풍선 텍스트 설정
        }
    }
}