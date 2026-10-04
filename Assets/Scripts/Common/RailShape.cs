
public enum RailShape 
{
    // 레일 타일을 설치 및 회수할떄 레일의 모양을 정의
    
    VerticalLine,       // 1. 직선 레일의 세로 형태
    HorizontalLine,     // 2. 직선 레일의 가로 형태 
    DownToRightCurve,   // 3, (┌ 방향 곡선]: ⬇️➡️ 
    UpToRightCurve,     // 4. (└ 방향 곡선): ⬆️➡️
    UpToLeftCurve,      // 5. (┘ 방향 곡선):  ⬆️⬅️
    DownToLeftCurve,    // 6. (┐ 방향 곡선): ⬇️⬅️
}
