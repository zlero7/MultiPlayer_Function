using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public enum Action_State // 가능한 액션 상태를 정의하는 열거형
{
    None = 0, // 기본값 ( 아무 동작 X )
    InviteParty, // 파티 초대 액션
    Trade, // 거래 요청 액션
    InviteGuild // 길드 초대 액션
}

public class ActionHolder : MonoBehaviour
{
    // 액션상태 -> 실행 델리게이트 매핑
    public static Dictionary<Action_State, Action> Actions = new Dictionary<Action_State, Action>();

    private void Start()
    {
        Actions[Action_State.InviteParty] = InviteParty; // 파티 초대 액션 등록
        Actions[Action_State.Trade] = Trade; // 거래 액션 등록
        Actions[Action_State.InviteGuild] = InviteGuild; // 길드 초대 액션 등록
    }

    #region Party // 파티 관련 메서드 블록 시작
    // 파티 초대 액션 실행 메서드 ( 정적 )
    public static void InviteParty()
    {
        Debug.Log("[Action] 파티 초대 액션이 실행되었습니다."); // 동작 확인용 디버그 로그
    }
    #endregion

    #region Trade // 거래 관련 메서드 블록 시작
    // 거래 초대 액션 실행 메서드 ( 정적 )
    public static void Trade()
    {
        Debug.Log("[Action] 거래 요청 액션이 실행되었습니다."); // 동작 확인용 디버그 로그
    }
    #endregion

    #region Guild // 길드 관련 메서드 블록 시작
    // 길드 초대 액션 실행 메서드 ( 정적 )
    public static void InviteGuild()
    {
        Debug.Log("[Action] 길드 초대 액션이 실행되었습니다."); // 동작 확인용 디버그 로그
    }
    #endregion
}
