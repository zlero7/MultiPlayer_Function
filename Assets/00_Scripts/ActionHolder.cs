using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System;
using Photon.Pun;

public enum Action_State // 가능한 액션 상태를 정의하는 열거형
{
    None = 0, // 기본값 ( 아무 동작 X )
    InviteParty, // 파티 초대 액션
    Trade, // 거래 요청 액션
    InviteGuild // 길드 초대 액션
}

public class ActionHolder : MonoBehaviour
{
    // 프로젝트 전역에서 사용하는 스프라이트 아틀라스
    public static SpriteAtlas Atlas;

    // 액션상태 -> 실행 델리게이트 매핑
    public static Dictionary<Action_State, Action> Actions = new Dictionary<Action_State, Action>();
    public static PhotonView photonView; // 네트워크 호출용 PhotonView 참조 (정적)
    public static int TargetPlayerIndex; // 현재 대상 플레이어의 인덱스 (정적)

    // 아틀라스에서 이름으로 스프라이트를 가져오는 헬퍼
    public static Sprite GetAtlas(string temp)
    {
        // 이름에 해당하는 스프라이트 반환
        return Atlas.GetSprite(temp);
    }

    private void Start()
    {
        // 같은 게임 오브젝트의 PhotonView 컴포넌트 획득
        photonView = GetComponent<PhotonView>();

        // Resources 폴더의 "Atlas"라는 이름의 SpriteAtlas 로드
        Atlas = Resources.Load<SpriteAtlas>("Atlas");

        Actions[Action_State.InviteParty] = InviteParty; // 파티 초대 액션 등록
        Actions[Action_State.Trade] = Trade; // 거래 액션 등록
        Actions[Action_State.InviteGuild] = InviteGuild; // 길드 초대 액션 등록
    }

    #region Party // 파티 관련 메서드 블록 시작
    // 파티 초대 액션 실행 메서드 ( 정적 )
    public static void InviteParty()
    {
        Debug.Log("InviteParty() 호출됨"); // 디버그 로그 출력
    }
    #endregion

    #region Trade // 거래 관련 메서드 블록 시작
    // 거래 초대 액션 실행 메서드 ( 정적 )
    public static void Trade()
    {
        Debug.Log("Trade() 호출됨"); // 디버그 로그 출력
    }
    #endregion

    #region Guild // 길드 관련 메서드 블록 시작
    // 길드 초대 액션 실행 메서드 ( 정적 )
    public static void InviteGuild()
    {
        Debug.Log("InviteGuild() 호출됨"); // 디버그 로그 출력
    }
    #endregion
}
